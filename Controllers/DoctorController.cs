using System.Security.Claims;
using DairyFarm.Api.Data;
using DairyFarm.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DairyFarm.Api.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class DoctorController(AppDbContext db) : ControllerBase
{
    int Me => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? cowId)
    {
        var q = db.DoctorVisits.AsQueryable();
        if (cowId.HasValue) q = q.Where(v => v.CowId == cowId);
        var rows = await (from v in q
                          join c in db.Cows on v.CowId equals c.CowId
                          orderby v.VisitDate descending
                          select new { v.VisitId, v.CowId, c.TagNumber, v.VisitDate, v.DoctorName,
                                       v.Disease, v.Treatment, v.Cost, v.NextVisitDate }).ToListAsync();
        return Ok(rows);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> Upcoming()
    {
        var today = DateTime.Today; var end = today.AddDays(7);
        return Ok(await (from v in db.DoctorVisits
                         join c in db.Cows on v.CowId equals c.CowId
                         where v.NextVisitDate >= today && v.NextVisitDate <= end
                         select new { v.VisitId, c.TagNumber, v.DoctorName, v.NextVisitDate }).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(DoctorVisit v)
    {
        v.VisitId = 0; v.EnteredBy = Me;
        db.DoctorVisits.Add(v);
        await db.SaveChangesAsync();
        return Ok(v);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DoctorVisit input)
    {
        var v = await db.DoctorVisits.FindAsync(id);
        if (v is null) return NotFound();
        v.CowId = input.CowId; v.VisitDate = input.VisitDate; v.DoctorName = input.DoctorName;
        v.Disease = input.Disease; v.Treatment = input.Treatment; v.Cost = input.Cost;
        v.NextVisitDate = input.NextVisitDate;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var v = await db.DoctorVisits.FindAsync(id);
        if (v is null) return NotFound();
        db.DoctorVisits.Remove(v);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
