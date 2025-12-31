using Submission.Domain.ValueObjects;

namespace Submission.Domain.Entities;

public partial class Author
{
    public static Author Create(string emailAddress, string name, string? title, string affiliation)
    {
        var author = new Author
        {
            EmailAddress = EmailAddress.Create(emailAddress),
            Name = name,
            Title = title,
            Affiliation = affiliation
        };
        
        return author;
    }
}