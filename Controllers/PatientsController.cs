using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradProject.Data;
using GradProject.Models;

namespace GradProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(ApplicationDbContext context, ILogger<PatientsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all patients
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PatientResponse>>> GetAllPatients()
    {
        var patients = await _context.Patients
            .Include(p => p.User)
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
            .ToListAsync();

        return Ok(patients);
    }
}

