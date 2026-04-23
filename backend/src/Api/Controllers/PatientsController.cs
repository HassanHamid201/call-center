using Api.DTOs;
using Domain.Entities.Patients;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly PatientDbContext _context;

    public PatientsController(PatientDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Patient>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Patient>>> GetAll(
        [FromQuery] string? phone,
        [FromQuery] string? fileNumber,
        [FromQuery] string? identityNumber,
        [FromQuery] string? name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Patients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(phone))
            query = query.Where(p => p.Phone != null && p.Phone.Contains(phone));

        if (!string.IsNullOrWhiteSpace(fileNumber))
            query = query.Where(p => p.FileNumber != null && p.FileNumber.Contains(fileNumber));

        if (!string.IsNullOrWhiteSpace(identityNumber))
            query = query.Where(p => p.IdentityNumber != null && p.IdentityNumber.Contains(identityNumber));

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.FullName.Contains(name));

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResult<Patient>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Patient>> GetById(Guid id)
    {
        var patient = await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null)
            return Problem(title: "Not found", detail: "Patient not found", statusCode: StatusCodes.Status404NotFound);

        return Ok(patient);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status201Created)]
    public async Task<ActionResult<Patient>> Create([FromBody] Patient request)
    {
        var patient = new Patient
        {
            FullName = request.FullName,
            Phone = request.Phone,
            IdentityNumber = request.IdentityNumber,
            FileNumber = request.FileNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Nationality = request.Nationality,
            Address = request.Address,
            Email = request.Email
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Patient>> Update(Guid id, [FromBody] Patient request)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            return Problem(title: "Not found", detail: "Patient not found", statusCode: StatusCodes.Status404NotFound);

        patient.FullName = request.FullName;
        patient.Phone = request.Phone;
        patient.IdentityNumber = request.IdentityNumber;
        patient.FileNumber = request.FileNumber;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.Nationality = request.Nationality;
        patient.Address = request.Address;
        patient.Email = request.Email;
        patient.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(patient);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            return Problem(title: "Not found", detail: "Patient not found", statusCode: StatusCodes.Status404NotFound);

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:guid}/appointments")]
    [ProducesResponseType(typeof(List<Appointment>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Appointment>>> GetAppointments(Guid id)
    {
        var items = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == id)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("{id:guid}/appointments")]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status201Created)]
    public async Task<ActionResult<Appointment>> CreateAppointment(Guid id, [FromBody] Appointment request)
    {
        var appointment = new Appointment
        {
            PatientId = id,
            DoctorId = request.DoctorId,
            BranchId = request.BranchId,
            AppointmentDate = request.AppointmentDate,
            Status = request.Status,
            Notes = request.Notes
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAppointments), new { id }, appointment);
    }

    [HttpPut("{id:guid}/appointments/{appointmentId:guid}")]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Appointment>> UpdateAppointment(Guid id, Guid appointmentId, [FromBody] Appointment request)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == id);
        if (appointment == null)
            return Problem(title: "Not found", detail: "Appointment not found", statusCode: StatusCodes.Status404NotFound);

        appointment.DoctorId = request.DoctorId;
        appointment.BranchId = request.BranchId;
        appointment.AppointmentDate = request.AppointmentDate;
        appointment.Status = request.Status;
        appointment.Notes = request.Notes;

        await _context.SaveChangesAsync();
        return Ok(appointment);
    }

    [HttpDelete("{id:guid}/appointments/{appointmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAppointment(Guid id, Guid appointmentId)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == id);
        if (appointment == null)
            return Problem(title: "Not found", detail: "Appointment not found", statusCode: StatusCodes.Status404NotFound);

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:guid}/visits")]
    [ProducesResponseType(typeof(List<Visit>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Visit>>> GetVisits(Guid id)
    {
        var items = await _context.Visits
            .AsNoTracking()
            .Where(v => v.PatientId == id)
            .OrderByDescending(v => v.VisitDate)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("{id:guid}/visits")]
    [ProducesResponseType(typeof(Visit), StatusCodes.Status201Created)]
    public async Task<ActionResult<Visit>> CreateVisit(Guid id, [FromBody] Visit request)
    {
        var visit = new Visit
        {
            PatientId = id,
            DoctorId = request.DoctorId,
            BranchId = request.BranchId,
            VisitDate = request.VisitDate,
            VisitType = request.VisitType,
            Diagnosis = request.Diagnosis,
            Treatment = request.Treatment,
            Notes = request.Notes
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVisits), new { id }, visit);
    }

    [HttpPut("{id:guid}/visits/{visitId:guid}")]
    [ProducesResponseType(typeof(Visit), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Visit>> UpdateVisit(Guid id, Guid visitId, [FromBody] Visit request)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.Id == visitId && v.PatientId == id);
        if (visit == null)
            return Problem(title: "Not found", detail: "Visit not found", statusCode: StatusCodes.Status404NotFound);

        visit.DoctorId = request.DoctorId;
        visit.BranchId = request.BranchId;
        visit.VisitDate = request.VisitDate;
        visit.VisitType = request.VisitType;
        visit.Diagnosis = request.Diagnosis;
        visit.Treatment = request.Treatment;
        visit.Notes = request.Notes;

        await _context.SaveChangesAsync();
        return Ok(visit);
    }

    [HttpDelete("{id:guid}/visits/{visitId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteVisit(Guid id, Guid visitId)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.Id == visitId && v.PatientId == id);
        if (visit == null)
            return Problem(title: "Not found", detail: "Visit not found", statusCode: StatusCodes.Status404NotFound);

        _context.Visits.Remove(visit);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:guid}/medical-history")]
    [ProducesResponseType(typeof(List<MedicalHistory>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MedicalHistory>>> GetMedicalHistory(Guid id)
    {
        var items = await _context.MedicalHistories
            .AsNoTracking()
            .Where(m => m.PatientId == id)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("{id:guid}/medical-history")]
    [ProducesResponseType(typeof(MedicalHistory), StatusCodes.Status201Created)]
    public async Task<ActionResult<MedicalHistory>> CreateMedicalHistory(Guid id, [FromBody] MedicalHistory request)
    {
        var history = new MedicalHistory
        {
            PatientId = id,
            Condition = request.Condition,
            DiagnosisDate = request.DiagnosisDate,
            Status = request.Status,
            Notes = request.Notes
        };

        _context.MedicalHistories.Add(history);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMedicalHistory), new { id }, history);
    }

    [HttpPut("{id:guid}/medical-history/{historyId:guid}")]
    [ProducesResponseType(typeof(MedicalHistory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicalHistory>> UpdateMedicalHistory(Guid id, Guid historyId, [FromBody] MedicalHistory request)
    {
        var history = await _context.MedicalHistories.FirstOrDefaultAsync(m => m.Id == historyId && m.PatientId == id);
        if (history == null)
            return Problem(title: "Not found", detail: "Medical history record not found", statusCode: StatusCodes.Status404NotFound);

        history.Condition = request.Condition;
        history.DiagnosisDate = request.DiagnosisDate;
        history.Status = request.Status;
        history.Notes = request.Notes;

        await _context.SaveChangesAsync();
        return Ok(history);
    }

    [HttpDelete("{id:guid}/medical-history/{historyId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMedicalHistory(Guid id, Guid historyId)
    {
        var history = await _context.MedicalHistories.FirstOrDefaultAsync(m => m.Id == historyId && m.PatientId == id);
        if (history == null)
            return Problem(title: "Not found", detail: "Medical history record not found", statusCode: StatusCodes.Status404NotFound);

        _context.MedicalHistories.Remove(history);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
