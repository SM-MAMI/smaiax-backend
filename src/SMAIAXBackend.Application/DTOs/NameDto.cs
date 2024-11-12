using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Application.DTOs;

public class NameDto(string firstName, string lastName)
{
    [Required]
    public string FirstName { get; set; } = firstName;

    [Required]
    public string LastName { get; set; } = lastName;

    public static NameDto FromName(Name name)
    {
        return new NameDto(name.FirstName, name.LastName);
    }
}