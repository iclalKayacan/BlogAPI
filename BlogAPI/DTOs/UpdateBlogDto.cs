namespace BlogAPI.DTOs
{
    public class UpdateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();

    }

}
