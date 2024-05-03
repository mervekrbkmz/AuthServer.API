using AuthServer.Core.DTOs;
using SharedLibrary;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
  public interface IAuthenticationService
  {

    Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto); 
    Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken); 

    //refreshtoken sonlandırma-kullanıcı logout yapmak istediğinde-revoke:yetkinliğini kaldırmak
    //refreshtoken dan yeni token alabilir bu yüzden revoke kullanılabilir. datayı null atar
    Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken);

    Task<Response<ClientTokenDto>> CreateTokenByClientAsync(ClientLoginDto clientLoginDto);
  }
}
