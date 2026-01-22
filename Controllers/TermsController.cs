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
    public class TermsController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;

        public TermsController(Thesis_Capstone_ArchiveContext context)
        {
            _context = context;
        }

        // GET: Terms
        public async Task<IActionResult> Index()
        {
              return _context.Terms != null ? 
                          View(await _context.Terms.ToListAsync()) :
                          Problem("Entity set 'Thesis_Capstone_ArchiveContext.Term'  is null.");
        }

        // GET: Terms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Terms == null)
            {
                return NotFound();
            }

            var term = await _context.Terms
                .FirstOrDefaultAsync(m => m.TermID == id);
            if (term == null)
            {
                return NotFound();
            }

            return View(term);
        }

        // GET: Terms/Create
        public IActionResult Create()
        {
            return PartialView("Create", new Term());
        }

        // POST: Terms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TermID,TermName,StartYear,EndYear")] Term term)
        {
            if (_context.Terms.Any(e => e.StartYear.Year == term.StartYear.Year && e.EndYear == term.EndYear))
            {
                ModelState.AddModelError("", "Academic Year already exists or Invalid.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(term);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
    
            return PartialView("Create", term);
        }

        // GET: Terms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Terms == null)
            {
                return NotFound();
            }

            var term = await _context.Terms.FindAsync(id);
            if (term == null)
            {
                return NotFound();
            }
            return PartialView("Edit", term);
        }

        // POST: Terms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TermID,TermName,StartYear,EndYear")] Term term)
        {
            if (id != term.TermID)
            {
                return NotFound();
            }

            if (_context.Terms.Any(st => st.StartYear > term.EndYear && term.TermID !=id))
            {
                ModelState.AddModelError("", "Academic Year already exists or Invalid.");
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(term);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TermExists(term.TermID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View("Edit", term);
        }




        // GET: Terms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Terms == null)
            {
                return NotFound();
            }

            var term = await _context.Terms
                .FirstOrDefaultAsync(m => m.TermID == id);
            if (term == null)
            {
                return NotFound();
            }

            return View(term);
        }

        // POST: Terms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Terms == null)
            {
                return Problem("Entity set 'Thesis_Capstone_ArchiveContext.Term'  is null.");
            }
            var term = await _context.Terms.FindAsync(id);
            if (term != null)
            {
                _context.Terms.Remove(term);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TermExists(int id)
        {
          return (_context.Terms?.Any(e => e.TermID == id)).GetValueOrDefault();
        }
    }
}
