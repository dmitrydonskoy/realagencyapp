using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealAgencyClientApp.Models;
using System.Text;

namespace RealAgencyClientApp.Controllers
{
	public class AuthController : Controller
	{
		private readonly HttpClient _httpClient;

		public AuthController(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri("https://localhost:7023/api"); // Базовый URL внешнего API
		}

	
		public IActionResult RegisterView()
		{
			return View();
		}
		
		[HttpPost]
		public async Task<IActionResult> RegisterView(UserRegisterModel model)
		{
			
			if (!ModelState.IsValid) return View(model);

			var jsonContent = JsonConvert.SerializeObject(model);
			var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync("/api/auth/register", httpContent);

			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				ModelState.AddModelError(string.Empty, errorMessage);
				return View(model);
			}

			return RedirectToAction("LoginView");
		}

		
		public IActionResult LoginView()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> LoginView(UserLoginModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var jsonContent = JsonConvert.SerializeObject(model);
			var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync("/api/auth/login", httpContent);

			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				ModelState.AddModelError(string.Empty, errorMessage);
				return View(model);
			}

			var tokenResponse = await response.Content.ReadAsStringAsync();
			var token = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse)?.Token;

			if (token == null)
			{
				ModelState.AddModelError(string.Empty, "Ошибка получения токена.");
				return View(model);
			}

			Response.Cookies.Append("jwt", token, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			});

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult Logout()
		{
			Response.Cookies.Delete("jwt");
			return RedirectToAction("LoginView");
		}
	}

}
