    using System.ComponentModel.DataAnnotations;

namespace Thesis_Capstone_Archive.Models
{
    public class SystemType
    {
        [Key]
        public int SystemTypeID { get; set; }

        [Required(ErrorMessage = "The Type Name is required.")]
        public string SystemTypeName { get; set; }

        [Required(ErrorMessage = "The System Type needs a description.")]
        public string Description { get; set; }
    }
}
