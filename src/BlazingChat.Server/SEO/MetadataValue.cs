namespace BlazingChat.Server.SEO
{
    public class MetadataValue
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OgTitle { get; set; } = "This title is for Facebook";
        public string TwitterCard { get; set; } = "This Title is for Twitter";
    }
}