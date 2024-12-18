using Microsoft.AspNetCore.Mvc;
using RealAgencyClientApp.Models;
using System.Net.Http;

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
    }
}

