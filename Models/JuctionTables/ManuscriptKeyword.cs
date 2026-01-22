namespace Thesis_Capstone_Archive.Models.JuctionTables
{
    public class ManuscriptKeyword
    {
        public int ManuscriptID { get; set; }
        public Manuscript Manuscript { get; set; }

        public int KeywordID { get; set; }
        public Keyword Keyword { get; set; }
    }
}
