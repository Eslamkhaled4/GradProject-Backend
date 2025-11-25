using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradProject.Data;
using GradProject.Models;

namespace GradProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(ApplicationDbContext context, ILogger<DoctorsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all doctors
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetAllDoctors()
    {
        var doctors = await _context.Doctors
            .Include(d => d.User)
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
            .ToListAsync();

        return Ok(doctors);
    }
}

