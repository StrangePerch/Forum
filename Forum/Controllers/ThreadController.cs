using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;

namespace Forum.Controllers
{
    public class ThreadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThreadController(ApplicationDbContext context)
        {
            _context = context;
        }  

        public async Task<IActionResult> Browse(int? id)
        {
            var applicationDbContext = 
                _context.Threads.Include(c => c.Parent)
                    .Where(c => c.ParentId == id);
            var parentCategory = await _context.Categories.FindAsync(id);
            ViewBag.ParentId = parentCategory.ParentId;
            return View(await applicationDbContext.ToListAsync());
        }
        
        // GET: Thread
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Threads.Include(t => t.Parent);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Thread/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var threadModel = await _context.Threads
                .Include(t => t.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (threadModel == null)
            {
                return NotFound();
            }

            return View(threadModel);
        }

        // GET: Thread/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Id");
            return View();
        }

        // POST: Thread/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AuthorId,ParentId")] ThreadModel threadModel)
        {
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

            var threadModel = await _context.Threads.FindAsync(id);
            if (threadModel == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Id", threadModel.ParentId);
            return View(threadModel);
        }

        // POST: Thread/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AuthorId,ParentId")] ThreadModel threadModel)
        {
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
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Id", threadModel.ParentId);
            return View(threadModel);
        }

        // GET: Thread/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var threadModel = await _context.Threads
                .Include(t => t.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (threadModel == null)
            {
                return NotFound();
            }

            return View(threadModel);
        }

        // POST: Thread/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var threadModel = await _context.Threads.FindAsync(id);
            _context.Threads.Remove(threadModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThreadModelExists(int id)
        {
            return _context.Threads.Any(e => e.Id == id);
        }
    }
}
