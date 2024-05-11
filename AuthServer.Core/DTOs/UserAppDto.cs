using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
  public class UserAppDto
  {
    public int Id { get; set; }
    public  required string UserName { get; set; }
    public required string Email { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
  }
}
