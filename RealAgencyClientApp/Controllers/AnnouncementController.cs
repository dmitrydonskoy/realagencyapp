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
        public async Task<IActionResult> Create(CreateAnnouncementDTO model, IFormFile? photo)
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

                // Отправляем запрос на сервер для создания объявления
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsJsonAsync("/api/announcement", model);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to create announcement.");
                    return View(model);
                }
                var createdAnnouncement = await response.Content.ReadFromJsonAsync<CreateAnnouncementResponseDTO>();
                // Получаем ID созданного объявления
                var realEstateId = createdAnnouncement?.RealEstate.Id;  // Изменил на realEstateId

                if (realEstateId == null)
                {
                    ModelState.AddModelError("", "Failed to retrieve real estate ID.");
                    return View(model);
                }

                // Если фото загружено
                if (photo != null && photo.Length > 0)
                {
                    var photoContent = new MultipartFormDataContent();
                    var photoStream = new StreamContent(photo.OpenReadStream());
                    photoStream.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
                    photoContent.Add(photoStream, "photo", photo.FileName);

                    // Загружаем фото через realEstateId
                    var uploadResponse = await _httpClient.PostAsync($"/uploadPhoto?realEstateId={realEstateId}", photoContent);

                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("", "Failed to upload photo.");
                        return View(model);
                    }
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
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int realEstateId, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                ModelState.AddModelError("", "Please select a valid photo.");
                return RedirectToAction("Edit", new { id = realEstateId });
            }

            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to upload photos.";
                    return RedirectToAction("LoginView", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(realEstateId.ToString()), "realEstateId");
                    using (var stream = photo.OpenReadStream())
                    {
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
                        content.Add(fileContent, "photo", photo.FileName);
                    }

                    var response = await _httpClient.PostAsync("/uploadphoto", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("", "Failed to upload photo.");
                        return RedirectToAction("Edit", new { id = realEstateId });
                    }
                }

                TempData["Success"] = "Photo uploaded successfully.";
                return RedirectToAction("Edit", new { id = realEstateId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return RedirectToAction("Edit", new { id = realEstateId });
            }
        }


    }
    }


