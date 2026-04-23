using Api.DTOs;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Global search across doctors and branches
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> Search(
        [FromQuery] string? query,
        [FromQuery] string? city,
        [FromQuery] Guid? specialtyId,
        [FromQuery] Guid? sectorId,
        [FromQuery] Guid? branchId,
        [FromQuery] string? language,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var results = new List<SearchResultDto>();

        // Search doctors
        var doctorQuery = _context.Doctors
            .AsNoTracking()
            .Include(d => d.Branch)
            .Include(d => d.Specialty)
            .Include(d => d.Sector)
            .Where(d => d.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            doctorQuery = doctorQuery.Where(d =>
                d.FirstName.ToLower().Contains(q) ||
                d.LastName.ToLower().Contains(q) ||
                (d.Qualifications != null && d.Qualifications.ToLower().Contains(q)));
        }

        if (!string.IsNullOrWhiteSpace(city))
            doctorQuery = doctorQuery.Where(d => d.Branch != null && d.Branch.City.Contains(city));

        if (specialtyId.HasValue)
            doctorQuery = doctorQuery.Where(d => d.SpecialtyId == specialtyId.Value);

        if (sectorId.HasValue)
            doctorQuery = doctorQuery.Where(d => d.SectorId == sectorId.Value);

        if (branchId.HasValue)
            doctorQuery = doctorQuery.Where(d => d.BranchId == branchId.Value);

        if (!string.IsNullOrWhiteSpace(language))
            doctorQuery = doctorQuery.Where(d => d.Languages.Contains(language));

        var doctors = await doctorQuery
            .OrderBy(d => d.LastName)
            .Skip((page - 1) * pageSize / 2)
            .Take(pageSize / 2)
            .Select(d => new SearchResultDto(
                d.Id,
                "Doctor",
                $"Dr. {d.FirstName} {d.LastName}",
                d.Specialty != null ? d.Specialty.Name : "",
                d.Branch != null ? d.Branch.Name : "",
                $"/doctors/{d.Id}"))
            .ToListAsync();

        results.AddRange(doctors);

        // Search branches
        var branchQuery = _context.Branches
            .AsNoTracking()
            .Where(b => b.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            branchQuery = branchQuery.Where(b =>
                b.Name.ToLower().Contains(q) ||
                b.Address.ToLower().Contains(q));
        }

        if (!string.IsNullOrWhiteSpace(city))
            branchQuery = branchQuery.Where(b => b.City.Contains(city));

        var branches = await branchQuery
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize / 2)
            .Take(pageSize / 2)
            .Select(b => new SearchResultDto(
                b.Id,
                "Branch",
                b.Name,
                b.City,
                string.Join(", ", b.Services.Take(3)),
                $"/branches/{b.Id}"))
            .ToListAsync();

        results.AddRange(branches);

        var totalCount = await doctorQuery.CountAsync() + await branchQuery.CountAsync();
        var paged = results
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new PagedResult<SearchResultDto>(paged, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }
}
