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
    public class SystemTypesController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;

        public SystemTypesController(Thesis_Capstone_ArchiveContext context)
        {
            _context = context;
        }

        // GET: SystemTypes
        public async Task<IActionResult> Index()
        {
            return _context.SystemTypes != null ?
                        View(await _context.SystemTypes.ToListAsync()) :
                        Problem("Entity set 'Thesis_Capstone_ArchiveContext.SystemTypes' is null.");
        }

        // GET: SystemTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SystemTypes == null)
            {
                return NotFound();
            }

            var systemType = await _context.SystemTypes
                .FirstOrDefaultAsync(m => m.SystemTypeID == id);
            if (systemType == null)
            {
                return NotFound();
            }

            return PartialView("Details",systemType);
        }

        // GET: SystemTypes/Create
        public IActionResult Create()
        {
            return PartialView("Create", new SystemType());
        }

        // POST: SystemTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SystemTypeID,SystemTypeName,Description")] SystemType systemType)
        {
            if (_context.SystemTypes.Any(st => st.SystemTypeName.ToLower() == systemType.SystemTypeName.ToLower()))
            {
                ModelState.AddModelError("", "Some of the inputs are already existed or Invalid.");

                ViewData["ShowCreateModal"] = true;
            }

            if (ModelState.IsValid)
            {
                systemType.SystemTypeName = systemType.SystemTypeName.ToUpper();

                _context.Add(systemType);
                await _context.SaveChangesAsync();
                return Json(new{success = true});
            }

            return PartialView("Create", systemType);
        }

        // GET: SystemTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SystemTypes == null)
            {
                return NotFound();
            }

            var systemType = await _context.SystemTypes.FindAsync(id);
            if (systemType == null)
            {
                return NotFound();
            }
            return PartialView("Edit", systemType);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SystemTypeID,SystemTypeName,Description")] SystemType systemType)
        {
            if (id != systemType.SystemTypeID)
            {
                return NotFound();
            }

            if (_context.SystemTypes.Any(st => st.SystemTypeName.ToLower() == systemType.SystemTypeName.ToLower() && st.SystemTypeID != id))
            {
                ModelState.AddModelError("", "Some of the Iputs might already exists or Invalid.");
               
            }

            if (ModelState.IsValid)
            {
                try
                {
                    systemType.SystemTypeName = systemType.SystemTypeName.ToUpper();
                 
                    _context.Update(systemType);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemTypeExists(systemType.SystemTypeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return PartialView("Edit", systemType);
        }

        // GET: SystemTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SystemTypes == null)
            {
                return NotFound();
            }

            var systemType = await _context.SystemTypes.FirstOrDefaultAsync(m => m.SystemTypeID == id);
            if (systemType == null)
            {
                return NotFound();
            }

            return PartialView("Delete", systemType);
        }

        // POST: SystemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SystemTypes == null)
            {
                return Problem("Entity set 'Thesis_Capstone_ArchiveContext.SystemTypes' is null.");
            }
            var systemType = await _context.SystemTypes.FindAsync(id);
            if (systemType != null)
            {
                _context.SystemTypes.Remove(systemType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemTypeExists(int id)
        {
            return (_context.SystemTypes?.Any(e => e.SystemTypeID == id)).GetValueOrDefault();
        }
    }
}
