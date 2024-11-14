using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class SmartMeterUpdateDto(Guid id, string name)
{
    [Required]
    public Guid Id { get; set; } = id;
    [Required]
    [RegularExpression(@"^\S+$", ErrorMessage = "Smart meter name cannot be empty or whitespace")]
    public string Name { get; set; } = name;
}