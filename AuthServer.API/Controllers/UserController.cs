using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : CustomBaseController
  {
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
      _userService = userService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
    {
      var user = await _userService.CreateUserAsync(createUserDto);
      return ActionResultInstance(user);
    }
    [Authorize] //bu endpoint mutlaka bir token istiyor demek
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
      var user = await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name); //framework name claims bulur.User.IDENTITY propertiesden alır.
      return ActionResultInstance(user);
    }

  }
}
