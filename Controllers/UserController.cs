﻿using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;


        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }

        [HttpGet("Users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            List<UserViewModel> result = new List<UserViewModel>();

            foreach (var user in users)
            {
                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Pace = user?.Pace,
                    Mileage = user?.Mileage,
                    ProfileImageUrl = user?.ProfileImageUrl,
                });
            }

            return View(result);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserById(id);
            return View(new UserDetailViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user?.Pace,
                Mileage = user?.Mileage,
            });
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(CreateUserViewModel editVM)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editVM);
            }
            var user = await _userRepository.GetByIdNoTracking(editVM.Id);
            return View(new UserDetailViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user?.Pace,
                Mileage = user?.Mileage,
            });
        }
    }
}
