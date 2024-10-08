﻿using AuthServer.Core.DTOs;
using AuthServer.Core.Model;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AuthServer.API.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class ProductController : CustomBaseController
  {
   
    private readonly IGenericService<Product, ProductDto> _productService;
    public ProductController(IGenericService<Product, ProductDto> productService)
    {
      _productService = productService;
    }
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
      return ActionResultInstance(await _productService.GetAllAsync());
    }
    [HttpPost]
    public async Task<IActionResult> SaveProduct(ProductDto productDto)
    {
      return ActionResultInstance(await _productService.AddAsync(productDto));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct(ProductDto productDto)
    {
      return ActionResultInstance(await _productService.Update(productDto, productDto.Id));
    }
    //api/product?id=2 gibi..
    [HttpDelete("{id}")] //default valuetype--querystringden geleceği için Urlden almak istedim.
    public async Task<IActionResult> DeleteProduct(int id)
    {
      return ActionResultInstance(await _productService.Remove(id));
    }
  }
}
