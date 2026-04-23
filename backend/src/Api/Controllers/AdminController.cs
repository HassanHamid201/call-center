using Api.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AuthDbContext _authContext;
    private readonly MedicalDbContext _medicalContext;

    public AdminController(AuthDbContext authContext, MedicalDbContext medicalContext)
    {
        _authContext = authContext;
        _medicalContext = medicalContext;
    }

    /// <summary>
    /// Gets admin dashboard statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetStats()
    {
        var users = await _authContext.Users.CountAsync();
        var activeUsers = await _authContext.Users.CountAsync(u => u.IsActive);
        var branches = await _medicalContext.Branches.CountAsync();
        var doctors = await _medicalContext.Doctors.CountAsync();
        var specialties = await _medicalContext.Specialties.CountAsync();
        var sectors = await _medicalContext.Sectors.CountAsync();
        var auditLogs = await _authContext.AuditLogs.CountAsync();
        var faqItems = await _medicalContext.FaqItems.CountAsync(f => f.IsActive);

        return Ok(new
        {
            users,
            activeUsers,
            branches,
            doctors,
            specialties,
            sectors,
            auditLogs,
            faqItems
        });
    }

    /// <summary>
    /// Lists all users with optional filtering
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsers(
        [FromQuery] string? email,
        [FromQuery] string? role,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _authContext.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email.Contains(email));

        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, out var userRole))
            query = query.Where(u => u.Role == userRole);

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role.ToString(),
                BranchId = u.BranchId
            })
            .ToListAsync();

        return Ok(new PagedResult<UserDto>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    /// <summary>
    /// Gets a single user by ID
    /// </summary>
    [HttpGet("users/{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _authContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return Problem(title: "Not found", detail: "User not found", statusCode: StatusCodes.Status404NotFound);

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            BranchId = user.BranchId
        });
    }

    /// <summary>
    /// Updates a user's role and status
    /// </summary>
    [HttpPut("users/{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _authContext.Users.FindAsync(id);
        if (user == null)
            return Problem(title: "Not found", detail: "User not found", statusCode: StatusCodes.Status404NotFound);

        if (!string.IsNullOrWhiteSpace(request.Role) && Enum.TryParse<UserRole>(request.Role, out var role))
            user.Role = role;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        if (request.BranchId.HasValue)
            user.BranchId = request.BranchId.Value;
        else if (request.ClearBranch == true)
            user.BranchId = null;

        await _authContext.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            BranchId = user.BranchId
        });
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    [HttpDelete("users/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _authContext.Users.FindAsync(id);
        if (user == null)
            return Problem(title: "Not found", detail: "User not found", statusCode: StatusCodes.Status404NotFound);

        _authContext.Users.Remove(user);
        await _authContext.SaveChangesAsync();

        return NoContent();
    }
}

public record UpdateUserRequest
{
    public string? Role { get; init; }
    public bool? IsActive { get; init; }
    public Guid? BranchId { get; init; }
    public bool? ClearBranch { get; init; }
}
