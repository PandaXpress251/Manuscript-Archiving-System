using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Thesis_Capstone_Archive.Data;
using Thesis_Capstone_Archive.Helpers;
using Thesis_Capstone_Archive.Models;
using Thesis_Capstone_Archive.Models.ViewModels;

namespace Thesis_Capstone_Archive.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class AdminController : Controller
    {
        private readonly Thesis_Capstone_ArchiveContext _context;

        public AdminController(Thesis_Capstone_ArchiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Reports(int? SelectedTermID, DateTime? FromDate, DateTime? ToDate)
        {
            ModelState.Clear(); // Clear ModelState to prevent interference

            var viewModel = new AdminDashboardViewModel
            {
                SelectedTermID = SelectedTermID,
                Terms = await _context.Terms.Select(t => new SelectListItem
                {
                    Value = t.TermID.ToString(),
                    Text = $"{t.TermName} ({t.StartYear:MMMM yyyy} - {t.EndYear:MMMM yyyy})"
                }).ToListAsync(),
                TotalManuscriptCount = await _context.Manuscripts.CountAsync(), 

                //TotalUploadersCount = await _context.Users.Where(u => u.Role == UserRole.Uploader).CountAsync()

            };

            

            // Calculate manuscript count for selected term
            if (SelectedTermID.HasValue)
            {
                viewModel.SelectedSemesterManuscriptCount = await _context.Manuscripts
                    .Where(m => m.TermID == SelectedTermID.Value)
                    .CountAsync();
            }

            // Fetch data for most-viewed manuscripts
            var mostViewed = await _context.Manuscripts
                .OrderByDescending(m => m.ViewCount)
                .Take(10)
                .Select(m => new { m.Title, m.ViewCount })
                .ToListAsync();
            viewModel.MostViewedTitles = mostViewed.Select(m => m.Title).ToList();
            viewModel.MostViewedCounts = mostViewed.Select(m => m.ViewCount).ToList();

            
         

            return View(viewModel);
        }

    }

}
