using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace VideoGameLibrary.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [EmailAddress]
        [BindProperty]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        [BindProperty]
        public required string Password { get; set; }
        [BindProperty]
        public bool RememberMe { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var loginData = new { Email, Password };
            var client = _httpClientFactory.CreateClient("UserAuthenticationMicroservice");

            var response = await client.PostAsJsonAsync("api/auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(responseContent);
                string token = result.RootElement.GetProperty("token").GetString();

                // Decode the JWT to extract the claims
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

                var claims = new List<Claim> { new Claim("Token", token) };
                if (emailClaim != null)
                {
                    // Add the email claim to the identity for easy access
                    claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return LocalRedirect(Url.Content("~/"));
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
