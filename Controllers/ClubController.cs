using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {

        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }

        public IActionResult Create()
        {

            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var createClubViewModel = new CreateClubViewModel { AppUserId = curUserId };
            return View(createClubViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {

            if (ModelState.IsValid)
            {

                var result = await _photoService.AddPhotoAsync(clubVM.Image);
                var club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = result.Url.ToString(),
                    Address = new Address
                    {
                        City = clubVM.Address.City,
                        Street = clubVM.Address.Street,
                        State = clubVM.Address.State
                    },
                    AppUserId = clubVM.AppUserId,
                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed!");
            };
            return View(clubVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return View("Error");
            }

            var clubVM = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                ClubCategory = club.ClubCategory,
                Address = club.Address,
                AppUserId = club.AppUserId,
            };
            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel clubVM)
        {
            //var club = await _clubRepository.GetByIdAsync(id);
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", clubVM);
            }

            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);

            if (userClub != null)
            {

                try
                {
                    var club = new Club
                    {
                        Id = id,
                        Title = clubVM.Title,
                        Description = clubVM.Description,
                        Address = clubVM.Address,
                        AddressId = clubVM.AddressId,
                        Image = userClub.Image,
                        AppUserId = userClub.AppUserId,

                    };
                    if (clubVM.Image != null)
                    {
                        await _photoService.DeletePhotoAsync(userClub.Image);
                        var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);
                        club.Image = photoResult.Url.ToString();
                    }

                    _clubRepository.Update(club);

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(clubVM);
            }
        }

        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
            await _clubRepository.Delete(id);
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View("Index", clubs);
        }
    }
}

