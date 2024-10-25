using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class SmartMeterCreateDto(string name)
{
    [Required]
    public string Name { get; set; } = name;
}