using AuthServer.Core.Configuration;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //appsettingjsona gidip

//DI REGISTER

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic olduðu için tiplerini belirttim o yüzde typeof yazarak register ettim.
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>)); //IGenericService iki tane generic içerdiði için <,> þeklinde kullandým o arttýkça , arttacaktýr.

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<AppDbContext>(options =>
{

  options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions => {

    sqlOptions.MigrationsAssembly("AuthServer.Data"); //migration iþlemleri data katmanýnda gerçekleþir bu yüzden assembly adýný verdim.Migration burada oluþacak.
  
  }); //db yolunu alacaktýr

});
builder.Services.AddIdentity<UserApp, IdentityRole>(opt => { 

opt.User.RequireUniqueEmail = true;
  opt.Password.RequireNonAlphanumeric = false; //*?= gibi characters


}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); // þifre sýfýrlama, e posta doðrulama vs iþlemlerde, default bir token üretir.otomaitk bu kullanýlýr.


var tokenOptionsSection = builder.Configuration.GetSection("TokenOptions"); //appsettingjson içeriðindeki json nesnesini getir
builder.Services.Configure<CustomTokenOptions>(tokenOptionsSection); //sonrada configure et
var clients = builder.Configuration.GetSection("Clients"); //appsettingjson içeriðindeki json nesnesini getir
builder.Services.Configure<List<Client>>(clients); //sonrada configure et

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
