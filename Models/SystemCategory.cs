using System.ComponentModel.DataAnnotations;

namespace Thesis_Capstone_Archive.Models
{
    public class SystemCategory
    {
        [Key]
        public int SystemCategoryID { get; set; }

        [Required(ErrorMessage = "The Category Name is required.")]
        [MaxLength(100)]
        public string SystemCategoryName { get; set; }

        [Required(ErrorMessage = "The System Category needs a description.")]
        [MaxLength(5000)]
        public string Description { get; set; }
    }
}
