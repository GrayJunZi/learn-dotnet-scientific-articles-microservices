using BuildingBlocks.EntityFramework;
using FileStorage.Contracts;

namespace Submission.Application.Features.UploadFile.UploadManuscriptFile;

public class UploadManuscriptFileCommandHandler(
    ArticleRepository articleRepository,
    AssetTypeDefinitionRepository assetTypeDefinitionRepository,
    IFileService fileService)
    : IRequestHandler<UploadManuscriptFileCommand, IdResponse>
{
    public async Task<IdResponse> Handle(UploadManuscriptFileCommand command, CancellationToken cancellationToken)
    {
        var article = await articleRepository.FindByIdOrThrowAsync(command.ArticleId);

        var assetType = assetTypeDefinitionRepository.GetById(command.AssetType);

        Asset asset = null;
        if (!assetType.AllowsMultipleAssets)
            asset = article.Assets.SingleOrDefault(x => x.Type == assetType.Id);

        if (asset is null)
            asset = article.CreateAsset(assetType);

        var filePath = asset.GenerateStorageFilePath(command.File.FileName);
        await fileService.UploadFileAsync(filePath, command.File,true,new Dictionary<string, string>
        {
            { "entity", nameof(Asset) },
            { "entityId", asset.Id.ToString() },
        });

        await articleRepository.SaveChangesAsync();

        return new IdResponse(asset.Id);
    }
}