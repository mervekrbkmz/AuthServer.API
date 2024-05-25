using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
  internal class DtoMapper:Profile
  {
    public DtoMapper()
    {
      CreateMap<ProductDto, Product>().ReverseMap(); //productdto nesnemi product dönüştrüp create daha sonra da reverse ettim yani tam tersi de olabilir
      CreateMap<UserAppDto, UserApp>().ReverseMap();
    }
  }
}
