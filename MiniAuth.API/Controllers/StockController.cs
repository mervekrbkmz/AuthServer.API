using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniAuth.API.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class StockController : Controller
  {
    [HttpGet]
    public IActionResult GetStock()
    {
      var userName = HttpContext.User.Identity.Name; // endpointe istek yaptığımda isteğin tokenın payloadından name direkt alır
      var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
      var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

      return Ok($"UserName:{userName}-UserId:{userIdClaim.Value}- Email:{email}");
    }
  }
}
