using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ForumUser> _userManager;
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context, UserManager<ForumUser> manager)
        {
            _userManager = manager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var posts = _context.Posts.Count(p => p.Author == user);
                userViewModels.Add(new UserViewModel(user, roles.ToList(), posts));
            }
            return View(userViewModels);
        }
        
        // GET: User/Details/e7f99321-9f1c-4672-9301-221a64f3dc42
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userModel = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            if (userModel == null)
            {
                return NotFound();
            }
            
            var roles = await _userManager.GetRolesAsync(userModel);
            var posts = _context.Posts.Count(p => p.Author == userModel);
            
            return View(new UserViewModel(userModel, roles.ToList(), posts));
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userModel = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            if (userModel == null)
            {
                return NotFound();
            }
            
            var roles = await _userManager.GetRolesAsync(userModel);
            var posts = _context.Posts.Count(p => p.Author == userModel);
            
            ViewBag.Roles = new SelectList(_context.Roles, "Name", "Name", _userManager.GetRolesAsync(userModel));
            
            return View(new UserViewModel(userModel, roles.ToList(), posts));
        }

        // GET: User/Ban/5
        public async Task<IActionResult> Ban(string id)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            var roles = await _userManager.GetRolesAsync(user);
            var posts = _context.Posts.Count(p => p.Author == user);
            return View(new UserViewModel(user, roles.ToList(), posts));
        }
        
        // POST: User/Ban/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(string id, DateTime until)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            await _userManager.AddToRoleAsync(user, "Banned");
            return RedirectToAction("Index");
        }
        
        
        // // POST: User/Ban/5
        // [HttpPost]
        // public async Task<IActionResult> UnBan(string id)
        // {
        //     var user = await _userManager.Users
        //         .FirstOrDefaultAsync(u => u.Id == id);
        //     await _userManager.RemoveFromRoleAsync(user, "Banned");
        //     return View();
        // }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id, UserName, Location, Roles")] UserViewModel userViewModel)
        {
            var userModel = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            userViewModel.ToModel(userModel);
            var roles = userViewModel.Roles;
            if (id != userViewModel.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var currentRoles = await _userManager.GetRolesAsync(userModel);
                    var rolesToRemove = currentRoles.Where(r => !roles.Contains(r));
                    var rolesToAdd = roles.Where(r => !currentRoles.Contains(r));
                    await _userManager.RemoveFromRolesAsync(userModel, rolesToRemove);
                    await _userManager.AddToRolesAsync(userModel, rolesToAdd);

                    _context.Update(userModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserModelExists(userModel.Id))
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
            ViewBag.Roles = new SelectList(_context.Roles, "Name", "Name", _userManager.GetRolesAsync(userModel));
            return View(userViewModel);
        }

        private bool UserModelExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}