using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public int? profileId;

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
                AgentProfileModel? agentProfile = null;

                // Если пользователь - агент, загружаем профиль агента
                if (user.Role.Equals("Agent") == true)
                {
                    agentProfile = await _httpClient.GetFromJsonAsync<AgentProfileModel>($"/api/profile/agentProfile/{userId}");
                    if (agentProfile == null)
                    {
                        profileId = agentProfile.Id;
                        ViewBag.AgentProfile = agentProfile;
                    }
                }

                

                var viewModel = new AgentProfileViewModel
                {
                    User = user,
                    agentProfile = agentProfile
                };


                // Передача данных в представление
                return View(viewModel);
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

        [HttpGet]
        public async Task<IActionResult> EditUserProfile()
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to edit your profile.";
                    return RedirectToAction("LoginView", "Auth");
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "Invalid token: userId not found.";
                    return RedirectToAction("LoginView", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Получаем данные пользователя
                var user = await _httpClient.GetFromJsonAsync<UserModel>($"/api/user/{int.Parse(userId)}");
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Получаем профиль агента, если есть
                AgentProfileModel? agentProfile = null;
                if (user.Role == "Agent")
                {
                    agentProfile = await _httpClient.GetFromJsonAsync<AgentProfileModel>($"/api/profile/agentProfile/{userId}");

                    profileId = agentProfile.Id;

                }

                var viewModel = new AgentProfileViewModel
                {
                    User = user,
                    agentProfile = agentProfile
                    
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(AgentProfileViewModel model)
        {
            try
            {
                

                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "You must be logged in to edit your profile.";
                    return RedirectToAction("LoginView", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                
                // Обновление данных пользователя
                var userResponse = await _httpClient.PutAsJsonAsync($"/api/User/{userId}", model.User);
                if (!userResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to update user profile.";
                    return View(model);
                }
                
                // Обновление данных агента, если профиль существует
                if (model.agentProfile != null)
                {
                   

                    var agentResponse = await _httpClient.PutAsJsonAsync($"/api/profile/{model.agentProfile.Id}", model.agentProfile);
                    if (!agentResponse.IsSuccessStatusCode)
                    {
                        
                        TempData["Error"] = "Failed to update agent profile.";
                        return View(model);
                    }
                }
                
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
                return View(model);
            }
        }
    }
}