﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Model
{
  public class UserRefreshToken1
  {
        public string UserId { get; set; }
        public string Code { get; set; } //refreshtoken
        public DateTime Expiration { get; set; }//token time
    }
}
