using System.ComponentModel.DataAnnotations;

namespace Thesis_Capstone_Archive.Models.ViewModels
{
    public class AuthorViewModel
    {
        [Required(ErrorMessage = "Author's first name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Author's last name is required.")]
        public string LastName { get; set; }
    }
}
