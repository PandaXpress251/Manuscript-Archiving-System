using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thesis_Capstone_Archive.Data;
using Thesis_Capstone_Archive.Helpers;
using Thesis_Capstone_Archive.Models.ViewModels;

namespace Thesis_Capstone_Archive.Controllers
{
    public class AccountController : Controller
    {
      
       readonly private Thesis_Capstone_ArchiveContext _context;
       readonly private PasswordService _passwordService;

        public AccountController(Thesis_Capstone_ArchiveContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<IActionResult> Index()
        {

            return _context.User != null ?
                View(await _context.User.ToListAsync()) :
                Problem("Entity set 'Thesis_Capstone_ArchiveContext.User' is null.");
        }

        //Login Method
        [HttpPost]
        public JsonResult Login(LoginViewModel model)
        {
            // Validate the input model
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return Json(new { success = false, message = "Username and password are required." });
            }

            // Find the user by username
            var user = _context.User.FirstOrDefault(u => u.IDNumber == model.Username);

            if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, model.Password))
            {
                return Json(new { success = false, message = "Invalid username or password." });
            }

            // Check if the user needs to change the default password
            if (user.IsDefaultPassword)
            {
                // Store necessary session information
                HttpContext.Session.SetString("UserID", user.UserID.ToString());
                return Json(new { success = true, requiresPasswordChange = true });
            }

            // Store user details in the session on successful login
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Username", user.IDNumber);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("LastName", user.LastName);
            HttpContext.Session.SetString("Role", user.Role);

            // Return a success response with redirect URL
            return Json(new { success = true, message = "Successfully logged In!" ,redirectUrl = Url.Action("Dashboard", "Home") });
        }



        //Change Password Method
        [HttpPost]
        [Route("Account/ChangePassword")]
        public IActionResult ChangePassword(string newPassword, string confirmPassword)
        {

            //Validation if the two Input fields are null or empty
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return Json(new { success = false, message = "Both fields are required." });
            }

            // Password strength validation (example: at least 8 characters, one special char, etc.)
            //if (newPassword.Length < 8 || !newPassword.Any(char.IsDigit) || !newPassword.Any(char.IsLetter) || !newPassword.Any(ch => !char.IsLetterOrDigit(ch)))
            //{
            //    return Json(new { success = false, message = "Password must be at least 8 characters long and contain a number, a letter, and a special character." });
            //}

            //Validation if the new password is equal to the confirmed password
            if (newPassword != confirmPassword)
            {
                return Json(new { success = false, message = "Passwords do not match." });
            }

            //Validation for illegally accessing change password key entry
            var userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            //Also Validates the illegally accessing change password key entry 2
            int userId = int.Parse(userIdString);
            var user = _context.User.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }


            user.PasswordHash = _passwordService.HashPassword(newPassword);
            user.IsDefaultPassword = false;
            _context.User.Update(user);
            _context.SaveChanges();

            return Json(new { success = true, message = "Password successfully changed." });
        }

        //Logout Method
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to the Login page
            return RedirectToAction("Index", "Home");
        }

    }
}
