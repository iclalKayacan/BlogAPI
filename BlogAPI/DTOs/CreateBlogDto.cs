﻿namespace BlogAPI.DTOs
{
    public class CreateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public List<int> TagIds { get; set; }

        public List<int> CategoryIds { get; set; } = new List<int>();


    }

}
