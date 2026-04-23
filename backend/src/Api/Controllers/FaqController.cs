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
public class FaqController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FaqController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FaqItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FaqItem>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var q = _context.FaqItems
            .AsNoTracking()
            .Where(f => f.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            q = q.Where(f => f.Category == category);

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(f => EF.Functions.Like(f.Question, $"%{query}%") || EF.Functions.Like(f.Answer, $"%{query}%"));

        var totalCount = await q.CountAsync();
        var items = await q
            .OrderBy(f => f.Question)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResult<FaqItem>(items, totalCount, page, pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FaqItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FaqItem>> GetById(Guid id)
    {
        var item = await _context.FaqItems.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (item == null)
            return Problem(title: "Not found", detail: "FAQ item not found", statusCode: StatusCodes.Status404NotFound);
        return Ok(item);
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> GetCategories()
    {
        var items = await _context.FaqItems
            .AsNoTracking()
            .Where(f => f.IsActive && f.Category != null)
            .Select(f => f.Category!)
            .Distinct()
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(FaqItem), StatusCodes.Status201Created)]
    public async Task<ActionResult<FaqItem>> Create([FromBody] FaqItem request)
    {
        var item = new FaqItem
        {
            Question = request.Question,
            Answer = request.Answer,
            Category = request.Category ?? "عام",
            IsActive = true
        };
        _context.FaqItems.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(FaqItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FaqItem>> Update(Guid id, [FromBody] FaqItem request)
    {
        var item = await _context.FaqItems.FindAsync(id);
        if (item == null)
            return Problem(title: "Not found", detail: "FAQ item not found", statusCode: StatusCodes.Status404NotFound);

        item.Question = request.Question;
        item.Answer = request.Answer;
        item.Category = request.Category ?? item.Category;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _context.FaqItems.FindAsync(id);
        if (item == null)
            return Problem(title: "Not found", detail: "FAQ item not found", statusCode: StatusCodes.Status404NotFound);

        _context.FaqItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
