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
using Microsoft.AspNetCore.Authorization;

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
                return RedirectToAction("Browse", "Thread", new {id});

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
                        if (thread.Posts.Any())
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

        private static bool IsCategoryChild(CategoryModel childCategory, CategoryModel parentCategory)
        {
            while (true)
            {
                if (childCategory == parentCategory) return true;
                if (childCategory == null) return false;
                childCategory = childCategory.Parent;
            }
        }

        [Authorize(Roles = "Admin")]
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
                        if (thread.Posts.Any())
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<SelectListItem> items = new SelectList(_context.Categories, "Id", "Name").ToList();
            items.Insert(0, (new SelectListItem {Text = "Root", Value = "-1"}));
            ViewData["ParentId"] = items;
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
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

            List<CategoryModel> collection = new();
            foreach (var category in _context.Categories.Include(c => c.Parent))
            {
                if(!IsCategoryChild(category, categoryModel))
                    collection.Add(category);
            }
            List<SelectListItem> items = new SelectList(collection,
                    "Id", "Name").ToList();
            items.Insert(0, (new SelectListItem {Text = "Root", Value = "-1"}));
            ViewData["ParentId"] = items;
            return View(new CategoryViewModel(categoryModel));
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Description,Name,ParentId")] CategoryViewModel categoryViewModel)
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

            List<CategoryModel> collection = new();
            foreach (var category in _context.Categories.Include(c => c.Parent))
            {
                if(!IsCategoryChild(category, categoryModel))
                    collection.Add(category);
            }
            List<SelectListItem> items = new SelectList(collection,
                "Id", "Name").ToList();
            items.Insert(0, (new SelectListItem {Text = "Root", Value = "-1"}));
            ViewData["ParentId"] = items;
            return View(categoryViewModel);
        }

        [Authorize(Roles = "Admin")]
        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationDbContext =
                _context.Categories.Include(c => c.Parent)
                    .Where(c => c.ParentId == id);

            var categoryModel = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            List<SelectListItem> items;
            if (applicationDbContext.Any())
            {
                List<CategoryModel> collection = new();
                foreach (var category in _context.Categories.Include(c => c.Parent))
                {
                    if(!IsCategoryChild(category, categoryModel))
                        collection.Add(category);
                }
                items = new SelectList(collection,
                    "Id", "Name").ToList();
                items.Insert(0, (new SelectListItem {Text = "Root", Value = "-1"}));
            }
            else if(_context.Threads.Any(t => t.ParentId == (int)id))
            {
                var parentsIds = _context.Categories.Select(c => c.ParentId);
                var select = _context.Categories
                    .Where(c => !parentsIds.Contains(c.Id))
                    .Where(c => c.Id != id);
                items = new SelectList(select, "Id", "Name").ToList();
            }
            else
            {
                items = null;
            }

            ViewData["ParentId"] = items;
            
            if (categoryModel == null)
            {
                return NotFound();
            }

            return View(new CategoryViewModel(categoryModel));
        }

        // POST: Category/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int ParentId)
        {
            foreach (var thread in _context.Threads.Where(t => t.ParentId == id))
            {
                thread.ParentId = ParentId;
                _context.Update(thread);
            }
            foreach (var category in _context.Categories.Where(c => c.ParentId == id))
            {
                category.ParentId = ParentId == -1 ? null : ParentId;
                _context.Update(category);
            }
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