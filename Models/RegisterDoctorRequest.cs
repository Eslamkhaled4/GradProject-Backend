using System.ComponentModel.DataAnnotations;

namespace GradProject.Models;

public class RegisterDoctorRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Specialization { get; set; }

    public string? LicenseNumber { get; set; }

    public string? Hospital { get; set; }
}

