using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : CustomBaseController
  {
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserService _userService;

    public AuthController(IAuthenticationService authenticationService,IUserService userService)
    {
      _authenticationService = authenticationService;
      _userService= userService;
    }

    // POST api/auth/createtoken 
    // Kullanıcıdan gelen giriş bilgileriyle bir JWT oluşturur.
    [HttpPost("createtoken")]
    public async Task<IActionResult> CreateToken(LoginDto login)
    {
      var result = await _authenticationService.CreateTokenAsync(login);
      return ActionResultInstance(result);
    }

    // POST api/auth/createtokenbyclient 
    // İstemci giriş bilgileriyle bir JWT oluşturur.
    [HttpPost("createtokenbyclient")]
    public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
    {
      var result = _authenticationService.CreateTokenByClient(clientLoginDto);
      return ActionResultInstance(result);
    }

    // POST api/auth/revokerefreshtoken 
    // refresh token'ı geçersiz kılar.
    [HttpPost("revokerefreshtoken")]
    public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
    {
      var result = await _authenticationService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken);
      return ActionResultInstance(result);
    }

    // POST api/auth/createtokenbyrefreshtoken 
    // refreh token'ı kullanarak bir JWT oluşturur.
    [HttpPost("createtokenbyrefreshtoken")]
    public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
    {
      var result = await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.RefreshToken);
      return ActionResultInstance(result);
    }
  }
}
