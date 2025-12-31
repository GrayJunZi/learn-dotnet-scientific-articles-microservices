using BuildingBlocks.Core.Cache;
using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BuildingBlocks.EntityFramework;

public abstract class CachedRepository<TDbContext, TEntity, TId>(TDbContext dbContext, IMemoryCache cache)
    where TDbContext : DbContext
    where TEntity : class, IEntity<TId>, ICacheable
    where TId : struct
{
    public IEnumerable<TEntity> GetAll()
        => cache.GetOrCreateByType(entry => dbContext.Set<TEntity>().AsNoTracking().ToList());

    public TEntity GetById(TId id)
        => cache.GetOrCreate($"{typeof(TEntity).FullName}_{id}",
            entry => dbContext.Set<TEntity>().AsNoTracking().Single(x => x.Id.Equals(id)));
}