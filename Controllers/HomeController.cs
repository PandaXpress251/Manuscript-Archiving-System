using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Thesis_Capstone_Archive.Data;
using Thesis_Capstone_Archive.Helpers;
using Thesis_Capstone_Archive.Models;
using Thesis_Capstone_Archive.Models.ViewModels;

namespace Thesis_Capstone_Archive.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Thesis_Capstone_ArchiveContext _context;

        public HomeController(Thesis_Capstone_ArchiveContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Manuscripts
        public async Task<IActionResult> Index(string searchBy, string searchString, int? systemTypeId, int? systemCategoryId, 
                                               int? termId, int? programId)
        {
            

            // Store the current filters in ViewBag to maintain state in the dropdowns
            ViewBag.CurrentFilter = searchString;
            ViewBag.SearchBy = searchBy;
            ViewBag.CurrentSystemType = systemTypeId;
            ViewBag.CurrentSystemCategory = systemCategoryId;
            ViewBag.CurrentTerm = termId;
            ViewBag.CurrentProgram = programId;

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
                              .Include(m => m.Authors)
                              .Include(m => m.SystemType)
                              .Include(m => m.SystemCategory)
                              .Include(m => m.AcademicProgram)
                              .Include(m => m.Term)
                              .AsQueryable();

            // Check if any filter or search string is provided
            if (!string.IsNullOrEmpty(searchString) || systemTypeId.HasValue || systemCategoryId.HasValue || termId.HasValue || programId.HasValue)
            {
                // Apply search filter if a search string is provided
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    if (searchBy == "Keywords")
                    {
                        manuscripts = manuscripts.Where(m => m.ManuscriptKeywords.Any(k => k.Keyword.KeywordName.ToLower().Contains(searchString)));
                    }
                    else
                    {
                        manuscripts = manuscripts.Where(m => m.Title.ToLower().Contains(searchString));
                    }
                }

                // Apply each filter only if it has a value
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

            // Return the filtered list to the view
            return View(await manuscripts.ToListAsync());
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

            ViewData["Title"] = manuscript.Title;

            // Ensure that FileName includes the full path including extension
            var filePath = $"/ManusUpFiles/{manuscript.ManusFile.FileName}";
            ViewData["FilePath"] = filePath;

            string sessionKey = $"ViewedManuscript_{id}";
            var lastViewed = HttpContext.Session.Get<DateTime?>(sessionKey);

            if (lastViewed == null || DateTime.Now.Subtract(lastViewed.Value).TotalMinutes > 2)
            {
                manuscript.ViewCount++;
                _context.Manuscripts.Update(manuscript);
                await _context.SaveChangesAsync();

                HttpContext.Session.Set(sessionKey, DateTime.Now);
            }

            return View("ViewDocument", manuscript);
        }



        // GET: Manuscripts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Manuscripts == null)
            {
                return NotFound();
            }

            var home = await _context.Manuscripts
                .Include(m => m.ManusFile)
                .Include(m => m.SystemCategory)
                .Include(m => m.SystemType)
                .Include(m => m.Term)
                .Include(m => m.Authors) // Include Authors collection
                .Include(m => m.ManuscriptKeywords) // Include ManuscriptKeywords collection
                    .ThenInclude(mk => mk.Keyword) // Then include the associated Keywords
                .FirstOrDefaultAsync(m => m.ManuscriptID == id);

            if (home == null)
            {
                return NotFound();
            }

            return PartialView("Details", home);
        }

        private bool ManuscriptExists(int id)
        {
            return (_context.Manuscripts?.Any(e => e.ManuscriptID == id)).GetValueOrDefault();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
