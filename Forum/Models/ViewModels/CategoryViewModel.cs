using System;

namespace Forum.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set;}
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int PostsCount { get; set; }
        public int ThreadsCount { get; set; }
        
        public int LastActiveId { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; }
        
        public CategoryViewModel() {}

        
        public CategoryViewModel(CategoryModel model, int postsCount = 0, 
                                int threadsCount = 0, PostModel lastActive = null)
        {
            FromModel(model, postsCount, threadsCount, lastActive);
        }

        public CategoryViewModel FromModel(CategoryModel model, int postsCount, 
                                           int threadsCount, PostModel lastActive)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            if (model.Parent != null)
            {
                ParentId = model.Parent.Id;
                ParentName = model.Parent.Name;
            }
            else
            {
                ParentName = "Root";
            }

            PostsCount = postsCount;
            ThreadsCount = threadsCount;
            if (lastActive != null)
            {
                LastActive = lastActive.Created;
                LastActiveId = lastActive.ThreadId;
            }
            Created = model.Created;
            return this;
        }

        public void ToModel(CategoryModel model)
        {
            model.Id = Id;
            model.Name = Name;
            model.ParentId = ParentId;
            model.Description = Description;
        }
        
        public CategoryModel ToModel()
        {
            var model = new CategoryModel();
            ToModel(model);
            return model;
        }
    }
}