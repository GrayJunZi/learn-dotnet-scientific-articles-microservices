namespace EmailService.Contracts;

public record EmailMessage(string Subject, Content Content, EmailAddress From, List<EmailAddress> To);

public record EmailAddress(string Name, string Address);

public record Content(ContentType Type, string Value);

public enum ContentType
{
    Text,
    Html,
}