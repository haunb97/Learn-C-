using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public interface IUserController
    {
        IActionResult Index();
        IActionResult Login();
        Task<IActionResult> Login(LoginVM login);
        Task<IActionResult> Logout();
        IActionResult Register();
        Task<IActionResult> Register(RegisterViewModel registerVM);
    }
}