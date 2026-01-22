using System.ComponentModel.DataAnnotations;

namespace Thesis_Capstone_Archive.Models
{
    public class ManusFile
    {
        [Key]
        public int ManusFileID { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}
