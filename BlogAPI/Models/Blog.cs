using BlogAPI.Models;

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public string Summary { get; set; }
    public string ImageUrl { get; set; }
    public string Status { get; set; } = "taslak";
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public List<Category> Categories { get; set; } = new List<Category>();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();

}
