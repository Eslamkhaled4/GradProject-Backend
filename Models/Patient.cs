namespace GradProject.Models;

public class Patient
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
}

