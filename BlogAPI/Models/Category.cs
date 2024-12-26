using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Color { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();


    }
}
