namespace WT_API.Models
{
    public class Serial
    {
        public string id { get; set; }
        public string title{ get; set; }
        public string status { get; set; }
        public string author { get; set; }
        public string link { get; set; }
        public string nextChLinkXPath { get; set; }
        //public bool reviewStatus { get; set; }

        public Serial() { }
    }
}
