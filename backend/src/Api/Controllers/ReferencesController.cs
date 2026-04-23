using Api.DTOs;
using Domain.Entities.Reference;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ReferencesController : ControllerBase
{
    private readonly ReferenceDbContext _context;

    public ReferencesController(ReferenceDbContext context)
    {
        _context = context;
    }

    private static readonly string[] ValidTypes =
    {
        "nationalities", "insurance-options", "classifications", "availability-statuses",
        "clinic-mechanisms", "coordinators", "working-hours", "working-days",
        "age-groups", "services"
    };

    private bool IsValidType(string type) => ValidTypes.Contains(type);

    [HttpGet("{type}")]
    [ProducesResponseType(typeof(List<ReferenceItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ReferenceItemDto>>> GetActive(string type)
    {
        if (!IsValidType(type))
            return Problem(title: "Invalid type", detail: $"Type '{type}' is not valid.", statusCode: StatusCodes.Status400BadRequest);

        var items = await GetQuery(type)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new ReferenceItemDto(x.Id, x.Name, x.IsActive, x.CreatedAt))
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{type}/all")]
    [ProducesResponseType(typeof(PagedResult<ReferenceItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ReferenceItemDto>>> GetAll(
        string type,
        [FromQuery] string? name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        if (!IsValidType(type))
            return Problem(title: "Invalid type", detail: $"Type '{type}' is not valid.", statusCode: StatusCodes.Status400BadRequest);

        var query = GetQuery(type).AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.Name.Contains(name));

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ReferenceItemDto(x.Id, x.Name, x.IsActive, x.CreatedAt))
            .ToListAsync();

        return Ok(new PagedResult<ReferenceItemDto>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    [HttpPost("{type}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ReferenceItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReferenceItemDto>> Create(string type, [FromBody] CreateReferenceRequest request)
    {
        if (!IsValidType(type))
            return Problem(title: "Invalid type", detail: $"Type '{type}' is not valid.", statusCode: StatusCodes.Status400BadRequest);

        if (string.IsNullOrWhiteSpace(request.Name))
            return Problem(title: "Validation failed", detail: "Name is required.", statusCode: StatusCodes.Status400BadRequest);

        var entity = CreateEntity(type, request.Name);
        AddEntity(type, entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetActive), new { type }, new ReferenceItemDto(entity.Id, entity.Name, entity.IsActive, entity.CreatedAt));
    }

    [HttpPut("{type}/{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ReferenceItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReferenceItemDto>> Update(string type, Guid id, [FromBody] UpdateReferenceRequest request)
    {
        if (!IsValidType(type))
            return Problem(title: "Invalid type", detail: $"Type '{type}' is not valid.", statusCode: StatusCodes.Status400BadRequest);

        var entity = await FindEntityAsync(type, id);
        if (entity == null)
            return Problem(title: "Not found", detail: $"Item not found in '{type}'.", statusCode: StatusCodes.Status404NotFound);

        if (request.Name != null) entity.Name = request.Name;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();
        return Ok(new ReferenceItemDto(entity.Id, entity.Name, entity.IsActive, entity.CreatedAt));
    }

    [HttpDelete("{type}/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string type, Guid id)
    {
        if (!IsValidType(type))
            return Problem(title: "Invalid type", detail: $"Type '{type}' is not valid.", statusCode: StatusCodes.Status400BadRequest);

        var entity = await FindEntityAsync(type, id);
        if (entity == null)
            return Problem(title: "Not found", detail: $"Item not found in '{type}'.", statusCode: StatusCodes.Status404NotFound);

        RemoveEntity(type, entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Helper methods

    private IQueryable<IReferenceEntity> GetQuery(string type)
    {
        return type switch
        {
            "nationalities" => _context.Nationalities.Cast<IReferenceEntity>(),
            "insurance-options" => _context.InsuranceOptions.Cast<IReferenceEntity>(),
            "classifications" => _context.Classifications.Cast<IReferenceEntity>(),
            "availability-statuses" => _context.AvailabilityStatuses.Cast<IReferenceEntity>(),
            "clinic-mechanisms" => _context.ClinicMechanisms.Cast<IReferenceEntity>(),
            "coordinators" => _context.Coordinators.Cast<IReferenceEntity>(),
            "working-hours" => _context.WorkingHours.Cast<IReferenceEntity>(),
            "working-days" => _context.WorkingDays.Cast<IReferenceEntity>(),
            "age-groups" => _context.AgeGroups.Cast<IReferenceEntity>(),
            "services" => _context.Services.Cast<IReferenceEntity>(),
            _ => throw new ArgumentException($"Invalid type: {type}", nameof(type))
        };
    }

    private IReferenceEntity CreateEntity(string type, string name)
    {
        return type switch
        {
            "nationalities" => new Nationality { Name = name },
            "insurance-options" => new InsuranceOption { Name = name },
            "classifications" => new Classification { Name = name },
            "availability-statuses" => new AvailabilityStatus { Name = name },
            "clinic-mechanisms" => new ClinicMechanism { Name = name },
            "coordinators" => new Coordinator { Name = name },
            "working-hours" => new WorkingHour { Name = name },
            "working-days" => new WorkingDay { Name = name },
            "age-groups" => new AgeGroup { Name = name },
            "services" => new ServiceCatalog { Name = name },
            _ => throw new ArgumentException($"Invalid type: {type}", nameof(type))
        };
    }

    private void AddEntity(string type, IReferenceEntity entity)
    {
        switch (type)
        {
            case "nationalities": _context.Nationalities.Add((Nationality)entity); break;
            case "insurance-options": _context.InsuranceOptions.Add((InsuranceOption)entity); break;
            case "classifications": _context.Classifications.Add((Classification)entity); break;
            case "availability-statuses": _context.AvailabilityStatuses.Add((AvailabilityStatus)entity); break;
            case "clinic-mechanisms": _context.ClinicMechanisms.Add((ClinicMechanism)entity); break;
            case "coordinators": _context.Coordinators.Add((Coordinator)entity); break;
            case "working-hours": _context.WorkingHours.Add((WorkingHour)entity); break;
            case "working-days": _context.WorkingDays.Add((WorkingDay)entity); break;
            case "age-groups": _context.AgeGroups.Add((AgeGroup)entity); break;
            case "services": _context.Services.Add((ServiceCatalog)entity); break;
        }
    }

    private async Task<IReferenceEntity?> FindEntityAsync(string type, Guid id)
    {
        return type switch
        {
            "nationalities" => await _context.Nationalities.FindAsync(id),
            "insurance-options" => await _context.InsuranceOptions.FindAsync(id),
            "classifications" => await _context.Classifications.FindAsync(id),
            "availability-statuses" => await _context.AvailabilityStatuses.FindAsync(id),
            "clinic-mechanisms" => await _context.ClinicMechanisms.FindAsync(id),
            "coordinators" => await _context.Coordinators.FindAsync(id),
            "working-hours" => await _context.WorkingHours.FindAsync(id),
            "working-days" => await _context.WorkingDays.FindAsync(id),
            "age-groups" => await _context.AgeGroups.FindAsync(id),
            "services" => await _context.Services.FindAsync(id),
            _ => null
        };
    }

    private void RemoveEntity(string type, IReferenceEntity entity)
    {
        switch (type)
        {
            case "nationalities": _context.Nationalities.Remove((Nationality)entity); break;
            case "insurance-options": _context.InsuranceOptions.Remove((InsuranceOption)entity); break;
            case "classifications": _context.Classifications.Remove((Classification)entity); break;
            case "availability-statuses": _context.AvailabilityStatuses.Remove((AvailabilityStatus)entity); break;
            case "clinic-mechanisms": _context.ClinicMechanisms.Remove((ClinicMechanism)entity); break;
            case "coordinators": _context.Coordinators.Remove((Coordinator)entity); break;
            case "working-hours": _context.WorkingHours.Remove((WorkingHour)entity); break;
            case "working-days": _context.WorkingDays.Remove((WorkingDay)entity); break;
            case "age-groups": _context.AgeGroups.Remove((AgeGroup)entity); break;
            case "services": _context.Services.Remove((ServiceCatalog)entity); break;
        }
    }
}

public record ReferenceItemDto(Guid Id, string Name, bool IsActive, DateTime CreatedAt);
public record CreateReferenceRequest(string Name);
public record UpdateReferenceRequest(string? Name, bool? IsActive);
