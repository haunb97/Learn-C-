using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)

        {
            _raceRepository = raceRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAll();
            return View(races);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository?.GetByIdAsync(id);
            return View(race);
        }

        public IActionResult Create()
        {
            var curUserID = _httpContextAccessor.HttpContext.User.GetUserId();
            var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserID };
            return View(createRaceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {

            if (ModelState.IsValid)
            {

                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                var race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    Address = new Address
                    {
                        City = raceVM.Address.City,
                        Street = raceVM.Address.Street,
                        State = raceVM.Address.State
                    },
                    AppUserId = raceVM.AppUserId,

                };
                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed!");
            };
            return View(raceVM);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _raceRepository.GetByIdAsync(id);
            if (club == null)
            {
                return View("Error");
            }

            var clubVM = new EditRaceViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                RaceCategory = club.RaceCategory,
                Address = club.Address,
                AppUserId = club.AppUserId,
            };
            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            //var club = await _clubRepository.GetByIdAsync(id);
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit race");
                return View("Edit", raceVM);
            }

            var userClub = await _raceRepository.GetByIdAsyncNoTracking(id);

            if (userClub != null)
            {

                try
                {
                    var race = new Race
                    {
                        Id = id,
                        Title = raceVM.Title,
                        Description = raceVM.Description,
                        Address = raceVM.Address,
                        AddressId = raceVM.AddressId,
                        Image = userClub.Image,
                        AppUserId = userClub.AppUserId,
                    };
                    if (raceVM.Image != null)
                    {
                        await _photoService.DeletePhotoAsync(userClub.Image);
                        var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);
                        race.Image = photoResult.Url.ToString();
                    }

                    _raceRepository.Update(race);

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(raceVM);
            }
        }

        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
            await _raceRepository.Delete(id);
            IEnumerable<Race> races = await _raceRepository.GetAll();
            return View("Index", races);
        }
    }
}
