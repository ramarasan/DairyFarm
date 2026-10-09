using System.Security.Claims;
using DairyFarm.Api.Data;
using DairyFarm.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DairyFarm.Api.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class CowsController(AppDbContext db) : ControllerBase
{
    int Me => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/cows?type=New|Old&status=Active
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? type, [FromQuery] string? status)
    {
        var q = db.Cows.AsQueryable();
        if (!string.IsNullOrEmpty(type)) q = q.Where(c => c.CowType == type);
        if (!string.IsNullOrEmpty(status)) q = q.Where(c => c.Status == status);
        return Ok(await q.OrderBy(c => c.TagNumber).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var c = await db.Cows.FindAsync(id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpGet("count")]
    public async Task<IActionResult> Count() => Ok(new
    {
        total = await db.Cows.CountAsync(c => c.Status == "Active"),
        newCows = await db.Cows.CountAsync(c => c.Status == "Active" && c.CowType == "New"),
        oldCows = await db.Cows.CountAsync(c => c.Status == "Active" && c.CowType == "Old")
    });

    [HttpPost]
    public async Task<IActionResult> Create(Cow cow)
    {
        if (await db.Cows.AnyAsync(c => c.TagNumber == cow.TagNumber))
            return Conflict(new { message = "Tag number already exists" });
        cow.CowId = 0; cow.CreatedBy = Me; cow.CreatedAt = DateTime.Now;
        db.Cows.Add(cow);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = cow.CowId }, cow);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Cow input)
    {
        var c = await db.Cows.FindAsync(id);
        if (c is null) return NotFound();
        c.TagNumber = input.TagNumber; c.Name = input.Name; c.Breed = input.Breed;
        c.DateOfBirth = input.DateOfBirth; c.CowType = input.CowType;
        c.PurchaseDate = input.PurchaseDate; c.PurchasePrice = input.PurchasePrice;
        c.Status = input.Status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Cows.FindAsync(id);
        if (c is null) return NotFound();
        if (await db.MilkRecords.AnyAsync(m => m.CowId == id) || await db.DoctorVisits.AnyAsync(d => d.CowId == id))
        {   // keep history: mark as Sold instead of hard delete
            c.Status = "Sold";
            await db.SaveChangesAsync();
            return Ok(new { message = "Cow has records, status set to Sold" });
        }
        db.Cows.Remove(c);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
