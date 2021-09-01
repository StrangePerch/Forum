using System;
using System.Collections.Generic;
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

            return View();
        }
    }
}