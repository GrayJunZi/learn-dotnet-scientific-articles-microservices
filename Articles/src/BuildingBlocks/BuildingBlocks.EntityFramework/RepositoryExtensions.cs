using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFramework;

public static class RepositoryExtensions
{
    public static async Task<TEntity> FindByIdOrThrowAsync<TContext, TEntity>(
        this Repository<TContext, TEntity> repository, int id)
        where TContext : DbContext
        where TEntity : class, IEntity
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            throw new NotFoundException($"{typeof(TEntity).Name} not found");

        return entity;
    }

    public static async Task<TEntity> FindByIdOrThrowAsync<TEntity>(
        this DbSet<TEntity> dbSet, int id)
        where TEntity : class, IEntity
    {
        var entity = await dbSet.FindAsync(id);
        if (entity is null)
            throw new NotFoundException($"{typeof(TEntity).Name} not found");

        return entity;
    }
}