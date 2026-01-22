using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Thesis_Capstone_Archive.Helpers;

namespace Thesis_Capstone_Archive.Models.ViewModels
{
    public class ManuscriptUploadViewModel
    {
       //GET MANUSCRIPT TITLE
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(255)]
        public string Title { get; set; }

       //GET ADVISER
        [MaxLength(255)]
        public string Adviser { get; set; }

       //AUTO INCREMENT PUBLISH DATE
        [Required(ErrorMessage = "Publication date is required.")]
        public DateTime PublicationDate { get; set; }

       //GET MANUSFILE
        [Required(ErrorMessage = "Please upload a PDF file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile ManusFile { get; set; }

       //GET SYSTEM TYPE
        [Required(ErrorMessage = "The System Type is required.")]
        public int? SystemTypeID { get; set; }
        public IEnumerable<SelectListItem>? SystemTypes { get; set; }

       //GET SYSTEM CATEGORY
        [Required(ErrorMessage = "The System Category is required.")]
        public int? SystemCategoryID { get; set; }
        public IEnumerable<SelectListItem>? SystemCategories { get; set; }

        //GET SYSTEM Program
        [Required(ErrorMessage = "The Program is required.")]
        public int ProgramID { get; set; }
        public IEnumerable<SelectListItem> AcademicProgram { get; set; }

        //GET TERM
        [Required(ErrorMessage = "The Term is required.")]
        public int TermID { get; set; }
        public IEnumerable<SelectListItem> Terms { get; set; }

        public List<AuthorViewModel> Authors { get; set; } = new List<AuthorViewModel>();
        public List<string> Keywords { get; set; } = new List<string>();


    //New Added
        [Required]
        public int UserID { get; set; }

        public User User { get; set; }

        [Required]
        public bool IsPublished { get; set; }
       
    }
}
