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
public class DoctorsController : ControllerBase
{
    private readonly MedicalDbContext _context;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(MedicalDbContext context, ILogger<DoctorsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DoctorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DoctorDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? classification,
        [FromQuery] string? availabilityStatus,
        [FromQuery] Guid? specialtyId,
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? sectorId,
        [FromQuery] string? language,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Doctors
            .AsNoTracking()
            .Include(d => d.Branch)
            .Include(d => d.Specialty)
            .Include(d => d.Sector)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(d => (d.FirstName + " " + d.LastName).Contains(name) || (d.DisplayName != null && d.DisplayName.Contains(name)));

        if (!string.IsNullOrWhiteSpace(classification))
            query = query.Where(d => d.Classification == classification);

        if (!string.IsNullOrWhiteSpace(availabilityStatus))
            query = query.Where(d => d.AvailabilityStatus == availabilityStatus);

        if (specialtyId.HasValue)
            query = query.Where(d => d.SpecialtyId == specialtyId.Value);

        if (branchId.HasValue)
            query = query.Where(d => d.BranchId == branchId.Value);

        if (sectorId.HasValue)
            query = query.Where(d => d.SectorId == sectorId.Value);

        if (!string.IsNullOrWhiteSpace(language))
            query = query.Where(d => d.Languages.Contains(language));

        if (isActive.HasValue)
            query = query.Where(d => d.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => ToDto(d))
            .ToListAsync();

        return Ok(new PagedResult<DoctorDto>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    [HttpGet("classifications")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> GetClassifications()
    {
        var items = await _context.Doctors
            .AsNoTracking()
            .Where(d => d.Classification != null && d.Classification != "")
            .Select(d => d.Classification!)
            .Distinct()
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("availability-statuses")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> GetAvailabilityStatuses()
    {
        var items = await _context.Doctors
            .AsNoTracking()
            .Where(d => d.AvailabilityStatus != null && d.AvailabilityStatus != "")
            .Select(d => d.AvailabilityStatus!)
            .Distinct()
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDto>> GetById(Guid id)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Branch)
            .Include(d => d.Specialty)
            .Include(d => d.Sector)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null)
            return Problem(title: "Not found", detail: "Doctor not found", statusCode: StatusCodes.Status404NotFound);

        return Ok(ToDto(doctor));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DoctorDto>> Create([FromBody] CreateDoctorRequest request)
    {
        var doctor = new Doctor
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DisplayName = request.DisplayName,
            Email = request.Email,
            Phone = request.Phone,
            Mobile = request.Mobile,
            Languages = request.Languages ?? new List<string>(),
            Qualifications = request.Qualifications,
            Biography = request.Biography,
            Nationality = request.Nationality,
            Classification = request.Classification,
            InsuranceAcceptance = request.InsuranceAcceptance,
            InsuranceNotes = request.InsuranceNotes,
            AvailabilityStatus = request.AvailabilityStatus,
            CoordinatorName = request.CoordinatorName,
            InternalExtension = request.InternalExtension,
            WorkingHours = request.WorkingHours,
            WorkingDays = request.WorkingDays,
            AgeGroup = request.AgeGroup,
            ConsultationFee = request.ConsultationFee,
            Services = request.Services,
            ClinicMechanism = request.ClinicMechanism,
            Notes = request.Notes,
            BranchId = request.BranchId,
            SpecialtyId = request.SpecialtyId,
            SectorId = request.SectorId
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Branch)
            .Include(d => d.Specialty)
            .Include(d => d.Sector)
            .FirstAsync(d => d.Id == doctor.Id);

        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, ToDto(doctor));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDto>> Update(Guid id, [FromBody] UpdateDoctorRequest request)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return Problem(title: "Not found", detail: "Doctor not found", statusCode: StatusCodes.Status404NotFound);

        if (request.FirstName != null) doctor.FirstName = request.FirstName;
        if (request.LastName != null) doctor.LastName = request.LastName;
        if (request.DisplayName != null) doctor.DisplayName = request.DisplayName;
        if (request.Email != null) doctor.Email = request.Email;
        if (request.Phone != null) doctor.Phone = request.Phone;
        if (request.Mobile != null) doctor.Mobile = request.Mobile;
        if (request.Languages != null) doctor.Languages = request.Languages;
        if (request.Qualifications != null) doctor.Qualifications = request.Qualifications;
        if (request.Biography != null) doctor.Biography = request.Biography;
        if (request.Nationality != null) doctor.Nationality = request.Nationality;
        if (request.Classification != null) doctor.Classification = request.Classification;
        if (request.InsuranceAcceptance != null) doctor.InsuranceAcceptance = request.InsuranceAcceptance;
        if (request.InsuranceNotes != null) doctor.InsuranceNotes = request.InsuranceNotes;
        if (request.AvailabilityStatus != null) doctor.AvailabilityStatus = request.AvailabilityStatus;
        if (request.CoordinatorName != null) doctor.CoordinatorName = request.CoordinatorName;
        if (request.InternalExtension != null) doctor.InternalExtension = request.InternalExtension;
        if (request.WorkingHours != null) doctor.WorkingHours = request.WorkingHours;
        if (request.WorkingDays != null) doctor.WorkingDays = request.WorkingDays;
        if (request.AgeGroup != null) doctor.AgeGroup = request.AgeGroup;
        if (request.ConsultationFee.HasValue) doctor.ConsultationFee = request.ConsultationFee.Value;
        if (request.Services != null) doctor.Services = request.Services;
        if (request.ClinicMechanism != null) doctor.ClinicMechanism = request.ClinicMechanism;
        if (request.Notes != null) doctor.Notes = request.Notes;
        if (request.BranchId.HasValue) doctor.BranchId = request.BranchId.Value;
        if (request.SpecialtyId.HasValue) doctor.SpecialtyId = request.SpecialtyId.Value;
        if (request.SectorId.HasValue) doctor.SectorId = request.SectorId.Value;
        if (request.IsActive.HasValue) doctor.IsActive = request.IsActive.Value;
        doctor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Branch)
            .Include(d => d.Specialty)
            .Include(d => d.Sector)
            .FirstAsync(d => d.Id == doctor.Id);

        return Ok(ToDto(doctor));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return Problem(title: "Not found", detail: "Doctor not found", statusCode: StatusCodes.Status404NotFound);

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static DoctorDto ToDto(Doctor d) => new(
        d.Id,
        d.FirstName,
        d.LastName,
        d.DisplayName,
        d.FullName,
        d.Email,
        d.Phone,
        d.Mobile,
        d.Languages,
        d.Qualifications,
        d.Biography,
        d.IsActive,
        d.CreatedAt,
        d.Nationality,
        d.Classification,
        d.InsuranceAcceptance,
        d.InsuranceNotes,
        d.AvailabilityStatus,
        d.CoordinatorName,
        d.InternalExtension,
        d.WorkingHours,
        d.WorkingDays,
        d.AgeGroup,
        d.ConsultationFee,
        d.Services,
        d.ClinicMechanism,
        d.Notes,
        d.BranchId,
        d.Branch?.Name ?? "",
        d.SpecialtyId,
        d.Specialty?.Name ?? "",
        d.SectorId,
        d.Sector?.Name ?? ""
    );
}
