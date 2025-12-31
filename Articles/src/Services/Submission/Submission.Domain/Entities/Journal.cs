using BuildingBlocks.Domain.Entities;

namespace Submission.Domain.Entities;

public partial class Journal : IEntity
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Abreviation { get; set; }

    private readonly List<Article> _articles = new();
    public IReadOnlyCollection<Article> Articles => _articles.AsReadOnly();
}