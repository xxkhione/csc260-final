using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Services;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //Basic Gets and Posts in preparation for the migration of concerns.

        [HttpPost("login")]
        public void Post()
        {

        }

        [HttpPost("register")]
        public void PostRegistration()
        {

        }
    }
}
