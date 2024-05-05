using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
  public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
  {

    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext appDbContext)
    {
      _dbContext = appDbContext;
      _dbSet = appDbContext.Set<TEntity>(); //dbcontextten set ettim
    }

    public async Task AddAsync(TEntity entity)
    {
      await _dbSet.AddAsync(entity); //memoriye bir tane entity eklendi.
                                     //uniotfwork kullandığım için burada direkti contexti savechanges etmedim. Service katmanında yapıcam
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
      return await _dbSet.ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
      var entity = await _dbSet.FindAsync(id);

      if (entity != null)
      {
        _dbContext.Entry(entity).State = EntityState.Detached;// veri memoryde tracklenmesin diye.Çünkü remove ve update..
      
      }
      return entity;
    }

    public void Remove(TEntity entity)
    {
      _dbSet.Remove(entity);
    }

    public TEntity Update(TEntity entity)
    {
      throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate)
    {
      throw new NotImplementedException();
    }
  }
}
