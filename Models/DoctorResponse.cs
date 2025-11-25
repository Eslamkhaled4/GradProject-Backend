namespace GradProject.Models;

public class DoctorResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Hospital { get; set; }
    public DateTime CreatedAt { get; set; }
}

