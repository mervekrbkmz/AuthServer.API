using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniAuth2.API.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class InvoiceController : Controller
  {
    [HttpGet]
    public IActionResult GetInvoices()
    {
      var userName = HttpContext.User.Identity.Name; // endpointe istek yaptığımda isteğin tokenın payloadından name direkt alır
      var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
      var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

      return Ok($"UserName:{userName}-UserId:{userId}- Email:{email}");
    }
  }
}
