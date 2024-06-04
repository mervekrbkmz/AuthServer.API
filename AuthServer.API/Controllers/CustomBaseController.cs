using Microsoft.AspNetCore.Mvc;
using SharedLibrary;

namespace AuthServer.API.Controllers
{
  public class CustomBaseController : ControllerBase
  {

    public IActionResult ActionResultInstance<T>(Response<T> response) where T : class
    {
      return new OkObjectResult(response)
      {
        StatusCode = response.StatusCode

      };

    }
  }
}
