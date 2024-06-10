using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
  public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
  {

    private readonly IUnitOfWork _unitOfWork;

    private readonly IGenericRepository<TEntity> _repository;

    public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> repository)
    {
      _unitOfWork = unitOfWork;
      _repository = repository;
    }
    public async Task<Response<TDto>> AddAsync(TDto dto)
    {
    
      var entity = ObjectMapper.Mapper.Map<TEntity>(dto);

      await _repository.AddAsync(entity);

      _unitOfWork.CommitAsync(); //dbye gittim


      //dto nesnesine dönüştürdüm entityi
      var nDto = ObjectMapper.Mapper.Map<TDto>(entity);

      return Response<TDto>.Success(nDto, 200);
    }

    public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
    {
      var entities = await _repository.GetAllAsync();
      var dtoList = ObjectMapper.Mapper.Map<List<TDto>>(entities);


      // Başarılı bir yanıt döndür
      return Response<IEnumerable<TDto>>.Success(dtoList, 200);
    }

    public async Task<Response<TDto>> GetByIdAsync(int id)
    {
      var product = await _repository.GetByIdAsync(id);
      if (product == null)
      { 
        return Response<TDto>.Fail("Id not found", 404, true);
      }
      var productDto = ObjectMapper.Mapper.Map<TDto>(product);
      return Response<TDto>.Success(productDto, 200);
    }

    public async Task<Response<NoDataDto>> Remove(int id)
    {
      var isExistEntity = await _repository.GetByIdAsync(id);
      if (isExistEntity == null)
      {
        return Response<NoDataDto>.Fail("Id not found", 404, true);
      }
      _repository.Remove(isExistEntity);//ŞUAN Bellekte tutuyoruz
      await _unitOfWork.CommitAsync(); //dbye gitti artık burada

      return Response<NoDataDto>.Success(204);
    }

    public async Task<Response<NoDataDto>> Update(TDto dto, int id)
    {
      var dataExist = await _repository.GetByIdAsync(id);
      if (dataExist == null)
      {
        return Response<NoDataDto>.Fail("id not found", 404, true);
      }
      var updateEntity = ObjectMapper.Mapper.Map(dto, dataExist);//update memoryde bir ıd daha tutar, getbyid fonksiyonunda biz id detached ettikki memoryde boş yere tutmasın. update işleminde zaten mevcut. state alıcak.
      _repository.Update(updateEntity);

      await _unitOfWork.CommitAsync();
      return Response<NoDataDto>.Success(204); //content-bodyde data olmucak
    }

    public async Task<Response<IQueryable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
    {
      var query = _repository.Where(predicate)
                              .Skip(3)  // Skip the first 3 items
                              .Take(5); // Take the next 5 items

      var list = await query.ToListAsync();

      var dtoList = ObjectMapper.Mapper.Map<List<TDto>>(list).AsQueryable();

      return Response<IQueryable<TDto>>.Success(dtoList, 204);
    }

  }
}
