using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepositoy;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly IPhotoService _photoService;
        public DashboardController(IDashboardRepository dashboardRepositoy,IHttpContextAccessor httpContextAccessor,IPhotoService photoservice)
        {
            _dashboardRepositoy = dashboardRepositoy;
            _httpcontextAccessor = httpContextAccessor;
            _photoService = photoservice; 
        }
        private void MapUserEdit(AppUser user,EditUserDashboardViewModel editVM,ImageUploadResult photoResult)
        {
            user.Id=editVM.Id;
            user.Pace=editVM.Pace;
            user.Mileage=editVM.Mileage;
            user.ProfileImageUrl = photoResult.Url.ToString();
            user.City =editVM.City;
            user.State = editVM.State;
        }
        public async Task<IActionResult> Index()
        {
            var userRaces=await _dashboardRepositoy.GetAllUserRaces();
            var useerClubs=await _dashboardRepositoy.GetAllUserClubs();
            var dashboardViewModel = new DashboardViewModel
            {
                Races = userRaces,
                Clubs = useerClubs,
            };
            return View(dashboardViewModel);
        }
        public async Task<IActionResult> EditUserProfile()
        {
            var curUserId=_httpcontextAccessor.HttpContext.User.GetUserId();    
            var user=await _dashboardRepositoy.GetUserById(curUserId);
            if (user == null) return View("Error");
            var editUserViewModel = new EditUserDashboardViewModel()
            {
                Id = curUserId,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfileImage = user.ProfileImageUrl,
                City = user.City,
                State = user.State
            };
            return View(editUserViewModel); 
        }
        [HttpPost]
        public async Task<IActionResult> EdituserProfile(EditUserDashboardViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile",editVM);
            }
            AppUser user = await _dashboardRepositoy.GetByIdNoTracking(editVM.Id);
            if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
            {
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user,editVM, photoResult);
                _dashboardRepositoy.Update(user);
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

                    ModelState.AddModelError("","Could not delete photo");
                    return View(editVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                _dashboardRepositoy.Update(user);
                return RedirectToAction("Index");
            }
        }
    }
}
