using AuthServer.Core.DTOs;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
  public interface IUserService
  {
    //presantationlayer ile haberleşicek

    Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);

    Task<Response<UserAppDto>> GetUserByNameAsync(string userName);

  }
}
