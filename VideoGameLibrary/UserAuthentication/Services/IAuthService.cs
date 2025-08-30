using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using UserAuthentication.Models.Responses;

namespace UserAuthentication.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<IdentityResult> RegisterAsync(RegisterRequest request);
    }
}
