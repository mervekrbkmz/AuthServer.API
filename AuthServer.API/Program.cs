using AuthServer.Core.Configuration;
using AuthServer.Core.Model;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data.Repositories;
using AuthServer.Data;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

//DI REGISTER
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));//IGenericService iki tane generic i�erdi�i i�in <,> �eklinde kulland�m o artt�k�a , arttacakt�r.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));//generic oldu�u i�in tiplerini belirttim o y�zde typeof yazarak register ettim.

builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions =>
  {
    sqlOptions.MigrationsAssembly("AuthServer.Data");//migration i�lemleri data katman�nda ger�ekle�ir bu y�zden assembly ad�n� verdim.Migration burada olu�acak.

  }); //db yolunu alacakt�r
});

builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
  options.User.RequireUniqueEmail = true;
  options.Password.RequireNonAlphanumeric = false; //*?= etc.. characters
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();// �ifre s�f�rlama, e posta do�rulama vs i�lemlerde, default bir token �retir.otomaitk bu kullan�l�r

builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption")); //appsettingjson i�eri�indeki json nesnesini getir
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));//sonrada configure et


builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //farkl� �yelik sistemleri oldu�unda schema kullan�r�z Ama� kimlik do�rulamalar farkl� olabilir cookie veya jwt olbilir. Jwt kullanaca��mdan �t�r� jwt tan�mlad�m
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//hangi kimlik do�rulama �emas�n� kullanaca��n� belirtmemizi bekler. 
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
  var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();
  opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
  {
    ValidIssuer = tokenOptions.Issuer,
    ValidAudience = tokenOptions.Audience[0],
    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
    ValidateIssuerSigningKey = true,

    ValidateAudience = true,
    ValidateIssuer = true,
    ValidateLifetime = true,//�mr�n� kontrol ediyoruz ge�erli bir token m� ge�ersiz mi gibi..
    ClockSkew = TimeSpan.Zero //tokena �m�r verildi�inde default ek 5 dk ekler. Farkl� serverlarda bu zaman fark� olu�turur. Ayn� zamanda gitsin diye 0 verdim.
  };
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer.API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthServer.API v1"));
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
