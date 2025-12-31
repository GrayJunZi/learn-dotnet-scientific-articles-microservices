using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Core.FluentValidation;
using Microsoft.AspNetCore.Http;
using Submission.Domain.Enums;

namespace Submission.Application.Features.UploadFile.UploadManuscriptFile;

public record UploadManuscriptFileCommand : ArticleCommand
{
    /// <summary>
    /// 资产类型
    /// </summary>
    [Required]
    public AssetType AssetType { get; init; }

    /// <summary>
    /// 上传文件
    /// </summary>
    [Required]
    public IFormFile File { get; init; }

    public override ArticleActionType ActionType => ArticleActionType.Upload;
}

public class UploadManuscriptFileCommandValidator : ArticleCommandValidator<UploadManuscriptFileCommand>
{
    public UploadManuscriptFileCommandValidator()
    {
        RuleFor(x => x.File)
            .WithMessageForNotNull();

        RuleFor(x => x.AssetType).Must(IsAssetTypeAllowed)
            .WithMessage(x => $"{x.AssetType} is not allowed");
    }

    public IReadOnlyCollection<AssetType> AllowedAssetTypes => new HashSet<AssetType>
    {
        AssetType.Manuscript
    };

    private bool IsAssetTypeAllowed(AssetType assetType)
        => AllowedAssetTypes.Contains(assetType);
}