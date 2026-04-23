using Api.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class SpecialtiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SpecialtiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SpecialtyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SpecialtyDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.Specialties.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(s => s.Name.Contains(name));

        if (isActive.HasValue)
            query = query.Where(s => s.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SpecialtyDto(s.Id, s.Name, s.Description, s.IconUrl, s.IsActive, s.CreatedAt, s.Doctors.Count))
            .ToListAsync();

        return Ok(new PagedResult<SpecialtyDto>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SpecialtyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecialtyDto>> GetById(Guid id)
    {
        var specialty = await _context.Specialties
            .AsNoTracking()
            .Include(s => s.Doctors)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (specialty == null)
            return Problem(title: "Not found", detail: "Specialty not found", statusCode: StatusCodes.Status404NotFound);

        return Ok(new SpecialtyDto(specialty.Id, specialty.Name, specialty.Description, specialty.IconUrl,
            specialty.IsActive, specialty.CreatedAt, specialty.Doctors.Count));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(SpecialtyDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SpecialtyDto>> Create([FromBody] CreateSpecialtyRequest request)
    {
        var specialty = new Specialty
        {
            Name = request.Name,
            Description = request.Description,
            IconUrl = request.IconUrl
        };

        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = specialty.Id },
            new SpecialtyDto(specialty.Id, specialty.Name, specialty.Description, specialty.IconUrl, specialty.IsActive, specialty.CreatedAt, 0));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(SpecialtyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SpecialtyDto>> Update(Guid id, [FromBody] UpdateSpecialtyRequest request)
    {
        var specialty = await _context.Specialties.FindAsync(id);
        if (specialty == null)
            return Problem(title: "Not found", detail: "Specialty not found", statusCode: StatusCodes.Status404NotFound);

        if (request.Name != null) specialty.Name = request.Name;
        if (request.Description != null) specialty.Description = request.Description;
        if (request.IconUrl != null) specialty.IconUrl = request.IconUrl;
        if (request.IsActive.HasValue) specialty.IsActive = request.IsActive.Value;
        specialty.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new SpecialtyDto(specialty.Id, specialty.Name, specialty.Description, specialty.IconUrl,
            specialty.IsActive, specialty.CreatedAt, specialty.Doctors.Count));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var specialty = await _context.Specialties.FindAsync(id);
        if (specialty == null)
            return Problem(title: "Not found", detail: "Specialty not found", statusCode: StatusCodes.Status404NotFound);

        _context.Specialties.Remove(specialty);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
