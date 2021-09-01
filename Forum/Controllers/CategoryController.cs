using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Forum.Models.ViewModels;

namespace Forum.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Browse(int? id)
        {
            var applicationDbContext = 
                _context.Categories.Include(c => c.Parent)
                    .Where(c => c.ParentId == id);
            if (!applicationDbContext.Any())
                return RedirectToAction("Browse", "Thread", new { id });

            var categoryViewModels = new List<CategoryViewModel>();
            foreach (var category in applicationDbContext)
            {
                var threadCount = 0;
                var postsCount = 0;
                var lastActive = new DateTime();
                foreach (var thread in _context.Threads
                    .Include(t => t.Parent.Parent)
                    .Include(t => t.Posts))
                {
                    if (IsThreadChild(thread, category))
                    {
                        threadCount++;
                        if (thread.Posts != null)
                        {
                            postsCount += thread.Posts.Count;
                            var last = thread.Posts.Max(p => p.Created);
                            if (last > lastActive) lastActive = last;
                        }
                    }
                }
                
                categoryViewModels.Add(new CategoryViewModel()
                    .FromModel(category, postsCount, threadCount, 
                        await _context.Posts.FirstOrDefaultAsync(c => c.Created == lastActive)));
            }
            
            return View(categoryViewModels);
        }

        public bool IsThreadChild(ThreadModel thread, CategoryModel category)
        {
            return IsCategoryChild(thread.Parent, category);
        }

        public bool IsCategoryChild(CategoryModel childCategory, CategoryModel parentCategory)
        {
            while (true)
            {
                if (childCategory == parentCategory) return true;
                if (childCategory == null) return false;
                childCategory = childCategory.Parent;
            }
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var applicationDbContext =
                _context.Categories.Include(c => c.Parent);

            var categoryViewModels = new List<CategoryViewModel>();
            foreach (var category in applicationDbContext)
            {
                var threadCount = 0;
                var postsCount = 0;
                var lastActive = new DateTime();
                foreach (var thread in _context.Threads
                    .Include(t => t.Parent.Parent)
                    .Include(t => t.Posts))
                {
                    if (IsThreadChild(thread, category))
                    {
                        threadCount++;
                        if (thread.Posts != null)
                        {
                            postsCount += thread.Posts.Count;
                            var last = thread.Posts.Max(p => p.Created);
                            if (last > lastActive) lastActive = last;
                        }
                    }
                }
                
                categoryViewModels.Add(new CategoryViewModel()
                    .FromModel(category, postsCount, threadCount, 
                        await _context.Posts.FirstOrDefaultAsync(c => c.Created == lastActive)));
            }
            
            return View(categoryViewModels);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            return View(categoryModel);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            List<SelectListItem> items = new SelectList(_context.Categories, "Id", "Name").ToList();
            items.Insert(0, (new SelectListItem { Text = "Root", Value = "-1" }));
            ViewData["ParentId"] = items;
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ParentId")] CategoryModel categoryModel)
        {
            categoryModel.Created = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (categoryModel.ParentId == -1)
                    categoryModel.ParentId = null;
                _context.Add(categoryModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name", categoryModel.ParentId);
            return View(categoryModel);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Categories.FindAsync(id);
            if (categoryModel == null)
            {
                return NotFound();
            }
            List<SelectListItem> items = new SelectList(_context.Categories.Where(c => c.Id != id), "Id", "Name").ToList();
            items.Insert(0, (new SelectListItem { Text = "Root", Value = "-1" }));
            ViewData["ParentId"] = items;
            return View(new CategoryViewModel(categoryModel));
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Name,ParentId")] CategoryViewModel categoryViewModel)
        {
            var categoryModel = categoryViewModel.ToModel();
            if (id != categoryModel.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (categoryModel.ParentId == -1)
                    categoryModel.ParentId = null;
                try
                {
                    _context.Update(categoryModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryModelExists(categoryModel.Id))
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
            List<SelectListItem> items = new SelectList(_context.Categories.Where(c => c.Id != id), "Id", "Name").ToList();
            items.Insert(0, (new SelectListItem { Text = "Root", Value = "-1" }));
            ViewData["ParentId"] = items;            
            return View(categoryViewModel);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            return View(categoryModel);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryModel = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(categoryModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryModelExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
