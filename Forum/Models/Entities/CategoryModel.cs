using System;

namespace Forum.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Description { get; set;}
        public int? ParentId { get; set; }
        public CategoryModel Parent { get; set; }
        
        public DateTime Created { get; set; }
    }
}