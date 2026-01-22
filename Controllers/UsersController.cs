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
using Thesis_Capstone_Archive.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Thesis_Capstone_Archive.Controllers
{
    public class UsersController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;
        private readonly PasswordService _passwordService;

        public UsersController(Thesis_Capstone_ArchiveContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            
            return _context.User != null ?
                View(await _context.User.ToListAsync()) :
                Problem("Entity set 'Thesis_Capstone_ArchiveContext.User' is null.");
        }

      

     

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }



        // GET: Users/Create
        public IActionResult Create()
        {
            return PartialView("Create", new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            var existingUser = await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IDNumber == user.IDNumber);

            if (existingUser != null)
            {
                ModelState.AddModelError("IDNumber", "A user with this ID Number already exists.");
                return PartialView("Create", user);
            }

          
            user.PasswordHash = _passwordService.HashPassword(user.IDNumber);
            user.IsDefaultPassword = true;

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true }); // AJAX will detect this and reload
        }


        [HttpGet]
        public JsonResult GetLoggedInUserInfo()
        {
            var username = HttpContext.Session.GetString("Username");
            var firstName = HttpContext.Session.GetString("FirstName");
            var lastName = HttpContext.Session.GetString("LastName");
   

            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "No user is logged in." });
            }

            return Json(new { success = true, username, firstName, lastName });
        }

       

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.User.FindAsync(id);
            if (user == null)
                return NotFound();

            var userEditViewModel = new UserEditViewModel
            {
                UserID = user.UserID,
                IDNumber = user.IDNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };

            return PartialView("Edit", userEditViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel userEditViewModel)
        {
            if (id != userEditViewModel.UserID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.User.FindAsync(id);
                    if (existingUser == null)
                        return NotFound();

                    // Update only the allowed fields
                    existingUser.IDNumber = userEditViewModel.IDNumber;
                    existingUser.FirstName = userEditViewModel.FirstName;
                    existingUser.LastName = userEditViewModel.LastName;
                    existingUser.Role = userEditViewModel.Role;

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userEditViewModel.UserID))
                        return NotFound();
                    else
                        throw;
                }
                return Json(new { success = true });
            }
            return PartialView("Edit", userEditViewModel);
        }


        // POST: Users/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(); // Handle invalid user ID
            }

            // Reset the password to the username
            user.PasswordHash = _passwordService.HashPassword(user.IDNumber);
            user.IsDefaultPassword = true; // Flag as default password
            _context.User.Update(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Password for {user.IDNumber} has been reset to the default (username).";
            return RedirectToAction(nameof(Index));
        }



        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Thesis_Capstone_ArchiveContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
