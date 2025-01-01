using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;
        public DashboardController(IDashboardRepository dashboardRepository, IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
        {
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
        }

        private void MapUserEdit(AppUser user, EditUserDashboardViewModel editUserDashboardViewModel, ImageUploadResult uploadResult)
        {
            user.Id = editUserDashboardViewModel.Id;
            user.Pace = editUserDashboardViewModel.Pace;
            user.Mileage = editUserDashboardViewModel.Mileage;
            user.ProfileImageUrl = uploadResult.Url.ToString();
            user.City = editUserDashboardViewModel.City;
            user.State = editUserDashboardViewModel.State;
        }

        public async Task<IActionResult> Index()
        {
            var userClubs = await _dashboardRepository.GetAllUserClubs();
            var userRaces = await _dashboardRepository.GetAllUserRaces();
            var dashboardViewModel = new DashboardViewModel()
            {
                Races = userRaces,
                Clubs = userClubs
            };

            return View(dashboardViewModel);

        }

        public async Task<IActionResult> EditUserProfile()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _dashboardRepository.GetUserById(curUserId);
            if (user == null)
            {
                return View("Error");
            }

            var editUserViewModel = new EditUserDashboardViewModel()
            {

                Id = curUserId,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl,
                City = user.City,
                State = user.State
            };

            return View(editUserViewModel);

        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editUserDashboardVM)
        {


            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editUserDashboardVM);
            }
            var user = await _dashboardRepository.GetUserByIdNoTracking(editUserDashboardVM.Id);
            if (user == null)
            {
                return View("Error");
            }

            if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
            {

                var photoResult = await _photoService.AddPhotoAsync(editUserDashboardVM.Image);
                MapUserEdit(user, editUserDashboardVM, photoResult);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "Could not delete photo");
                    return View(editUserDashboardVM);
                }

                var photoResult = await _photoService.AddPhotoAsync(editUserDashboardVM.Image);
                MapUserEdit(user, editUserDashboardVM, photoResult);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
        }

    }
}
