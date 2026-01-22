using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Thesis_Capstone_Archive.Data;
using Thesis_Capstone_Archive.Helpers;
using Thesis_Capstone_Archive.Models;

namespace Thesis_Capstone_Archive.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class KeywordsController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;

        public KeywordsController(Thesis_Capstone_ArchiveContext context)
        {
            _context = context;
        }

        // GET: Keywords
        public async Task<IActionResult> Index()
        {
              return _context.Keywords != null ? 
                          View(await _context.Keywords.ToListAsync()) :
                          Problem("Entity set 'Thesis_Capstone_ArchiveContext.Keyword'  is null.");
        }

        // GET: Keywords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Keywords == null)
            {
                return NotFound();
            }

            var keyword = await _context.Keywords
                .FirstOrDefaultAsync(m => m.KeywordID == id);
            if (keyword == null)
            {
                return NotFound();
            }

            return View(keyword);
        }

        // GET: Keywords/Create
        public IActionResult Create()
        {
            return PartialView("Create", new Keyword());
        }

        // POST: Keywords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KeywordID,KeywordName")] Keyword keyword)
        {
            if (_context.Keywords.Any(st => st.KeywordName.ToLower() == keyword.KeywordName.ToLower()))
            {
                ModelState.AddModelError("Keyword", "Academic Year already exists.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(keyword);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("Create", keyword);
        }

        // GET: Keywords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Keywords == null)
            {
                return NotFound();
            }

            var keyword = await _context.Keywords.FindAsync(id);
            if (keyword == null)
            {
                return NotFound();
            }
            return PartialView("Edit", keyword);
        }

        // POST: Keywords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KeywordID,KeywordName")] Keyword keyword)
        {
            if (id != keyword.KeywordID)
            {
                return NotFound();
            }

            if (_context.Keywords.Any(st => st.KeywordName.ToLower() == keyword.KeywordName.ToLower() &&  st.KeywordID != id))
            {
                ModelState.AddModelError("", "Keywords already exists. Please choose a different keyword.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(keyword);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeywordExists(keyword.KeywordID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
             
            }
            return PartialView("Edit", keyword);

        }

        // GET: Keywords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Keywords == null)
            {
                return NotFound();
            }

            var keyword = await _context.Keywords
                .FirstOrDefaultAsync(m => m.KeywordID == id);
            if (keyword == null)
            {
                return NotFound();
            }

            return View(keyword);
        }

        // POST: Keywords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Keywords == null)
            {
                return Problem("Entity set 'Thesis_Capstone_ArchiveContext.Keyword'  is null.");
            }
            var keyword = await _context.Keywords.FindAsync(id);
            if (keyword != null)
            {
                _context.Keywords.Remove(keyword);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeywordExists(int id)
        {
          return (_context.Keywords?.Any(e => e.KeywordID == id)).GetValueOrDefault();
        }
    }
}
