namespace BlogAPI.DTOs
{

    public class CreateCommentDto
    {
        public string Content { get; set; }
        public string Author { get; set; }
        public int BlogId { get; set; } 
    }

    public class UpdateCommentDto
    {
        public string Content { get; set; }
        public string Author { get; set; }
    }

}
