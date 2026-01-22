using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Humanizer;

namespace Thesis_Capstone_Archive.Models
{
    public class Term
    {
        [Key]
        public int TermID { get; set; }

        [Required(ErrorMessage = "The Term Name is required.")]
        [MaxLength(50)]
        public string TermName { get; set; }

        [Required(ErrorMessage = "The Start Year is required.")]
        public DateTime StartYear { get; set; }

        [Required(ErrorMessage = "The End Year is required.")]
        public DateTime EndYear { get; set; }

        [NotMapped]
        public string AcademicYear => $"{StartYear:MMMM yyyy} - {EndYear:MMMM yyyy}";

        public ICollection<Manuscript> Manuscripts { get; set; } = new HashSet<Manuscript>();
    }
}
