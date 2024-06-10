using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
  public class AuthenticationService : Core.Services.IAuthenticationService
    {
    private readonly List<Client> _clients;
    private readonly ITokenService _tokenService;
    private readonly UserManager<UserApp> _userManager; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

    public AuthenticationService(IOptions<List<Client>> options, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshToken)
    {
      _clients = options.Value;
      _tokenService = tokenService;
      _userManager = userManager;
      _unitOfWork = unitOfWork;
      _userRefreshTokenService = userRefreshToken;

    }

    public  async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
    {
      if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

      var user = await _userManager.FindByEmailAsync(loginDto.Email);
      if (user == null) return Response<TokenDto>.Fail("Email or Password is wrong", 400, true); //clien hatası olduğu için 400 döndüm, bendn kaynaklı hatalarda 500 dönüyorum.

      var pass = await _userManager.CheckPasswordAsync(user, loginDto.Password);
      if (!pass) return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);

      //kullanıcı mail pass bilgileri alındı token oluşturma işlemi

      var token = _tokenService.CreateToken(user);

      var refreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

      if (refreshToken == null) await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
      else { //dbden güncellicek ; önce entity memoryde tutuyor 
        refreshToken.Code = token.RefreshToken; refreshToken.Expiration=token.RefreshTokenExpiration;
      }

      await _unitOfWork.CommitAsync(); //burada ise dbye kaydediyor
      return Response<TokenDto>.Success(token, 200);

    }

    public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
    {
      var client = _clients.FirstOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);

      if (client == null) return Response<ClientTokenDto>.Fail("ClientId or SecretId No Found",404,true);

      var token = _tokenService.CreateTokenByClient(client);
      return Response<ClientTokenDto>.Success(token,200);
    }

    public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
    {
      var refrToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).FirstOrDefaultAsync();
      if(refrToken == null) return Response<TokenDto>.Fail("RefreshToken Not Found",400,true);

      var user = await _userManager.FindByIdAsync(refrToken.UserId);
      if (user==null) return Response<TokenDto>.Fail("UserId Not Found", 400, true);
     var tokeDto = _tokenService.CreateToken(user);

      refrToken.Code = tokeDto.RefreshToken ;
      refrToken.Expiration = tokeDto.RefreshTokenExpiration ;

      await _unitOfWork.CommitAsync();

      return Response<TokenDto>.Success(tokeDto,200);

    }

    public async Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken)
    {
      var refreshTok = await _userRefreshTokenService.Where(x => x.Code == refreshToken).FirstOrDefaultAsync();

      if (refreshTok == null) return Response<NoDataDto>.Fail("RefreshToken Not Found", 404, true);

      _userRefreshTokenService.Remove(refreshTok); //mempryde entityState remove olarak işaretlendi.

      await _unitOfWork.CommitAsync();

      return Response<NoDataDto>.Success(200);
    }
  }
}
