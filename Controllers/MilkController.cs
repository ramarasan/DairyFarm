using System.Security.Claims;
using DairyFarm.Api.Data;
using DairyFarm.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DairyFarm.Api.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class MilkController(AppDbContext db) : ControllerBase
{
    int Me => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/milk?date=2026-10-09&shift=Morning
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime date, [FromQuery] string? shift)
    {
        var d = date.Date;
        var q = db.MilkRecords.Where(m => m.RecordDate == d);
        if (!string.IsNullOrEmpty(shift)) q = q.Where(m => m.Shift == shift);
        var rows = await (from m in q
                          join c in db.Cows on m.CowId equals c.CowId
                          select new { m.MilkRecordId, m.CowId, c.TagNumber, c.Name, m.RecordDate, m.Shift, m.Litres, m.Fat })
                         .ToListAsync();
        return Ok(rows);
    }

    // Bulk entry: one row per cow for a date + shift. Updates if already entered.
    [HttpPost("bulk")]
    public async Task<IActionResult> Bulk(List<MilkRecord> records)
    {
        foreach (var r in records)
        {
            var date = r.RecordDate.Date;
            var existing = await db.MilkRecords
                .FirstOrDefaultAsync(m => m.CowId == r.CowId && m.RecordDate == date && m.Shift == r.Shift);
            if (existing is null)
                db.MilkRecords.Add(new MilkRecord
                { CowId = r.CowId, RecordDate = date, Shift = r.Shift, Litres = r.Litres, Fat = r.Fat, EnteredBy = Me });
            else
            { existing.Litres = r.Litres; existing.Fat = r.Fat; }
        }
        await db.SaveChangesAsync();
        return Ok(new { message = $"{records.Count} records saved" });
    }

    // GET api/milk/summary?days=7  -> litres per day (for dashboard chart)
    [HttpGet("summary")]
    public async Task<IActionResult> Summary([FromQuery] int days = 7)
    {
        var from = DateTime.Today.AddDays(-(days - 1));
        var data = await db.MilkRecords.Where(m => m.RecordDate >= from)
            .GroupBy(m => m.RecordDate)
            .Select(g => new { date = g.Key, litres = g.Sum(x => x.Litres) })
            .OrderBy(x => x.date).ToListAsync();
        return Ok(data);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var m = await db.MilkRecords.FindAsync(id);
        if (m is null) return NotFound();
        db.MilkRecords.Remove(m);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
