namespace BlogAPI.DTOs
{
    public class PatchBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
        public List<int> CategoryIds { get; set; } = new List<int>();

    }



}
