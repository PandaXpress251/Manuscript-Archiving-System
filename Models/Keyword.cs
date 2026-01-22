using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Thesis_Capstone_Archive.Models.JuctionTables;

namespace Thesis_Capstone_Archive.Models
{
    public class Keyword
    {
        [Key]
        public int KeywordID { get; set; }


        [MaxLength(100)]
        public string KeywordName { get; set; }

    }

}
