using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class ForumUser : IdentityUser
    {
        public string Location { get; set; }
        public DateTime JoinedAt { get; set; }

        public string GetHighestRole(UserManager<ForumUser> manager)
        {
            var task = manager.GetRolesAsync(this);
            task.Wait();
            var roles = task.Result;
            return GetHighestRole(roles.ToList());
        }
        
        public string GetHighestRole(List<string> roles)
        {
            if(roles == null) throw new Exception("User without roles: " + Email);
            if (roles.Contains("Admin"))
                return "Administrator";
            if (roles.Contains("Senior"))
                return "Senior member";
            if (roles.Contains("Basic"))
                return "Basic member";
            if (roles.Contains("Novice"))
                return "Novice member";
            
            throw new Exception("This should never happen");
        }
    }
}