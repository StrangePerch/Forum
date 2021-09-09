using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Forum.Models.ViewModels
{
    public class ThreadViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public int PostsCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public bool Closed { get; set; }
        
        public ThreadViewModel() {}

        public ThreadViewModel(ThreadModel model)
        {
            FromModel(model);
        }

        public ThreadViewModel FromModel(ThreadModel model)
        {
            Id = model.Id;
            Name = model.Name;
            if (model.Author != null)
            {
                AuthorId = model.Author.Id;
                AuthorName = model.Author.UserName;
            }
            else
            {
                AuthorName = "No author";
            }
            if (model.Parent != null)
            {
                ParentId = model.Parent.Id;
                ParentName = model.Parent.Name;
            }
            else
            {
                ParentName = "Root";
            }

            if (model.Posts.Any())
            {
                PostsCount = model.Posts.Count;
                LastActive = model.Posts.Max(p => p.Created);
            }
            Created = model.Created;
            Closed = model.Closed;
            return this;
        }

        public void ToModel(ThreadModel model)
        {
            model.Id = Id;
            model.Name = Name;
            model.ParentId = ParentId;
            model.Closed = Closed;
        }
        
        public ThreadModel ToModel()
        {
            var model = new ThreadModel();
            ToModel(model);
            return model;
        }
    }
}