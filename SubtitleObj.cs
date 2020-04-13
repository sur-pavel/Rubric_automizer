namespace Rubric_automizer
{
    public class SubtitleObj
    {
        public string Index_MDA { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public SubtitleObj(string index_MDA, string title, string subtitle)
        {
            Index_MDA = index_MDA;
            Title = title;
            Subtitle = subtitle;
        }
    }
}