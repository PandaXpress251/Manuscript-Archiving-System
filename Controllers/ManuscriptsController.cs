using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Thesis_Capstone_Archive.Data;
using Thesis_Capstone_Archive.Models;
using Thesis_Capstone_Archive.Models.ViewModels;
using Thesis_Capstone_Archive.Models.JuctionTables;
using Thesis_Capstone_Archive.Helpers;

namespace Thesis_Capstone_Archive.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class ManuscriptsController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ManuscriptsController(Thesis_Capstone_ArchiveContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }


        // GET: Manuscripts
        public async Task<IActionResult> Index(string searchBy, string searchString, int? systemTypeId, int? systemCategoryId,
                                                int? termId, int? programId, int? pageNumber, int? pageSize)
        {
            // Set a default page size if not provided
            pageSize ??= 10;

            // Store the current filters and page size in ViewBag to maintain state
            ViewBag.CurrentFilter = searchString;
            ViewBag.SearchBy = searchBy;
            ViewBag.CurrentSystemType = systemTypeId;
            ViewBag.CurrentSystemCategory = systemCategoryId;
            ViewBag.CurrentTerm = termId;
            ViewBag.CurrentProgram = programId;
            ViewBag.PageSize = pageSize;

            // Populate dropdown lists
            ViewBag.SystemTypes = await _context.SystemTypes.Select(st => new SelectListItem
            {
                Value = st.SystemTypeID.ToString(),
                Text = st.SystemTypeName
            }).ToListAsync();

            ViewBag.SystemCategories = await _context.SystemCategories.Select(sc => new SelectListItem
            {
                Value = sc.SystemCategoryID.ToString(),
                Text = sc.SystemCategoryName
            }).ToListAsync();

            ViewBag.Terms = await _context.Terms.Select(t => new SelectListItem
            {
                Value = t.TermID.ToString(),
                Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
            }).ToListAsync();

            ViewBag.AcademicPrograms = await _context.AcademicProgram.Select(ap => new SelectListItem
            {
                Value = ap.ProgramID.ToString(),
                Text = ap.ProgramName
            }).ToListAsync();

            // Start with the full set of manuscripts, including related properties
            var manuscripts = _context.Manuscripts
                .Include(m => m.SystemType)
                .Include(m => m.SystemCategory)
                .Include(m => m.AcademicProgram)
                .Include(m => m.Term)
                .Include(m => m.Authors)
                .AsQueryable();

            // Apply search and filter conditions
            if (!string.IsNullOrEmpty(searchString) || systemTypeId.HasValue || systemCategoryId.HasValue || termId.HasValue || programId.HasValue)
            {


                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    var lowerSearch = searchString.ToLower();

                    switch (searchBy)
                    {
                        case "Keywords":
                            manuscripts = manuscripts.Where(m =>
                                m.ManuscriptKeywords.Any(k => k.Keyword.KeywordName.ToLower().Contains(lowerSearch)));
                            break;

                        case "Title":
                            manuscripts = manuscripts.Where(m =>
                                m.Title.ToLower().Contains(lowerSearch));
                            break;

                        case "Authors":
                            manuscripts = manuscripts.Where(m =>
                                m.Authors.Any(a =>
                                    a.FirstName.ToLower().Contains(lowerSearch) ||
                                    a.LastName.ToLower().Contains(lowerSearch)));
                            break;
                    }
                }


                if (systemTypeId.HasValue)
                {
                    manuscripts = manuscripts.Where(m => m.SystemTypeID == systemTypeId.Value);
                }

                if (systemCategoryId.HasValue)
                {
                    manuscripts = manuscripts.Where(m => m.SystemCategoryID == systemCategoryId.Value);
                }

                if (termId.HasValue)
                {
                    manuscripts = manuscripts.Where(m => m.TermID == termId.Value);
                }

                if (programId.HasValue)
                {
                    manuscripts = manuscripts.Where(m => m.ProgramID == programId.Value);
                }
            }
            else
            {
                // If no filters are applied, return an empty list to prevent overloading
                return View(new List<Manuscript>());
            }

            // Count the total number of items after filtering
            int totalItems = await manuscripts.CountAsync();

            // Calculate the current page number (default to 1 if not provided)
            pageNumber ??= 1;

            // Paginate the results
            var paginatedManuscripts = await manuscripts
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();

            // Set ViewBag properties for pagination
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize.Value);
            ViewBag.HasPreviousPage = pageNumber > 1;
            ViewBag.HasNextPage = pageNumber < ViewBag.TotalPages;

            // Return paginated manuscripts to the view
            return View(paginatedManuscripts);
        }

        //||////////////////////////////////////////////////////////////////////////////////START OF UPLOAD AREA/////////////////////////////////////////////////////////////||

        [HttpGet]
        public IActionResult Create()
        {
            var systemTypes = _context.SystemTypes.ToList();
            var systemCategories = _context.SystemCategories.ToList();

            // Convert to SelectListItems

            var systemTypesSelectList = systemTypes.Select(st => new SelectListItem
            {
                Value = st.SystemTypeID.ToString(),
                Text = st.SystemTypeName
            }).ToList();

            var systemCategoriesSelectList = systemCategories.Select(sc => new SelectListItem
            {
                Value = sc.SystemCategoryID.ToString(),
                Text = sc.SystemCategoryName
            }).ToList();

            // Store descriptions in ViewBag as dictionaries

            ViewBag.SystemTypeDescriptions = systemTypes.ToDictionary(st => st.SystemTypeID.ToString(), st => st.Description);
            ViewBag.SystemCategoryDescriptions = systemCategories.ToDictionary(sc => sc.SystemCategoryID.ToString(), sc => sc.Description);

            // Clear session if navigating away without submitting
            HttpContext.Session.Remove("Authors");
            HttpContext.Session.Remove("Keywords");

            var viewModel = new ManuscriptUploadViewModel
            {
                SystemTypes = systemTypesSelectList,
                SystemCategories = systemCategoriesSelectList,
                Terms = _context.Terms.Select(t => new SelectListItem
                {
                    Value = t.TermID.ToString(),
                    Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
                }).ToList(),
                AcademicProgram = _context.AcademicProgram.Select(ap => new SelectListItem 
                { 
                    Value = ap.ProgramID.ToString(),
                    Text= ap.ProgramName
                }).ToList()
            };

            return View(viewModel);
        }



        // POST: Manuscripts/Admins Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManuscriptUploadViewModel model)
        {
            var Uploader = HttpContext.Session.GetInt32("UserID") ?? 0;
            var uploaderRole = HttpContext.Session.GetString("UserRole") ?? "";
            var authors = HttpContext.Session.Get<List<AuthorViewModel>>("Authors");
            var keywords = HttpContext.Session.Get<List<string>>("Keywords");
            var academicProgramsCCS = new[] { 1, 2, 3, 4, 5 };

            //if (academicProgramsCCS.Contains(model.ProgramID) && model.SystemTypeID == null || model.SystemCategoryID == null)
            //{

                //    var viewModel = new ManuscriptUploadViewModel
                //    {
                //        ////Title = model.Title,
                //        Adviser = model.Adviser,
                //        SystemTypeID = model.SystemTypeID,
                //        SystemCategoryID = model.SystemCategoryID,
                //        TermID = model.TermID,
                //        ProgramID = model.ProgramID,
                //        SystemTypes = (IEnumerable<SelectListItem>)_context.SystemTypes.ToList(),
                //        SystemCategories = (IEnumerable<SelectListItem>)_context.SystemCategories.ToList(),
                //        Terms = _context.Terms.Select(t => new SelectListItem
                //        {
                //            Value = t.TermID.ToString(),
                //            Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
                //        }).ToList(),
                //        AcademicProgram = _context.AcademicProgram.Select(ap => new SelectListItem
                //        {
                //            Value = ap.ProgramID.ToString(),
                //            Text = ap.ProgramName
                //        }).ToList(),
                //        Authors = authors ?? new List<AuthorViewModel>(),
                //        Keywords = keywords ?? new List<string>()
                //    };

                //    return View(viewModel);
                //}

                ////Manual Restriction for MODELSTATEVALID Manuscript submission--- 
            if (model.TermID == 0 || model.ProgramID == 0 || authors == null)
            {

                var viewModel = new ManuscriptUploadViewModel
                {
                    ////Title = model.Title,
                    Adviser = model.Adviser,
                    SystemTypeID = model.SystemTypeID,
                    SystemCategoryID = model.SystemCategoryID,
                    TermID = model.TermID,
                    ProgramID = model.ProgramID,
                    SystemTypes = (IEnumerable<SelectListItem>)_context.SystemTypes.ToList(),
                    SystemCategories = (IEnumerable<SelectListItem>)_context.SystemCategories.ToList(),
                    Terms = _context.Terms.Select(t => new SelectListItem
                    {
                        Value = t.TermID.ToString(),
                        Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
                    }).ToList(),
                    AcademicProgram = _context.AcademicProgram.Select(ap => new SelectListItem
                    {
                        Value = ap.ProgramID.ToString(),
                        Text = ap.ProgramName
                    }).ToList(),
                    Authors = authors ?? new List<AuthorViewModel>(),
                    Keywords = keywords ?? new List<string>()
                };

                return View(viewModel);
            }

            // Save uploaded Manuscript and abstract file ---

            var fileName = $"{Path.GetFileNameWithoutExtension(model.ManusFile.FileName)}_{Guid.NewGuid()}{Path.GetExtension(model.ManusFile.FileName)}";
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "ManusUpFiles", fileName);

            if (model.ManusFile == null|| Path.GetExtension(model.ManusFile.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("ManusFile", "Only PDF files are allowed.");
                var viewModel = new ManuscriptUploadViewModel
                {
                    Title = model.Title,
                    Adviser = model.Adviser,
                    SystemTypeID = model.SystemTypeID,
                    SystemCategoryID = model.SystemCategoryID,
                    TermID = model.TermID,
                    ProgramID = model.ProgramID,
                    SystemTypes = (IEnumerable<SelectListItem>)_context.SystemTypes.ToList(),
                    SystemCategories = (IEnumerable<SelectListItem>)_context.SystemCategories.ToList(),
                    Terms = _context.Terms.Select(t => new SelectListItem
                    {
                        Value = t.TermID.ToString(),
                        Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
                    }).ToList(),

                    AcademicProgram = _context.AcademicProgram.Select(ap => new SelectListItem
                    {
                        Value = ap.ProgramID.ToString(),
                        Text = ap.ProgramName
                    }).ToList(),
                    Authors = authors ?? new List<AuthorViewModel>(),
                    Keywords = keywords ?? new List<string>()
                };
                return View(viewModel);
            }

        //Saving the ManusFile and AbstractFile if valid
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ManusFile.CopyToAsync(stream);
            }
            var manusFile = new ManusFile { FileName = fileName, FilePath = filePath };
            _context.ManusFiles.Add(manusFile);
            await _context.SaveChangesAsync();


            // FINAL SUBMISSION TO DATABASE
            var manuscript = new Manuscript
            {
                Title = model.Title.ToUpper(),
                Adviser = model.Adviser,
                SystemTypeID = model.SystemTypeID,
                SystemCategoryID = model.SystemCategoryID,
                PublicationDate = DateTime.Now,
                TermID = model.TermID,
                ProgramID = model.ProgramID,
                ManusFileID = manusFile.ManusFileID,
                UserID = Uploader,
                IsPublished = uploaderRole == "Staff" ? false : true
            };

            _context.Manuscripts.Add(manuscript);
            await _context.SaveChangesAsync();

            // Save authors from session
            if (authors != null)
            {
                foreach (var author in authors)
                {
                    var formattedFirstName = char.ToUpper(author.FirstName[0]) + author.FirstName.Substring(1).ToLower();
                    var formattedLastName = char.ToUpper(author.LastName[0]) + author.LastName.Substring(1).ToLower();

                    var newAuthor = new Author
                    {
                        FirstName = formattedFirstName,
                        LastName = formattedLastName,
                        ManuscriptID = manuscript.ManuscriptID // Link author to manuscript directly
                    };
                    _context.Authors.Add(newAuthor);

                    await _context.SaveChangesAsync();
                }
            }

            // Save keywords from session--
            if (keywords != null)
            {
                foreach (var keyword in keywords)
                {
                    var formattedKeyword = char.ToUpper(keyword[0]) + keyword.Substring(1).ToLower();

                    var existingKeyword = await _context.Keywords.FirstOrDefaultAsync(k => k.KeywordName == formattedKeyword)
                                          ?? new Keyword { KeywordName = formattedKeyword };

                    if (existingKeyword.KeywordID == 0) _context.Keywords.Add(existingKeyword);
                    await _context.SaveChangesAsync();

                    _context.ManuscriptKeywords.Add(new ManuscriptKeyword { ManuscriptID = manuscript.ManuscriptID, KeywordID = existingKeyword.KeywordID });
                    await _context.SaveChangesAsync();
                }
            }

            // Clear session
            HttpContext.Session.Remove("Authors");
            HttpContext.Session.Remove("Keywords");

            return RedirectToAction(nameof(Index));
        }



        //||//////////////////////////////////////////////////////////////////END OF MANUSCRIPTS UPLOAD AREA////////////////////////////////////////////////////////////////////////||

        //UPDATE MANUSCRIPT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var manuscript = await _context.Manuscripts
                .Include(m => m.ManusFile)
                .Include(m => m.Authors) // Include Authors
                .Include(m => m.ManuscriptKeywords)
                .ThenInclude(mk => mk.Keyword)
                .FirstOrDefaultAsync(m => m.ManuscriptID == id);

            if (manuscript == null) return NotFound();

            // Populate ViewModel
            var viewModel = new ManuscriptEditViewModel
            {
                ManuscriptID = manuscript.ManuscriptID,
                Title = manuscript.Title,
                Adviser = manuscript.Adviser,
                SystemTypeID = manuscript.SystemTypeID ?? 0,
                SystemCategoryID = manuscript.SystemCategoryID ?? 0,
                ProgramID = manuscript.ProgramID,
                TermID = manuscript.TermID,
                ManusFile = manuscript.ManusFile,
                Authors = manuscript.Authors.Select(a => new AuthorViewModel
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                Keywords = manuscript.ManuscriptKeywords.Select(mk => mk.Keyword.KeywordName).ToList(),

                // Dropdown lists
                SystemTypes = _context.SystemTypes.Select(st => new SelectListItem
                {
                    Value = st.SystemTypeID.ToString(),
                    Text = st.SystemTypeName
                }).ToList(),
                SystemCategories = _context.SystemCategories.Select(sc => new SelectListItem
                {
                    Value = sc.SystemCategoryID.ToString(),
                    Text = sc.SystemCategoryName
                }).ToList(),
                AcademicProgram = _context.AcademicProgram.Select(ap => new SelectListItem
                {
                    Value = ap.ProgramID.ToString(),
                    Text = ap.ProgramName
                }).ToList(),
                Terms = _context.Terms.Select(t => new SelectListItem
                {
                    Value = t.TermID.ToString(),
                    Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
                }).ToList()
            };

            // Populate session for authors and keywords
            HttpContext.Session.Set("Authors", viewModel.Authors);
            HttpContext.Session.Set("Keywords", viewModel.Keywords);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ManuscriptEditViewModel model)
        {
            // Retrieve authors and keywords from session
            var authors = HttpContext.Session.Get<List<AuthorViewModel>>("Authors") ?? new List<AuthorViewModel>();
            var keywords = HttpContext.Session.Get<List<string>>("Keywords") ?? new List<string>();

            // Validate inputs and reload dropdowns if invalid
            if (model.SystemTypeID == 0 || model.SystemCategoryID == 0 || model.TermID == 0 ||
                model.ProgramID == 0 || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Adviser))
            {
                // Reload dropdowns
                model.SystemTypes = _context.SystemTypes.Select(st => new SelectListItem
                {
                    Value = st.SystemTypeID.ToString(),
                    Text = st.SystemTypeName
                }).ToList();

                model.SystemCategories = _context.SystemCategories.Select(sc => new SelectListItem
                {
                    Value = sc.SystemCategoryID.ToString(),
                    Text = sc.SystemCategoryName
                }).ToList();

                model.Terms = _context.Terms.Select(t => new SelectListItem
                {
                    Value = t.TermID.ToString(),
                    Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
                }).ToList();

                model.AcademicProgram = _context.AcademicProgram.Select(ap => new SelectListItem
                {
                    Value = ap.ProgramID.ToString(),
                    Text = ap.ProgramName
                }).ToList();

                // Preserve current authors and keywords
                model.Authors = authors;
                model.Keywords = keywords;

                // Retain current file information
                var manuscript = await _context.Manuscripts
                    .Include(m => m.ManusFile)
                    //.Include(m => m.AbstractFile)
                    .FirstOrDefaultAsync(m => m.ManuscriptID == model.ManuscriptID);

                if (manuscript != null)
                {
                    model.ManusFile = manuscript.ManusFile;
                    //model.AbstractFile = manuscript.AbstractFile;
                }

                return View(model);
            }

            // Fetch the manuscript to update
            var manuscriptToUpdate = await _context.Manuscripts
                .Include(m => m.ManusFile)
                //.Include(m => m.AbstractFile)
                .Include(m => m.Authors)
                .Include(m => m.ManuscriptKeywords)
                .ThenInclude(mk => mk.Keyword)
                .FirstOrDefaultAsync(m => m.ManuscriptID == model.ManuscriptID);

            if (manuscriptToUpdate == null)
            {
                return NotFound();
            }

            // Update basic fields
            manuscriptToUpdate.Title = model.Title;
            manuscriptToUpdate.Adviser = model.Adviser;
            manuscriptToUpdate.SystemTypeID = model.SystemTypeID;
            manuscriptToUpdate.SystemCategoryID = model.SystemCategoryID;
            manuscriptToUpdate.ProgramID = model.ProgramID;
            manuscriptToUpdate.TermID = model.TermID;

            // **Handle ManusFile replacement or retention**
            if (model.NewManusFile != null)
            {
                var manusFilesDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "ManusUpFiles");

                // Ensure the directory exists
                if (!Directory.Exists(manusFilesDirectory))
                {
                    Directory.CreateDirectory(manusFilesDirectory);
                }

                // Delete the old file if it exists
                if (!string.IsNullOrEmpty(manuscriptToUpdate.ManusFile?.FilePath) &&
                    System.IO.File.Exists(manuscriptToUpdate.ManusFile.FilePath))
                {
                    System.IO.File.Delete(manuscriptToUpdate.ManusFile.FilePath);
                }

                // Save the new ManusFile
                var newFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.NewManusFile.FileName)}";
                var newFilePath = Path.Combine(manusFilesDirectory, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await model.NewManusFile.CopyToAsync(stream);
                }

                manuscriptToUpdate.ManusFile.FileName = newFileName;
                manuscriptToUpdate.ManusFile.FilePath = newFilePath;
            }

            // Retain current ManusFile if no new file is provided
            else if (model.NewManusFile == null)
            {
                manuscriptToUpdate.ManusFile = manuscriptToUpdate.ManusFile;
            }


            // Update authors
            if (authors.Any())
            {
                // Get the existing authors from the database
                var existingAuthors = manuscriptToUpdate.Authors.ToList();

                // Add or update authors
                foreach (var submittedAuthor in authors)
                {
                    // Check if the author already exists
                    var existingAuthor = existingAuthors
                        .FirstOrDefault(a => a.FirstName.Equals(submittedAuthor.FirstName, StringComparison.OrdinalIgnoreCase)
                                          && a.LastName.Equals(submittedAuthor.LastName, StringComparison.OrdinalIgnoreCase));

                    if (existingAuthor == null)
                    {
                        // Add new author if it doesn't exist
                        manuscriptToUpdate.Authors.Add(new Author
                        {
                            FirstName = submittedAuthor.FirstName,
                            LastName = submittedAuthor.LastName,
                            ManuscriptID = manuscriptToUpdate.ManuscriptID
                        });
                    }
                }

                // Remove authors that are no longer in the submitted list
                foreach (var existingAuthor in existingAuthors)
                {
                    if (!authors.Any(a => a.FirstName.Equals(existingAuthor.FirstName, StringComparison.OrdinalIgnoreCase)
                                       && a.LastName.Equals(existingAuthor.LastName, StringComparison.OrdinalIgnoreCase)))
                    {
                        _context.Authors.Remove(existingAuthor);
                    }
                }
            }


            // Update keywords
            if (keywords.Any())
            {
                // Load existing keywords linked to the manuscript
                var existingKeywords = manuscriptToUpdate.ManuscriptKeywords.ToList();

                // Iterate over the keywords from the session
                foreach (var keyword in keywords)
                {
                    // Case-insensitive check for the keyword in the database
                    var existingKeyword = await _context.Keywords
                        .AsNoTracking()
                        .FirstOrDefaultAsync(k => k.KeywordName.ToLower() == keyword.ToLower());

                    // If the keyword doesn't exist in the database, add it
                    if (existingKeyword == null)
                    {
                        existingKeyword = new Keyword { KeywordName = keyword };
                        _context.Keywords.Add(existingKeyword);
                        await _context.SaveChangesAsync(); // Save to generate the KeywordID
                    }

                    // If the keyword is not linked to the manuscript, add the association
                    if (!existingKeywords.Any(ek => ek.Keyword.KeywordName.Equals(keyword, StringComparison.OrdinalIgnoreCase)))
                    {
                        manuscriptToUpdate.ManuscriptKeywords.Add(new ManuscriptKeyword
                        {
                            ManuscriptID = manuscriptToUpdate.ManuscriptID,
                            KeywordID = existingKeyword.KeywordID
                        });
                    }
                }

                // Remove keywords that are no longer in the updated list
                foreach (var existingKeyword in existingKeywords)
                {
                    if (!keywords.Any(k => k.Equals(existingKeyword.Keyword.KeywordName, StringComparison.OrdinalIgnoreCase)))
                    {
                        _context.ManuscriptKeywords.Remove(existingKeyword);
                    }
                }
            }


            // Save changes to the database
            await _context.SaveChangesAsync();

            // Clear session data
            HttpContext.Session.Remove("Authors");
            HttpContext.Session.Remove("Keywords");

            return RedirectToAction(nameof(Index));
        }



        // GET: Autocomplete GetKeywords Method - FOR AUTOCOMPLETETEXTBOX OF KEYWORDS
        [HttpGet]
        public async Task<IActionResult> GetKeywords(string term)
        {
            var keywords = await _context.Keywords
                .Where(k => k.KeywordName.Contains(term))
                .Select(k => k.KeywordName)
                .ToListAsync();

            return Json(keywords);
        }

        public async Task<IActionResult> ViewDocument(int id)
        {
            var manuscript = await _context.Manuscripts
                .Include(m => m.ManusFile)
                .FirstOrDefaultAsync(m => m.ManuscriptID == id);

            if (manuscript == null)
            {
                return NotFound();
            }

            // Ensure that FileName includes the full path including extension
            var filePath = $"/ManusUpFiles/{manuscript.ManusFile.FileName}";
            ViewData["FilePath"] = filePath;

            return View("ViewDocument", manuscript);
        }


        // AJAX: Add Author to session
        [HttpPost]
        public IActionResult AddAuthor(string firstName, string lastName)
        {
            var authors = HttpContext.Session.Get<List<AuthorViewModel>>("Authors") ?? new List<AuthorViewModel>();
            var newAuthor = new AuthorViewModel { FirstName = firstName, LastName = lastName };
            authors.Add(newAuthor);
            HttpContext.Session.Set("Authors", authors);
            return Json(newAuthor);
        }

        // AJAX: Remove Author from session
        [HttpPost]
        public IActionResult RemoveAuthor(string firstName, string lastName)
        {
            // Retrieve the list of authors from the session
            var authors = HttpContext.Session.Get<List<AuthorViewModel>>("Authors") ?? new List<AuthorViewModel>();

            // Find the author to remove (case-insensitive comparison for robustness)
            var authorToRemove = authors.FirstOrDefault(a =>
                string.Equals(a.FirstName, firstName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(a.LastName, lastName, StringComparison.OrdinalIgnoreCase));

            if (authorToRemove != null)
            {
                authors.Remove(authorToRemove); // Remove the author
                HttpContext.Session.Set("Authors", authors); // Update the session with the modified list
                return Json(new { success = true, message = "Author removed successfully." });
            }

            return Json(new { success = false, message = "Author not found." }); // Handle case where author isn't found
        }


        // AJAX: Add Author to session
        [HttpPost]
        public IActionResult AddKeyword(string keyword)
        {
            var keywords = HttpContext.Session.Get<List<string>>("Keywords") ?? new List<string>();
            if (!keywords.Contains(keyword))
            {
                keywords.Add(keyword);
                HttpContext.Session.Set("Keywords", keywords);
            }
            return Json(new { keyword });
        }


        [HttpPost]
        public IActionResult RemoveKeyword(string keyword)
        {
            var keywords = HttpContext.Session.Get<List<string>>("Keywords") ?? new List<string>();

            // Remove the keyword
            keywords.RemoveAll(k => k.Equals(keyword, StringComparison.OrdinalIgnoreCase));

            // Update the session
            HttpContext.Session.Set("Keywords", keywords);

            return Json(new { success = true });
        }

        //=============================================================================================OTHER CRUD OPERATIONS=======================================================||

        // GET: Manuscripts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Manuscripts == null)
            {
                return NotFound();
            }

            var manuscript = await _context.Manuscripts
                .Include(m => m.ManusFile)
                .Include(m => m.SystemCategory)
                .Include(m => m.SystemType)
                .Include(m => m.Term)            
                .Include(m => m.Authors) // Include Authors collection
                .Include(m => m.ManuscriptKeywords) // Include ManuscriptKeywords collection
                    .ThenInclude(mk => mk.Keyword) // Then include the associated Keywords
                .FirstOrDefaultAsync(m => m.ManuscriptID == id);

            if (manuscript == null)
            {
                return NotFound();
            }

            return PartialView("Details", manuscript);
        }      

        private bool ManuscriptExists(int id)
        {
          return (_context.Manuscripts?.Any(e => e.ManuscriptID == id)).GetValueOrDefault();
        }
    }

}
