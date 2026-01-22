using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Thesis_Capstone_Archive.Models.JuctionTables;

namespace Thesis_Capstone_Archive.Models
{
    public class Author
    {
        [Key]
        public int AuthorID { get; set; }


        [MaxLength(255)]
        public string FirstName { get; set; }


        [MaxLength(255)]
        public string LastName { get; set; }

        //FK Manuscript
        [ForeignKey(nameof(Manuscript))]
        public int ManuscriptID { get; set; }
        public Manuscript Manuscript { get; set; }

    }
}
