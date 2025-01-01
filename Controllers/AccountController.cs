using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        public IActionResult Login()
        {
            var response = new LoginVM();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user != null)
            {
                // user is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, login.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                    // password is corect
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Race");
                    }
                    // password is incorect
                    //TempData["Error"] = "Wrong credentials, please, try again";
                    //return View(login);
                }
                // User not found
                //TempData["Error"] = "Wrong credentials, please, try again";
                //return View(login);

            }
            // User not found
            TempData["Error"] = "Wrong credentials, please, try again";
            return View(login);

        }

        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);

            if (user != null)
            {
                TempData["Error"] = "This email address is alrealdy in use";
                return View(registerVM);
            }
            // User not found
            var newUser = new AppUser()
            {
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                return RedirectToAction("Index", "Race");
            }
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }

    }
}
