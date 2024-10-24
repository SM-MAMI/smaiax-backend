using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class NameDto(string firstName, string lastName)
{
    [Required]
    public string FirstName { get; set; } = firstName;

    [Required]
    public string LastName { get; set; } = lastName;
}