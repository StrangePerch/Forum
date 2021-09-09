using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Posts = _context.Posts.Count();
            ViewBag.Threads = _context.Threads.Count();
            ViewBag.Users = _context.Users.Count();
            ViewBag.Categories = _context.Categories.Count();
            var files = Directory.GetFiles("wwwroot/Files");
            
            ViewBag.Files = files.Length;
            long size = 0;
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                size += fileInfo.Length;
            }
            ViewBag.Size = size / 1024;

            return View();
        }
    }
}