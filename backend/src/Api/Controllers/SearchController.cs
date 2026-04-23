using Api.DTOs;
using Infrastructure.Data.Contexts;
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
    private readonly MedicalDbContext _context;

    public SearchController(MedicalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Global search across all entities
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
        [FromQuery] string? type,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var results = new List<SearchResultDto>();
        int totalCount = 0;

        // Search doctors
        if (string.IsNullOrWhiteSpace(type) || type == "Doctor")
        {
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
                    (d.DisplayName != null && d.DisplayName.ToLower().Contains(q)) ||
                    (d.Classification != null && d.Classification.ToLower().Contains(q)) ||
                    (d.Services != null && d.Services.ToLower().Contains(q)) ||
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

            totalCount += await doctorQuery.CountAsync();

            var doctors = await doctorQuery
                .OrderBy(d => d.LastName)
                .Take(pageSize)
                .Select(d => new SearchResultDto(
                    d.Id,
                    "Doctor",
                    d.DisplayName ?? $"Dr. {d.FirstName} {d.LastName}",
                    d.Specialty != null ? d.Specialty.Name : "",
                    d.Branch != null ? d.Branch.Name : "",
                    $"/doctors/{d.Id}"))
                .ToListAsync();

            results.AddRange(doctors);
        }

        // Search branches
        if (string.IsNullOrWhiteSpace(type) || type == "Branch")
        {
            var branchQuery = _context.Branches
                .AsNoTracking()
                .Where(b => b.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                branchQuery = branchQuery.Where(b =>
                    b.Name.ToLower().Contains(q) ||
                    b.Address.ToLower().Contains(q) ||
                    (b.City != null && b.City.ToLower().Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(city))
                branchQuery = branchQuery.Where(b => b.City.Contains(city));

            totalCount += await branchQuery.CountAsync();

            var branchResults = await branchQuery
                .OrderBy(b => b.Name)
                .Take(pageSize)
                .ToListAsync();

            var branches = branchResults
                .Select(b => new SearchResultDto(
                    b.Id,
                    "Branch",
                    b.Name,
                    b.City,
                    string.Join(", ", (b.Services ?? new List<string>()).Take(3)),
                    $"/branches/{b.Id}"))
                .ToList();

            results.AddRange(branches);
        }

        // Search specialties
        if (string.IsNullOrWhiteSpace(type) || type == "Specialty")
        {
            var specQuery = _context.Specialties
                .AsNoTracking()
                .Where(s => s.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                specQuery = specQuery.Where(s =>
                    s.Name.ToLower().Contains(q) ||
                    (s.Description != null && s.Description.ToLower().Contains(q)));
            }

            totalCount += await specQuery.CountAsync();

            var specialties = await specQuery
                .OrderBy(s => s.Name)
                .Take(pageSize)
                .Select(s => new SearchResultDto(
                    s.Id,
                    "Specialty",
                    s.Name,
                    s.Description ?? "",
                    "",
                    $"/specialties/{s.Id}"))
                .ToListAsync();

            results.AddRange(specialties);
        }

        // Search sectors
        if (string.IsNullOrWhiteSpace(type) || type == "Sector")
        {
            var sectorQuery = _context.Sectors
                .AsNoTracking()
                .Where(s => s.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                sectorQuery = sectorQuery.Where(s =>
                    s.Name.ToLower().Contains(q) ||
                    (s.Description != null && s.Description.ToLower().Contains(q)));
            }

            totalCount += await sectorQuery.CountAsync();

            var sectors = await sectorQuery
                .OrderBy(s => s.Name)
                .Take(pageSize)
                .Select(s => new SearchResultDto(
                    s.Id,
                    "Sector",
                    s.Name,
                    s.Description ?? "",
                    "",
                    $"/sectors/{s.Id}"))
                .ToListAsync();

            results.AddRange(sectors);
        }

        // Search FAQ
        if (string.IsNullOrWhiteSpace(type) || type == "Faq")
        {
            var faqQuery = _context.FaqItems
                .AsNoTracking()
                .Where(f => f.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                faqQuery = faqQuery.Where(f =>
                    f.Question.ToLower().Contains(q) ||
                    f.Answer.ToLower().Contains(q));
            }

            totalCount += await faqQuery.CountAsync();

            var faqs = await faqQuery
                .OrderBy(f => f.Question)
                .Take(pageSize)
                .Select(f => new SearchResultDto(
                    f.Id,
                    "FAQ",
                    f.Question,
                    f.Answer.Length > 60 ? f.Answer.Substring(0, 60) + "..." : f.Answer,
                    f.Category ?? "",
                    $"/faq"))
                .ToListAsync();

            results.AddRange(faqs);
        }

        var paged = results
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new PagedResult<SearchResultDto>(paged, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }
}
