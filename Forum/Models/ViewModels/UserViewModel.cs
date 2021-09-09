using System;
using System.Collections.Generic;
using System.Linq;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime JoinedAt { get; set; }
        public string Location { get; set; }
        public string HighestRole { get; set; }
        public List<string> Roles { get; set; }
        public int Posts { get; set; }
        
        public UserViewModel() {}

        public UserViewModel(ForumUser model, List<string> roles, int posts)
        {
            FromModel(model, roles, posts);
        }

        public void FromModel(ForumUser model, List<string> roles, int posts)
        {
            Id = model.Id;
            UserName = model.UserName;
            Email = model.Email;
            JoinedAt = model.JoinedAt;
            Location = model.Location;
            HighestRole = model.GetHighestRole(roles);
            Roles = roles;
            Posts = posts;
        }

        public void ToModel(ForumUser model)
        {
            model.UserName = UserName;
            model.Location = Location;
        }
    }
}