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
    public class AcademicProgramsController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;

        public AcademicProgramsController(Thesis_Capstone_ArchiveContext context)
        {
            _context = context;
        }

        // GET: AcademicPrograms
        public async Task<IActionResult> Index()
        {
            return _context.AcademicProgram != null ?
                        View(await _context.AcademicProgram.ToListAsync()) :
                        Problem("Entity set 'Thesis_Capstone_ArchiveContext.AcademicProgram' is null.");
        }

        // GET: AcademicPrograms/Create
        public IActionResult Create()
        {
            return PartialView("Create", new AcademicProgram());
        }

        // POST: AcademicPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramID,ProgramName,Acronym")] AcademicProgram academicProgram)
        {
            if (_context.AcademicProgram.Any(ap => ap.ProgramName.ToLower() == academicProgram.ProgramName.ToLower()))
            {
                ModelState.AddModelError("", "Some of the inputs are already existed or Invalid. Please choose a different input.");

                ViewData["ShowCreateModal"] = true;
            }

            if (ModelState.IsValid)
            {
                academicProgram.ProgramName = academicProgram.ProgramName.ToUpper();

                _context.Add(academicProgram);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }


            ViewData["ShowCreateModal"] = true;
            return PartialView("Create", academicProgram);
        }

        // GET: AcademicPrograms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AcademicProgram == null)
            {
                return NotFound();
            }

            var academicProgram = await _context.AcademicProgram.FindAsync(id);
            if (academicProgram == null)
            {
                return NotFound();
            }
            return PartialView("Edit", academicProgram);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProgramID,ProgramName,Acronym")] AcademicProgram academicProgram)
        {
            if (id != academicProgram.ProgramID)
            {
                return NotFound();
            }

            if (_context.AcademicProgram.Any(ap =>
                (ap.ProgramName.ToLower() == academicProgram.ProgramName.ToLower())
                && ap.ProgramID != id))
            {
                ModelState.AddModelError("", "Some of the Iputs might already exists or Invalid.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    academicProgram.ProgramName = academicProgram.ProgramName.ToUpper();

                    _context.Update(academicProgram);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicProgramExists(academicProgram.ProgramID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return PartialView("Edit", academicProgram);
        }


        // GET: AcademicPrograms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AcademicProgram == null)
            {
                return NotFound();
            }

            var academicProgram = await _context.AcademicProgram
                .FirstOrDefaultAsync(m => m.ProgramID == id);
            if (academicProgram == null)
            {
                return NotFound();
            }

            return View(academicProgram);
        }

        // POST: AcademicPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AcademicProgram == null)
            {
                return Problem("Entity set 'Thesis_Capstone_ArchiveContext.AcademicProgram' is null.");
            }
            var academicProgram = await _context.AcademicProgram.FindAsync(id);
            if (academicProgram != null)
            {
                _context.AcademicProgram.Remove(academicProgram);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicProgramExists(int id)
        {
            return (_context.AcademicProgram?.Any(e => e.ProgramID == id)).GetValueOrDefault();
        }
    }
}
