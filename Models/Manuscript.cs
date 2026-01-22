using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Thesis_Capstone_Archive.Models.JuctionTables;


namespace Thesis_Capstone_Archive.Models
{
    public class Manuscript
    {
        [Key]
        public int ManuscriptID { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Adviser { get; set; }

        public DateTime PublicationDate { get; set; }

        public int? SystemTypeID { get; set; }
        public SystemType? SystemType { get; set; } 

        public int? SystemCategoryID { get; set; }
        public SystemCategory? SystemCategory { get; set; }

        //System Program
        [ForeignKey(nameof(AcademicProgram))]
        public int ProgramID { get; set; }
        public AcademicProgram? AcademicProgram { get; set; }


        //ManusFile
        [ForeignKey(nameof(ManusFile))]
        public int ManusFileID { get; set; }
        public ManusFile ManusFile { get; set; }

        public bool IsApproved { get; set; }

        // One-to-Many relationship with Author
        public ICollection<Author> Authors { get; set; } = new List<Author>();


        //FK Term
        [ForeignKey(nameof(Term))]
        public int TermID { get; set; }
        public Term Term { get; set; }

        //FK Term
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User? User { get; set; }

        public bool IsPublished { get; set; }

        [Range(0, int.MaxValue)]
        public int ViewCount { get; set; } = 0;

        public ICollection<ManuscriptKeyword> ManuscriptKeywords { get; set; } = new HashSet<ManuscriptKeyword>();

    
    }
}
