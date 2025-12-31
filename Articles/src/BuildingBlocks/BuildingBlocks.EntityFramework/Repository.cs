using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFramework;

public class Repository<TContext, TEntity> : IRepository<TEntity>
    where TContext : DbContext
    where TEntity : class, IEntity<int>
{
    public readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public string TableName;

    public Repository(TContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();

        TableName = Context.Model.FindEntityType(typeof(TEntity)).GetTableName();
    }

    public virtual DbSet<TEntity> Entity => DbSet;
    protected virtual IQueryable<TEntity> Query() => DbSet;

    public virtual Task<TEntity?> GetByIdAsync(int id)
        => Query().SingleOrDefaultAsync(x => x.Id == id);

    public virtual async Task<TEntity> AddAsync(TEntity entity)
        => (await DbSet.AddAsync(entity)).Entity;

    public virtual async Task<bool> DeleteByIdAsync(int id)
    {
        return await Context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM {TableName} WHERE Id = {id}") > 0;
    }

    public virtual TEntity Update(TEntity entity)
        => DbSet.Update(entity).Entity;

    public virtual void Remove(TEntity entity)
        => DbSet.Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}