using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class LoginDto(string username, string password)
{
    [Required]
    public string Username { get; set; } = username;

    [Required]
    public string Password { get; set; } = password;
}