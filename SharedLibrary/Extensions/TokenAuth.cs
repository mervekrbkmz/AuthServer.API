using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
  public static class TokenAuth
  {

    //program.cs de çağıracağımız için oluşturulan extensions-general kullanılır
    public static void AddTokenAuth(this IServiceCollection services, CustomTokenOptions tokenOptions)
    {

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
      {
        opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
          ValidIssuer = tokenOptions.Issuer,
          ValidAudience = tokenOptions.Audience[0],
          IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
          ValidateIssuerSigningKey = true,

          ValidateAudience = true,
          ValidateIssuer = true,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero 
        };
      });
    }
  }
}
