using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class RegisterDto(
    string username,
    string email,
    string password,
    NameDto name)
{
    [Required]
    public string Username { get; set; } = username;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = email;

    [Required]
    public string Password { get; set; } = password;

    [Required]
    public NameDto Name { get; set; } = name;
}