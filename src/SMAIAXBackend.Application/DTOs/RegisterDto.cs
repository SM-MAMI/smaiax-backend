using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class RegisterDto(
    string email,
    string password,
    string firstName,
    string lastName,
    string street,
    string city,
    string state,
    string zipCode,
    string country)
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = email;

    [Required]
    public string Password { get; set; } = password;

    [Required]
    public string FirstName { get; set; } = firstName;

    [Required]
    public string LastName { get; set; } = lastName;

    [Required]
    public string Street { get; set; } = street;

    [Required]
    public string City { get; set; } = city;

    [Required]
    public string State { get; set; } = state;

    [Required]
    public string ZipCode { get; set; } = zipCode;

    [Required]
    public string Country { get; set; } = country;
}