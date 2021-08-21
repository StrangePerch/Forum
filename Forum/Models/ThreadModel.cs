using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class ThreadModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string AuthorId { get; set; }
        public IdentityUser Author { get; set; }
        
        public int ParentId { get; set; }
        public CategoryModel Parent { get; set; }
        public List<PostModel> Posts { get; set; }
    }
}