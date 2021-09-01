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
using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Forum.Controllers
{
    public class ThreadController : Controller
    {
        private readonly ApplicationDbContext _context;

        private UserManager<ForumUser> UserManager { get; set; }
        private ForumUser User { get; set; }

        public ThreadController(ApplicationDbContext context, 
            UserManager<ForumUser> manager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            UserManager = manager;
        }

        public SelectList GetSelectList(int? selected)
        {
            var parentsIds = _context.Categories.Select(c => c.ParentId);
            var select = _context.Categories.Where(c => !parentsIds.Contains(c.Id));
            return new SelectList(select, "Id", "Name", selected);
        }

        public async Task<IActionResult> Browse(int? id)
        {
            var applicationDbContext = 
                _context.Threads
                    .Include(t => t.Parent)
                    .Include(t => t.Author)
                    .Include(t => t.Posts)
                    .Where(t => t.ParentId == id)
                    .Select(t => new ThreadViewModel().FromModel(t));
            ViewBag.Parent = await _context.Categories.FindAsync(id);
            ViewBag.UserManager = UserManager;
    
            return View(await applicationDbContext.ToListAsync());
        }
        
        // GET: Thread
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = 
                _context.Threads
                    .Include(t => t.Parent)
                    .Include(t => t.Author)
                    .Include(t => t.Posts)
                    .Select(t => new ThreadViewModel().FromModel(t));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Thread/Create
        public IActionResult Create(int? id)
        {
            ViewData["ParentId"] = GetSelectList(id);
            ViewBag.User = User;
            ViewBag.Id = id;
            return View();
        }

        // POST: Thread/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AuthorId,ParentId")] ThreadModel threadModel)
        {
            threadModel.Created = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(threadModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Id", threadModel.ParentId);
            return View(threadModel);
        }

        // GET: Thread/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var threadModel = await _context.Threads
                .Include(t => t.Parent)
                .Include(t => t.Author)
                .Include(t => t.Posts)
                .FirstOrDefaultAsync((t) => t.Id == id);
            if (threadModel == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = GetSelectList(threadModel.ParentId);
            return View(new ThreadViewModel(threadModel));
        }

        // POST: Thread/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ParentId,Closed")] ThreadViewModel threadViewModel)
        {
            var threadModel = await _context.Threads
                .Include(t => t.Parent)
                .Include(t => t.Author)
                .Include(t => t.Posts)
                .FirstOrDefaultAsync((t) => t.Id == id);
            threadViewModel.ToModel(threadModel);
            if (id != threadModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(threadModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThreadModelExists(threadModel.Id))
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
            ViewData["ParentId"] = GetSelectList(threadModel.ParentId);
            return View(threadViewModel);
        }

        private bool ThreadModelExists(int id)
        {
            return _context.Threads.Any(e => e.Id == id);
        }
    }
}
