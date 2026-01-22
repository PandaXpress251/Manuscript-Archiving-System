using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Thesis_Capstone_Archive.Helpers;
using Thesis_Capstone_Archive.Models.ViewModels;


namespace Thesis_Capstone_Archive.Models.ViewModels
{
    public class ManuscriptEditViewModel
    {
        public int ManuscriptID { get; set; }
        public string Title { get; set; }
        public string Adviser { get; set; }
        public int? SystemTypeID { get; set; }
        public int? SystemCategoryID { get; set; }
        public int ProgramID { get; set; }
        public int TermID { get; set; }
        public bool IsPublished { get; set; }
        public ManusFile ManusFile { get; set; } // Current Manuscript file

        [AllowedExtensions(new[] { ".pdf" }, ErrorMessage = "Only PDF files are allowed.")]
        public IFormFile NewManusFile { get; set; }
        public List<AuthorViewModel> Authors { get; set; } = new();
        public List<string> Keywords { get; set; } = new();
        public List<SelectListItem>? SystemTypes { get; set; }
        public List<SelectListItem>? SystemCategories { get; set; }
        public List<SelectListItem> AcademicProgram { get; set; }
        public List<SelectListItem> Terms { get; set; }
    }

}
