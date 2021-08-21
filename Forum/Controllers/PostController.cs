using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Forum.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<IdentityUser> UserManager { get; set; }
        private IdentityUser User { get; set; }

        public PostController(ApplicationDbContext context, 
            UserManager<IdentityUser> manager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            UserManager = manager;
            
            var task = UserManager.FindByIdAsync(httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value);
            Task.WaitAll(task);
            User = task.Result;
        }

        public async Task<IActionResult> Browse(int? id)
        {
            var applicationDbContext = 
                _context.Posts.Include(c => c.Thread)
                    .Where(p => p.ThreadId == id);
            ViewBag.Parent = await _context.Threads.FindAsync(id);
            ViewBag.UserManager = UserManager;
            return View(await applicationDbContext.ToListAsync());
        }
        
        // GET: Post
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Thread);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.Thread)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // GET: Post/Create
        public IActionResult Create(int? id)
        {
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Id");
            ViewBag.User = User;
            ViewBag.Id = id;
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Text,AuthorId,ThreadId,AttachmentsPaths")] PostModel postModel)
        {
            postModel.AuthorId = User.Id;
            postModel.Created = DateTime.Now;
            
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

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.Thread)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postModel = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(postModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostModelExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
