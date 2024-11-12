using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.DTOs;

public class UserDto(NameDto name, string email, string username)
{
    public NameDto Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Username { get; set; } = username;
    
    public static UserDto FromUser(User user)
    {
        return new UserDto(NameDto.FromName(user.Name), user.Email, user.Username);
    }
}