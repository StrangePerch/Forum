using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DotNet.PlatformAbstractions;

namespace Forum.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ForumUser> _userManager;
        private readonly SignInManager<ForumUser> _signInManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public IndexModel(
            UserManager<ForumUser> userManager,
            SignInManager<ForumUser> signInManager, 
            IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appEnvironment = appEnvironment;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Username")]
            public string Username { get; set; }
            [Display(Name = "Location")]
            public string Location { get; set; }
            
            [Display(Name = "Avatar")]
            
            public string AvatarRef { get; set; }
            public IFormFile AvatarFile { get; set; }
        }

        private async Task LoadAsync(ForumUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Location = user.Location,
                AvatarRef = user.Avatar,
                Username = user.UserName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.AvatarFile != null)
            {
                var file = Input.AvatarFile;
                var filename = Guid.NewGuid() + "$" + file.FileName;
                var path = "\\avatars\\" + filename;
                await using var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create);
                await file.CopyToAsync(fileStream);
                user.Avatar = path;
            }
            if (Input.Location != user.Location)
            {
                user.Location = Input.Location;
            }
            if (Input.Username != user.UserName)
            {
                user.UserName = Input.Username;
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
