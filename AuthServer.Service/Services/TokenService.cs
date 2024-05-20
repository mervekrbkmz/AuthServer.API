using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

    //üyelik sistemi oluşturduğumda token almak için paylodda göstereceğim datalar
    private IEnumerable<Claim> GetClaims(UserApp userApp, List<String> auidences)
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

    //üyelik sistemi olmadan token oluşturmak istersem
    private IEnumerable<Claim> GetClaimsByClient(Client client)
    {
      var claims = new List<Claim>();
      claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

      new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());


        return claims;
    }

    public TokenDto CreateToken(UserApp userApp)
    {
      var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOptions.AccessTokenExpiration);
      var refreshTokenExpiration = DateTime.Now.AddMinutes(_customTokenOptions.RefreshTokenExpiration);
      var securityKey= SignService.GetSymmetricSecurityKey( _customTokenOptions.SecurityKey);

      SigningCredentials signing = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature);
      JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(issuer: _customTokenOptions.Issuer, expires: accessTokenExpiration, notBefore: DateTime.Now, claims: GetClaims(userApp, _customTokenOptions.Audience),
        signingCredentials: signing);
      //notbefore: benim verdiğim zamandan itibaren geçersiz olmasın-accessexp kısmında oluşucak

      var handler= new JwtSecurityTokenHandler();
      var token = handler.WriteToken(jwtSecurityToken);

      var tokenDto = new TokenDto
      {
        AccessToken = token,
        RefreshToken = CreateRefreshToken(),//string token oluşuyor, jwt 3 parçadan oluşur
        AccessTokenExpiration = accessTokenExpiration,
        RefreshTokenExpiration = refreshTokenExpiration
      };
      return tokenDto;


    }
    //üyelik sistemiyle ilgili bilgileri barındırmaz kendi bilgilerini barındırır.
    public ClientTokenDto CreateTokenByClient(Client client)
    {
      var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOptions.AccessTokenExpiration);
      var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOptions.SecurityKey);

      SigningCredentials signing = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
      JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(issuer: _customTokenOptions.Issuer, expires: accessTokenExpiration, notBefore: DateTime.Now, claims: GetClaimsByClient(client),
        signingCredentials: signing);
      //notbefore: benim verdiğim zamandan itibaren geçersiz olmasın-accessexp kısmında oluşucak

      var handler = new JwtSecurityTokenHandler();
      var token = handler.WriteToken(jwtSecurityToken);

      var clientTokenDto = new ClientTokenDto
      {
        AccessToken = token,
        AccessTokenExpiration = accessTokenExpiration,
      };
      return clientTokenDto;
    }
  }
}
