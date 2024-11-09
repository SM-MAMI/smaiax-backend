using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class LoginDto(string userName, string password)
{
    public string UserName { get; set; } = userName;

    [Required]
    public string Password { get; set; } = password;
}