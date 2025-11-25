using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GradProject.Data;
using GradProject.Models;

namespace GradProject.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password, string role)
    {
        // Query user from database with their profile data
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Role == role);

        if (user == null)
            return null;

        // Verify password against the saved hash in database
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        // Verify patient/doctor profile exists in database
        if (role == "Patient")
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (patient == null)
                return null; // User exists but patient profile is missing
        }
        else if (role == "Doctor")
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);
            if (doctor == null)
                return null; // User exists but doctor profile is missing
        }

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<AuthResponse?> RegisterPatientAsync(RegisterPatientRequest request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Patient",
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create patient profile with all the data from the request
        var patient = new Patient
        {
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Verify the data was saved by reloading from database
        var savedPatient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == user.Id);

        if (savedPatient == null)
            throw new Exception("Failed to save patient data to database");

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<AuthResponse?> RegisterDoctorAsync(RegisterDoctorRequest request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Doctor",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create doctor profile
        var doctor = new Doctor
        {
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            Specialization = request.Specialization,
            LicenseNumber = request.LicenseNumber,
            Hospital = request.Hospital
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "GradProject";
        var audience = jwtSettings["Audience"] ?? "GradProject";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "1440");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

