using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RealAgencyClientApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace RealAgencyClientApp.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/api"); // Базовый URL внешнего API
        }

        // Получение данных пользователя
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            try
            {
                // Извлечение токена из cookies
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to access the profile.";
                    return RedirectToAction("LoginView", "Auth");
                }
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                
                // Извлечение userId из токена
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "Invalid user session.";
                    return RedirectToAction("LoginView", "Auth");
                }

               

                // Установка заголовка авторизации
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Получение профиля пользователя
                var user = await _httpClient.GetFromJsonAsync<UserModel>($"/api/user/{int.Parse(userId)}");
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Если пользователь - агент, загружаем профиль агента
                if (user.Role?.Contains("agent") == true)
                {
                    var agentProfile = await _httpClient.GetFromJsonAsync<AgentProfileModel>($"users/{userId}/agent-profile");
                    ViewBag.AgentProfile = agentProfile;
                }

                // Передача данных в представление
                return View(user);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Network error occurred while loading user profile.";
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
                Console.WriteLine($"Exception: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        // Обновление данных пользователя
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"users/{model.Id}", model);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to update user.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("UserProfile", new { id = model.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }
    }
}