using Microsoft.AspNetCore.Mvc;

namespace UserAuthentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAuthenticationController : ControllerBase
    {
        private readonly ILogger<UserAuthenticationController> _logger;
        public UserAuthenticationController(ILogger<UserAuthenticationController> logger)
        {
            _logger = logger;
        }

        //Basic Gets and Posts in preparation for the migration of concerns.

        [HttpPost(Name = "PostUserLogin")]
        public void Post()
        {

        }

        [HttpGet(Name = "GetUserLogout")]
        public void Get()
        {

        }

        [HttpPost(Name = "PostUserRegistration")]
        public void PostRegistration()
        {

        }
    }
}
