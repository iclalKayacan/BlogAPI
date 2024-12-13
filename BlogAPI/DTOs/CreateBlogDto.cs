namespace BlogAPI.DTOs
{
    public class CreateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public List<int> TagIds { get; set; } // Sadece etiket ID'leri
        public List<CreateCommentDto> Comments { get; set; } // Yorum oluşturma
    }
}
