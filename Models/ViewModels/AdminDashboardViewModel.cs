using Microsoft.AspNetCore.Mvc.Rendering;

namespace Thesis_Capstone_Archive.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int? SelectedTermID { get; set; }
        public int SelectedSemesterManuscriptCount { get; set; }
        public int TotalManuscriptCount { get; set; }

        public int TotalUploadersCount { get; set; }

        public List<string> MostViewedTitles { get; set; } = new List<string>();
        public List<int> MostViewedCounts { get; set; } = new List<int>();

        public List<SelectListItem> Terms { get; set; } = new List<SelectListItem>();

       
    }
}
