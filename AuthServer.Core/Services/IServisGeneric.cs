using SharedLibrary;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
  public interface IServisGeneric<TEntity, TDto> where TEntity : class where TDto : class //sadece class olma artı verdim
  {
    Task<Response<TDto>> GetByIdAsync(int id);
    Task<Response<IQueryable<TDto>>>GetAllAsync();
    Task<Response<IQueryable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);

    Task AddAsync(TDto dto);  

    //bestpracticese gör remove ve update bir değer döndürülmemesi gerekir. Burda döndürdüm ama dönmemesi daha iyi olur
    Task<Response<NoDataDto>> Remove(TDto dto);

    Task<Response<NoDataDto>> Update(TDto dto);

  }
}
