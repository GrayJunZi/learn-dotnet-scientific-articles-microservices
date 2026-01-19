using System.ComponentModel.DataAnnotations;

namespace EmailService.Smtp;

public class EmailOptions
{
    [Required]
    public string EmailServiceProvider { get; init; }
    [Required]
    public string EmailFromAddress { get; init; }
    [Required]
    public Smtp Smtp { get; set; }
}

public class Smtp
{
    [Required]
    public string Host { get; init; }
    [Required]
    public int Port { get; init; }
    [Required]
    public string Username { get; init; }
    [Required]
    public string Password { get; init; }
    public string DeliveryMethod { get; init; }
    public string PickupDirectoryLocation { get; init; }
    public bool UseSSL { get; set; } = true;    
}