using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http;
using RealAgencyClientApp.Models;
using System.Text.Json;

namespace RealAgencyClientApp.Controllers
{
    public class CooperationController : Controller
    {

        private readonly HttpClient _httpClient;

        public CooperationController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/api"); // Базовый URL внешнего API
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCooperation(int bidUserId)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to propose a cooperation.";
                    return RedirectToAction("LoginView", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Извлекаем BidPartnerId из токена
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var bidPartnerId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(bidPartnerId))
                {
                    TempData["Error"] = "Failed to retrieve agent ID.";
                    return RedirectToAction("PurchaseAnnouncements", "Announcement");
                }

                // Данные для отправки на сервер
                var cooperationData = new
                {
                    id = 0,
                    bidpartnerid = int.Parse(bidPartnerId),
                    biduserid = bidUserId
                };

                var response = await _httpClient.PostAsJsonAsync("/api/Cooperation", cooperationData);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to propose cooperation.";
                    return RedirectToAction("PurchaseAnnouncements", "Announcement");
                }

                TempData["Success"] = "Cooperation proposal sent successfully!";
                return RedirectToAction("PurchaseAnnouncements", "Announcement");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("PurchaseAnnouncements", "Announcement");
            }
        }
        [HttpGet]
        public async Task<IActionResult> MyCooperations()
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to view your cooperations.";
                    return RedirectToAction("LoginView", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var bidPartnerId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(bidPartnerId))
                {
                    TempData["Error"] = "Failed to retrieve agent ID.";
                    return RedirectToAction("PurchaseAnnouncements", "Announcement");
                }

                var cooperations = await _httpClient.GetFromJsonAsync<List<CooperationModel>>($"/api/Cooperation/agent/{bidPartnerId}");
                return View(cooperations);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(new List<CooperationModel>());
            }


        }
        [HttpPost]
        public async Task<IActionResult> DeleteCooperation(int id)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to delete a cooperation.";
                    return RedirectToAction("LoginView", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Отправляем запрос на сервер для удаления сотрудничества
                var response = await _httpClient.DeleteAsync($"/api/Cooperation/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to delete cooperation.";
                    return RedirectToAction("MyCooperations");
                }

                TempData["Success"] = "Cooperation deleted successfully.";
                return RedirectToAction("MyCooperations");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("MyCooperations");
            }
        }

      
       
        [HttpGet]
        public async Task<IActionResult> MyProposals()
        {
            try
            {
                var userId = GetUserIdFromToken();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);

                var cooperationResponse = await _httpClient.GetFromJsonAsync<CooperationModel>($"/api/Cooperation/api/Cooperation/user/{userId}");

                

                int cooperationId = cooperationResponse.Id;
                var announcements = await _httpClient.GetFromJsonAsync<List<AnnouncementListModel>>($"/api/Cooperation/api/Cooperation/{cooperationId}/Announcements");
                return View("MyProposals", announcements);
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("LoginView", "Auth");
            }
           
        }
        private string GetUserIdFromToken()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("JWT token is missing.");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Извлекаем userId из токена
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID is missing in the token.");
            }

            return userId;
        }
    }

}
