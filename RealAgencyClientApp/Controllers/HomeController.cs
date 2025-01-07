using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealAgencyClientApp.Models;
using System.Diagnostics;
using System.Net.Http;

namespace RealAgencyClientApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
		{
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/api");
            _logger = logger;
		}

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Announcement/get-all");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load real estate listings.";
                return View(new List<AnnouncementListModel>());
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var realEstates = JsonConvert.DeserializeObject<List<AnnouncementListModel>>(content);

            return View(realEstates);
        }
        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
        // Получение подробной информации по объявлению
        public async Task<IActionResult> Announcement(int id)
        {
            var details = await _httpClient.GetFromJsonAsync<RealEstateDetailsDTO>($"api/Announcement/page/{id}");
            return View(details);
        }
    }
}