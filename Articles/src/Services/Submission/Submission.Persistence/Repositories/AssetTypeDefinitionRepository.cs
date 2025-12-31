using Articles.Abstractions.Enums;
using BuildingBlocks.EntityFramework;
using Microsoft.Extensions.Caching.Memory;
using Submission.Domain.Entities;

namespace Submission.Persistence.Repositories;

public class AssetTypeDefinitionRepository(SubmissionDbContext dbContext, IMemoryCache memoryCache)
    : CachedRepository<SubmissionDbContext, AssetTypeDefinition, AssetType>(dbContext, memoryCache)
{
    
}