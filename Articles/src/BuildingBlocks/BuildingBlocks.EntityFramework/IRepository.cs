using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.EntityFramework;

public interface IRepository<TEntity>
    where TEntity : class, IEntity<int>
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> DeleteByIdAsync(int id);
    TEntity Update(TEntity entity);
    void Remove(TEntity entity);
}