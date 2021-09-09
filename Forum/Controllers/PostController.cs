using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Forum.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ForumUser> UserManager { get; set; }

        private int _pagesize = 5;
        
        private IWebHostEnvironment _appEnvironment;
        public PostController(ApplicationDbContext context, 
            UserManager<ForumUser> manager, 
            IWebHostEnvironment appEnvironment)
        {
            _context = context;
            UserManager = manager;
            _appEnvironment = appEnvironment;
        }
    
        [Route("{controller}/{action}/{id}/{_pagesize?}/{page?}")]
        public async Task<IActionResult> Browse(int id, int page = -1)
        {
            var applicationDbContext = await _context.Posts
                .Include(p => p.Thread)
                .Include(p => p.Author)
                .Include(p => p.Quote)
                .Where(p => p.ThreadId == id)
                .ToListAsync();
            ViewBag.Parent = await _context.Threads.FindAsync(id);
            ViewBag.UserManager = UserManager;
            if(!applicationDbContext.Any()) 
                return View(applicationDbContext);
            ViewBag.MaxPage = (int)Math.Ceiling((float)applicationDbContext.Count / _pagesize) - 1;
            if (page == -1)
                page = ViewBag.MaxPage;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = _pagesize;
            var index = (page) * _pagesize;
            if(applicationDbContext.Count < index + _pagesize)
                _pagesize = applicationDbContext.Count - index;
            return View(applicationDbContext.GetRange(index, _pagesize));
        }   
        
        // GET: Post
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = 
                _context.Posts.Include(p => p.Thread);
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Text,AuthorId,ThreadId,QuoteId,AttachmentsPaths")] 
            PostModel postModel, List<IFormFile> files)
        {
            var author = await UserManager.GetUserAsync(User);
            postModel.AuthorId = author.Id;
            postModel.Created = DateTime.Now;

            if (files != null)
            {
                foreach (var file in files)
                {
                    var filename = Guid.NewGuid() + "$" + file.FileName;
                    var path = "\\files\\" + filename;
                    await using var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    postModel.AttachmentsPaths += filename + ";";
                }
            }


            if (ModelState.IsValid)
            {
                _context.Add(postModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Browse", new { id = postModel.ThreadId });
            }
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Id", postModel.ThreadId);
            return View(postModel);
        }

        // GET: Post/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts.FindAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Id", postModel.ThreadId);
            return View(postModel);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Text,AuthorId,ThreadId,AttachmentsPaths")] PostModel postModel)
        {
            if (id != postModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostModelExists(postModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Id", postModel.ThreadId);
            return View(postModel);
        }

        private bool PostModelExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
