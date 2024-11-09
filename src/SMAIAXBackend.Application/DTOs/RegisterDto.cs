using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class RegisterDto(
    string userName,
    string email,
    string password,
    NameDto name)
{
    [Required]
    public string UserName { get; set; } = userName;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = email;

    [Required]
    public string Password { get; set; } = password;

    [Required]
    public NameDto Name { get; set; } = name;
}