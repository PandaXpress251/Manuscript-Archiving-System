using System.ComponentModel.DataAnnotations;

namespace Thesis_Capstone_Archive.Models
{
    public class AcademicProgram
    {
        [Key]
        public int ProgramID { get; set; }               

        [Required]
        public string ProgramName { get; set; }     

       
    }
}
