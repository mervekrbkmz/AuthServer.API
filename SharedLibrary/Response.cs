using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary
{
  public class Response<T> where T : class
  {

    public T Data { get; private set; }

    public int StatusCode { get; private set; }


    //clientlere açmamak için kendi apida kullanmak için
    //json cıktımda görünmemesi için JsonIgnore kullandım.
    [JsonIgnore]
    public bool IsSuccessful { get; private set; }
    public ErrorDto Error { get; set; }
    public static Response<T> Success(T data, int statusCode)
    {
      return new Response<T> { Data = data, StatusCode = statusCode,IsSuccessful=true};
    }

    //data olmadanda başarılı mesaj döndürebilirim
    public static Response<T> Success(int statusCode)
    {
      return new Response<T> { Data = null, StatusCode = statusCode,IsSuccessful=true };

    }

    public static Response<T> Fail(ErrorDto errorDto, int statusCode)
    {
      return new Response<T> { Error = errorDto, StatusCode = statusCode,IsSuccessful=false};

    }
    //tek bir fail dönmek için
    public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
    {
      var error = new ErrorDto(errorMessage,isShow);
     
      return new Response<T> { Error = error, StatusCode = statusCode,IsSuccessful=false };

    }
  }
}

