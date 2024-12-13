namespace BlogAPI.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Blog> Blogs { get; set; } = new List<Blog>(); // Çoktan çoka ilişki
    }

}
