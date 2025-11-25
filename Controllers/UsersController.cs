using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradProject.Data;
using GradProject.Models;

namespace GradProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get user data by user ID - Returns patient data if user is a patient, or doctor data if user is a doctor
    /// </summary>
    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> GetUserById(int userId)
    {
        // First check if user exists
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        // Check if user is a patient
        if (user.Role == "Patient")
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Select(p => new PatientResponse
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Email = p.User.Email,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Gender = p.Gender,
                    PhoneNumber = p.PhoneNumber,
                    DateOfBirth = p.DateOfBirth,
                    Address = p.Address,
                    CreatedAt = p.User.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound(new { message = $"Patient data not found for user ID {userId}" });

            return Ok(new
            {
                userType = "Patient",
                data = patient
            });
        }

        // Check if user is a doctor
        if (user.Role == "Doctor")
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.UserId == userId)
                .Select(d => new DoctorResponse
                {
                    Id = d.Id,
                    UserId = d.UserId,
                    Email = d.User.Email,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Gender = d.Gender,
                    PhoneNumber = d.PhoneNumber,
                    Specialization = d.Specialization,
                    LicenseNumber = d.LicenseNumber,
                    Hospital = d.Hospital,
                    CreatedAt = d.User.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (doctor == null)
                return NotFound(new { message = $"Doctor data not found for user ID {userId}" });

            return Ok(new
            {
                userType = "Doctor",
                data = doctor
            });
        }

        return BadRequest(new { message = $"Unknown user role: {user.Role}" });
    }
}

