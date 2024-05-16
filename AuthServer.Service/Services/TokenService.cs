using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
  class TokenService : ITokenService
  {
    private readonly UserManager<UserApp> _userManager;

    private readonly CustomTokenOptions _customTokenOptions;

    public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options)
    {
      _userManager = userManager;
      _customTokenOptions = options.Value; //yukarıdaki generic olarak tanımlandı direkt değerini alabiliriz
    }

    private string CreateRefreshToken()
    {
      var numbByte = new Byte[32];
      using var rnd = RandomNumberGenerator.Create(); // RANDOM DEĞER ÜRETİLİCEK

      rnd.GetBytes(numbByte);

      return Convert.ToBase64String(numbByte);

    }

    private IEnumerable<Claim> GetClaim(UserApp userApp, List<String> auidences)
    {
      //payloadda görünecekler
      var userList = new List<Claim> {
        new Claim(ClaimTypes.NameIdentifier,userApp.Id),
        new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
        new Claim(ClaimTypes.Name,userApp.UserName),
        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), //jti: jsonı kimliklendiricek, itendidty veriyor
        
        };

      userList.AddRange(auidences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x))); //tokena bakıcak oauideince bakıcak token geçerliysebu api için, hangi api olduğunu anlamak 

      return userList;
    }
    public TokenDto CreateToken(UserApp userApp)
    {
      throw new NotImplementedException();
    }

    public ClientTokenDto CreateTokenByClient(UserApp userApp)
    {
      throw new NotImplementedException();
    }
  }
}
