namespace GradProject.Models;

public class Doctor
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Hospital { get; set; }
}

