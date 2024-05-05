using AuthServer.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Configurations
{
  public class UserAppConfiguration : IEntityTypeConfiguration<UserApp>
  {
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserApp> builder)
    {
      builder.Property(x=>x.City).HasMaxLength(50);
    }
  }
}
