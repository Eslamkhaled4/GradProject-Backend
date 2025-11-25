using Microsoft.AspNetCore.Mvc;
using GradProject.Models;
using GradProject.Services;

namespace GradProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login as a patient
    /// </summary>
    [HttpPost("login/patient")]
    public async Task<ActionResult<AuthResponse>> LoginPatient([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.LoginAsync(request.Email, request.Password, "Patient");

        if (response == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(response);
    }

    /// <summary>
    /// Login as a doctor
    /// </summary>
    [HttpPost("login/doctor")]
    public async Task<ActionResult<AuthResponse>> LoginDoctor([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.LoginAsync(request.Email, request.Password, "Doctor");

        if (response == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(response);
    }

    /// <summary>
    /// Register as a patient
    /// </summary>
    [HttpPost("register/patient")]
    public async Task<ActionResult<AuthResponse>> RegisterPatient([FromBody] RegisterPatientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.RegisterPatientAsync(request);

        if (response == null)
            return Conflict(new { message = "Email already exists" });
        return CreatedAtAction(nameof(RegisterPatient), response);
    }

    /// <summary>
    /// Register as a doctor
    /// </summary>
    [HttpPost("register/doctor")]
    public async Task<ActionResult<AuthResponse>> RegisterDoctor([FromBody] RegisterDoctorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.RegisterDoctorAsync(request);

        if (response == null)
            return Conflict(new { message = "Email already exists" });

        return CreatedAtAction(nameof(RegisterDoctor), response);
    }
}

