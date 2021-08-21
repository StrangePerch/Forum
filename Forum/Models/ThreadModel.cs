using System.Collections.Generic;

namespace Forum.Models
{
    public class ThreadModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string AuthorId { get; set; }
        
        public int ParentId { get; set; }
        public CategoryModel Parent { get; set; }
        public List<PostModel> Posts { get; set; }
    }
}