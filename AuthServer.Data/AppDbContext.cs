using AuthServer.Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
  public class AppDbContext : IdentityDbContext<UserApp, IdentityRole, string> //kullanıcıyla ilgili işlemlerle models klasörümün altındaki userapp modelime karşılık gelicek
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Product> Products { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.ApplyConfigurationsFromAssembly(GetType().Assembly);// data klasörünün altındaki dll gidip interface alıp ayarları ekler.
      base.OnModelCreating(builder);
    }
  }
}
