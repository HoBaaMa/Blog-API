using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog_API.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected string GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) throw new UnauthorizedAccessException("User ID is not found in claims.");
            return userId;
        }
    }
}
