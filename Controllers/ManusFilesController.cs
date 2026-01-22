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
    public class ManusFilesController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;

        public ManusFilesController(Thesis_Capstone_ArchiveContext context)
        {
            _context = context;
        }

        // GET: ManusFiles
        public async Task<IActionResult> Index()
        {
              return _context.ManusFiles != null ? 
                          View(await _context.ManusFiles.ToListAsync()) :
                          Problem("Entity set 'Thesis_Capstone_ArchiveContext.ManusFile'  is null.");
        }

        // GET: ManusFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ManusFiles == null)
            {
                return NotFound();
            }

            var manusFile = await _context.ManusFiles
                .FirstOrDefaultAsync(m => m.ManusFileID == id);
            if (manusFile == null)
            {
                return NotFound();
            }

            return View(manusFile);
        }

        // GET: ManusFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ManusFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ManusFileID,FileName,FilePath")] ManusFile manusFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manusFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manusFile);
        }

        // GET: ManusFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ManusFiles == null)
            {
                return NotFound();
            }

            var manusFile = await _context.ManusFiles.FindAsync(id);
            if (manusFile == null)
            {
                return NotFound();
            }
            return View(manusFile);
        }

        // POST: ManusFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ManusFileID,FileName,FilePath")] ManusFile manusFile)
        {
            if (id != manusFile.ManusFileID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manusFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManusFileExists(manusFile.ManusFileID))
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
            return View(manusFile);
        }

        // GET: ManusFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ManusFiles == null)
            {
                return NotFound();
            }

            var manusFile = await _context.ManusFiles
                .FirstOrDefaultAsync(m => m.ManusFileID == id);
            if (manusFile == null)
            {
                return NotFound();
            }

            return View(manusFile);
        }

        // POST: ManusFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ManusFiles == null)
            {
                return Problem("Entity set 'Thesis_Capstone_ArchiveContext.ManusFile'  is null.");
            }
            var manusFile = await _context.ManusFiles.FindAsync(id);
            if (manusFile != null)
            {
                _context.ManusFiles.Remove(manusFile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManusFileExists(int id)
        {
          return (_context.ManusFiles?.Any(e => e.ManusFileID == id)).GetValueOrDefault();
        }
    }
}
