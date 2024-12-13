using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public int BlogId { get; set; }

        [JsonIgnore] 
        public Blog Blog { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
