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
  public interface IGenericService<TEntity, TDto> where TEntity : class where TDto : class //sadece class olma artı verdim
  {
    Task<Response<TDto>> GetByIdAsync(int id);
    Task<Response<IEnumerable<TDto>>>GetAllAsync();
    Task<Response<IQueryable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);

    Task<Response<TDto>>AddAsync(TDto dto);  

    //bestpracticese gör remove ve update bir değer döndürülmemesi gerekir. Burda döndürdüm ama dönmemesi daha iyi olur
    Task<Response<NoDataDto>> Remove(int id);

    Task<Response<NoDataDto>> Update(TDto dto,int id);

  }
}
