using Api.DTOs;
using Domain.Entities;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class BranchesController : ControllerBase
{
    private readonly MedicalDbContext _context;
    private readonly ILogger<BranchesController> _logger;

    public BranchesController(MedicalDbContext context, ILogger<BranchesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lists all branches with optional filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BranchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BranchDto>>> GetAll(
        [FromQuery] string? city,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Branches.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(b => b.City.Contains(city));

        if (isActive.HasValue)
            query = query.Where(b => b.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BranchDto(
                b.Id,
                b.Name,
                b.Address,
                b.City,
                b.Phone,
                b.Email,
                b.Latitude,
                b.Longitude,
                b.IsActive,
                b.Services,
                b.CreatedAt,
                b.Doctors.Count))
            .ToListAsync();

        return Ok(new PagedResult<BranchDto>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    /// <summary>
    /// Gets a single branch by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BranchDto>> GetById(Guid id)
    {
        var branch = await _context.Branches
            .AsNoTracking()
            .Include(b => b.Doctors)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null)
            return Problem(title: "Not found", detail: "Branch not found", statusCode: StatusCodes.Status404NotFound);

        return Ok(new BranchDto(
            branch.Id, branch.Name, branch.Address, branch.City,
            branch.Phone, branch.Email, branch.Latitude, branch.Longitude,
            branch.IsActive, branch.Services, branch.CreatedAt, branch.Doctors.Count));
    }

    /// <summary>
    /// Creates a new branch
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<BranchDto>> Create([FromBody] CreateBranchRequest request)
    {
        var branch = new Branch
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Phone = request.Phone,
            Email = request.Email,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Services = request.Services ?? new List<string>()
        };

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = branch.Id }, new BranchDto(
            branch.Id, branch.Name, branch.Address, branch.City,
            branch.Phone, branch.Email, branch.Latitude, branch.Longitude,
            branch.IsActive, branch.Services, branch.CreatedAt, 0));
    }

    /// <summary>
    /// Updates an existing branch
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BranchDto>> Update(Guid id, [FromBody] UpdateBranchRequest request)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch == null)
            return Problem(title: "Not found", detail: "Branch not found", statusCode: StatusCodes.Status404NotFound);

        if (request.Name != null) branch.Name = request.Name;
        if (request.Address != null) branch.Address = request.Address;
        if (request.City != null) branch.City = request.City;
        if (request.Phone != null) branch.Phone = request.Phone;
        if (request.Email != null) branch.Email = request.Email;
        if (request.Latitude.HasValue) branch.Latitude = request.Latitude;
        if (request.Longitude.HasValue) branch.Longitude = request.Longitude;
        if (request.Services != null) branch.Services = request.Services;
        if (request.IsActive.HasValue) branch.IsActive = request.IsActive.Value;
        branch.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new BranchDto(
            branch.Id, branch.Name, branch.Address, branch.City,
            branch.Phone, branch.Email, branch.Latitude, branch.Longitude,
            branch.IsActive, branch.Services, branch.CreatedAt, branch.Doctors.Count));
    }

    /// <summary>
    /// Deletes a branch
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch == null)
            return Problem(title: "Not found", detail: "Branch not found", statusCode: StatusCodes.Status404NotFound);

        _context.Branches.Remove(branch);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
