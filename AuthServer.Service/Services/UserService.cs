using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
  public class UserService : IUserService
  {

    private readonly UserManager<UserApp> _userManager;

    public UserService(UserManager<UserApp> userManager)
    {
      _userManager = userManager;
    }
    public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
     var user= new UserApp{ Email=createUserDto.Email,UserName=createUserDto.UserName };
      var result= await _userManager.CreateAsync(user,createUserDto.Password);

      if (!result.Succeeded)
      {
        var err= result.Errors.Select(x=>x.Description).ToList();

        return Response<UserAppDto>.Fail(new ErrorDto(err, true), 400);

      }
      return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);

    }

    public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
    {
     var user= await _userManager.FindByNameAsync(userName);
      if (user == null) return Response<UserAppDto>.Fail("UserName Not Found", 404, true);
      else
      {
        return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
      }
    }
  }
}
