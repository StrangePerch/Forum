using System;
using System.Collections.Generic;
using System.Text;
using Forum.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<ThreadModel> Threads { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}