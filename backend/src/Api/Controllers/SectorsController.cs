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
public class SectorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SectorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SectorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SectorDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.Sectors.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(s => s.Name.Contains(name));

        if (isActive.HasValue)
            query = query.Where(s => s.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SectorDto(s.Id, s.Name, s.Description, s.IsActive, s.CreatedAt, s.Doctors.Count))
            .ToListAsync();

        return Ok(new PagedResult<SectorDto>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SectorDto>> GetById(Guid id)
    {
        var sector = await _context.Sectors
            .AsNoTracking()
            .Include(s => s.Doctors)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sector == null)
            return Problem(title: "Not found", detail: "Sector not found", statusCode: StatusCodes.Status404NotFound);

        return Ok(new SectorDto(sector.Id, sector.Name, sector.Description, sector.IsActive, sector.CreatedAt, sector.Doctors.Count));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(SectorDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SectorDto>> Create([FromBody] CreateSectorRequest request)
    {
        var sector = new Sector { Name = request.Name, Description = request.Description };
        _context.Sectors.Add(sector);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sector.Id },
            new SectorDto(sector.Id, sector.Name, sector.Description, sector.IsActive, sector.CreatedAt, 0));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(SectorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SectorDto>> Update(Guid id, [FromBody] UpdateSectorRequest request)
    {
        var sector = await _context.Sectors.FindAsync(id);
        if (sector == null)
            return Problem(title: "Not found", detail: "Sector not found", statusCode: StatusCodes.Status404NotFound);

        if (request.Name != null) sector.Name = request.Name;
        if (request.Description != null) sector.Description = request.Description;
        if (request.IsActive.HasValue) sector.IsActive = request.IsActive.Value;
        sector.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new SectorDto(sector.Id, sector.Name, sector.Description, sector.IsActive, sector.CreatedAt, sector.Doctors.Count));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sector = await _context.Sectors.FindAsync(id);
        if (sector == null)
            return Problem(title: "Not found", detail: "Sector not found", statusCode: StatusCodes.Status404NotFound);

        _context.Sectors.Remove(sector);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
