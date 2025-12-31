using Articles.Abstractions.Enums;

namespace Submission.Domain.Entities;

public class ArticleActor
{
    public int ArticleId { get; init; }
    public Article Article { get; init; }
    public int PersonId { get; set; }
    public Person Person { get; init; }

    public UserRoleType Role { get; init; }
}