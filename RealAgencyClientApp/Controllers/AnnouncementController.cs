using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RealAgencyClientApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;

namespace RealAgencyClientApp.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly HttpClient _httpClient;
        public AnnouncementController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/api"); // Базовый URL внешнего API
            
        }
       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var realEstates = await _httpClient.GetFromJsonAsync<List<AnnouncementListModel>>("realestate/list");
                return View(realEstates); // Передаём модель в представление
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to load real estate listings: {ex.Message}";
                return View(new List<AnnouncementListModel>()); // Передаём пустую модель в случае ошибки
            }
        }

        // Метод для загрузки фотографии к объявлению
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int realEstateId, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] = "Please select a valid photo.";
                return RedirectToAction("Index");
            }

            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(realEstateId.ToString()), "RealEstateId");
                    using (var stream = photo.OpenReadStream())
                    {
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
                        content.Add(fileContent, "Photo", photo.FileName);
                    }

                    var response = await _httpClient.PostAsync("realestate/uploadphoto", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Photo uploaded successfully.";
                    }
                    else
                    {
                        TempData["Error"] = "Error uploading photo.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error uploading photo: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<RealEstateDetailsDTO?> GetRealEstatePageDataAsync(int realEstateId)
        {
            var response = await _httpClient.GetFromJsonAsync<RealEstateDetailsDTO>($"api/announcement/page/{realEstateId}");
            return response;
        }
        public async Task<IActionResult> Announcement(int id)
        {
            var details = await _httpClient.GetFromJsonAsync<RealEstateDetailsDTO>($"api/Announcement/page/{id}");
            return View(details);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "You must be logged in to access the profile.";
                return RedirectToAction("LoginView", "Auth");
            }

            return View(new CreateAnnouncementDTO());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAnnouncementDTO model)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to access the profile.";
                    return RedirectToAction("LoginView", "Auth");
                }
                
                if (model.RealEstate.Type == "Квартира")
                {
                    model.AreaInfo = null;
                }
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                model.Userid = int.Parse(userId);
                // Отправляем запрос на сервер
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsJsonAsync("/api/announcement", model);
                
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to create announcement.");
                    return View(model);
                }

                // Успешное создание
                TempData["SuccessMessage"] = "Announcement created successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> PurchaseAnnouncements()
        {
            try
            {
                // Получаем данные через API для объявлений типа "Покупка"
                var allAnnouncements = await _httpClient.GetFromJsonAsync<List<AnnouncementListModel>>("/api/Announcement");
                var purchaseAnnouncements = allAnnouncements?.Where(a => a.Type == "Покупка").ToList();
                if (purchaseAnnouncements == null || !purchaseAnnouncements.Any())
                {
                    TempData["Info"] = "No purchase announcements available.";
                    return View(new List<AnnouncementListModel>());
                }

                return View(purchaseAnnouncements); // Передаём данные в представление
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to load purchase announcements: {ex.Message}";
                return View(new List<AnnouncementListModel>());
            }
        }
    }
    }


