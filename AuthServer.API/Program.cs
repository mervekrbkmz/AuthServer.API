using AuthServer.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthServer.Core.Model;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configurations;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //appsettingjsona gidip

//DI REGISTER

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic oldu�u i�in tiplerini belirttim o y�zde typeof yazarak register ettim.
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>)); //IGenericService iki tane generic i�erdi�i i�in <,> �eklinde kulland�m o artt�k�a , arttacakt�r.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddDbContext<AppDbContext>(options =>
{

  options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions =>
  {

    sqlOptions.MigrationsAssembly("AuthServer.Data"); //migration i�lemleri data katman�nda ger�ekle�ir bu y�zden assembly ad�n� verdim.Migration burada olu�acak.

  }); //db yolunu alacakt�r

});
builder.Services.AddIdentity<UserApp, IdentityRole>(opt =>
{

  opt.User.RequireUniqueEmail = true;
  opt.Password.RequireNonAlphanumeric = false; //*?= etc.. characters


}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); // �ifre s�f�rlama, e posta do�rulama vs i�lemlerde, default bir token �retir.otomaitk bu kullan�l�r.


var tokenOptionsSection = builder.Configuration.GetSection("TokenOptions"); //appsettingjson i�eri�indeki json nesnesini getir
builder.Services.Configure<CustomTokenOptions>(tokenOptionsSection); //sonrada configure et


var clients = builder.Configuration.GetSection("Clients"); //appsettingjson i�eri�indeki json nesnesini getir
builder.Services.Configure<List<Client>>(clients); //sonrada configure et
builder.Services.AddAuthentication(options =>
{
  //�ema vermek zorunday�m
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //farkl� �yelik sistemleri oldu�unda schema kullan�r�z Ama� kimlik do�rulamalar farkl� olabilir cookie veya jwt olbilir. Jwt kullanaca��mdan �t�r� jwt tan�mlad�m�.
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//hangi kimlik do�rulama �emas�n� kullanaca��n� belirtmemizi bekler. 

}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
  var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
  options.TokenValidationParameters = new TokenValidationParameters
  {

    ValidIssuer = tokenOptions.Issuer,
    ValidAudience = tokenOptions.Audience[0],
    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.Issuer),



    //kontroller
    ValidateIssuerSigningKey = true, //imzal� d�rulama olmal�
    ValidateAudience = true,
    ValidateIssuer = true,
    ValidateLifetime = true, //�mr�n� kontrol ediyoruz ge�erli bir token m� ge�ersiz mi gibi..

    ClockSkew = TimeSpan.Zero //tokena �m�r verildi�inde default ek 5 dk ekler. Farkl� serverlarda bu zaman fark� olu�turur. Ayn� zamanda gitsin diye 0 verdim.








  };


});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
