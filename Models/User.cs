using System.ComponentModel.DataAnnotations;
using Thesis_Capstone_Archive.Models;

namespace Thesis_Capstone_Archive.Models
{
    public enum UserRole
    {
        Admin,
        Staff
    }

    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "IDNumber is required.")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")] 
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        public bool IsDefaultPassword { get; set; } = true;

        public string FullName => string.Join(" ", new[] {FirstName, LastName}
                                         .Where(s => !string.IsNullOrWhiteSpace(s)));

        public ICollection<Manuscript> Manuscripts { get; set; } = new HashSet<Manuscript>();
    }

}
