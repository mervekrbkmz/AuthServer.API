﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
  public class CreateUserDto
  {
    //kullanıcının kayıt olması 
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    //public string City { get; set; }


  }
}
