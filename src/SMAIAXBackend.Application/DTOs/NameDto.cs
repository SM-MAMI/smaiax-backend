using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class NameDto(string firstName, string lastName)
{
    [Required]
    [RegularExpression(@"^\S+$", ErrorMessage = "First name cannot be empty or whitespace")]
    public string FirstName { get; set; } = firstName;

    [Required]
    [RegularExpression(@"^\S+$", ErrorMessage = "Last name cannot be empty or whitespace")]
    public string LastName { get; set; } = lastName;
}