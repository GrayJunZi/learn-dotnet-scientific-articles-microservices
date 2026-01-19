namespace Auth.API.Features.Login;

public record LoginCommand(string Email, string Password);

public record LoginResponse(string Email, string AccessToken, string RefreshToken);