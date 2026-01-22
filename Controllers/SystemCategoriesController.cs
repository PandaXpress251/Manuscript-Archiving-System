using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Thesis_Capstone_Archive.Data;
using Thesis_Capstone_Archive.Helpers;
using Thesis_Capstone_Archive.Models;

namespace Thesis_Capstone_Archive.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SystemCategoriesController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;

        public SystemCategoriesController(Thesis_Capstone_ArchiveContext context)
        {
            _context = context;
        }

        // GET: SystemCategories
        public async Task<IActionResult> Index()
        {
              return _context.SystemCategories != null ? 
                          View(await _context.SystemCategories.ToListAsync()) :
                          Problem("Entity set 'Thesis_Capstone_ArchiveContext.SystemCategory'  is null.");
        }

        // GET: SystemCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SystemCategories == null)
            {
                return NotFound();
            }

            var systemCategory = await _context.SystemCategories
                .FirstOrDefaultAsync(m => m.SystemCategoryID == id);
            if (systemCategory == null)
            {
                return NotFound();
            }

            return PartialView("Details" ,systemCategory);
        }

        // GET: SystemCategories/Create
        public IActionResult Create()
        {
            return PartialView("Create", new SystemCategory());
        }

        // POST: SystemCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SystemCategoryID,SystemCategoryName,Description")] SystemCategory systemCategory)
        {
            if (_context.SystemCategories.Any(st => st.SystemCategoryName.ToLower() == systemCategory.SystemCategoryName.ToLower()))
            {
                ModelState.AddModelError("", "Some Inputs are already exists or Invalid.");
              
            }

            if (ModelState.IsValid)
            {
                systemCategory.SystemCategoryName = systemCategory.SystemCategoryName.ToUpper();
                _context.Add(systemCategory);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("Create", systemCategory); // Load the Index view with current system types
        }

        // GET: SystemCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SystemCategories == null)
            {
                return NotFound();
            }

            var systemCategory = await _context.SystemCategories.FindAsync(id);
            if (systemCategory == null)
            {
                return NotFound();
            }
            return PartialView("Edit", systemCategory);
        }

        // POST: SystemCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SystemCategoryID,SystemCategoryName,Description")] SystemCategory systemCategory)
        {
            if (id != systemCategory.SystemCategoryID)
            {
                return NotFound();
            }

            // Ensure uniqueness of SystemTypeName excluding the current one
            if (_context.SystemCategories.Any(st => st.SystemCategoryName.ToLower() == systemCategory.SystemCategoryName.ToLower() && st.SystemCategoryID != id))
            {
                ModelState.AddModelError("", "Some Inputs are already exists or invalid.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    systemCategory.SystemCategoryName = systemCategory.SystemCategoryName.ToUpper();

                    _context.Update(systemCategory);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemCategoryExists(systemCategory.SystemCategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShowEditModal"] = true;
            return PartialView("Edit", systemCategory);
        }

        // GET: SystemCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SystemCategories == null)
            {
                return NotFound();
            }

            var systemCategory = await _context.SystemCategories
                .FirstOrDefaultAsync(m => m.SystemCategoryID == id);
            if (systemCategory == null)
            {
                return NotFound();
            }

            return View(systemCategory);
        }

        // POST: SystemCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SystemCategories == null)
            {
                return Problem("Entity set 'Thesis_Capstone_ArchiveContext.SystemCategory'  is null.");
            }
            var systemCategory = await _context.SystemCategories.FindAsync(id);
            if (systemCategory != null)
            {
                _context.SystemCategories.Remove(systemCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemCategoryExists(int id)
        {
          return (_context.SystemCategories?.Any(e => e.SystemCategoryID == id)).GetValueOrDefault();
        }
    }
}
