using DairyFarm.Api.Data;
using DairyFarm.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DairyFarm.Api.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class StockController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await db.StockItems.OrderBy(s => s.ItemName)
            .Select(s => new { s.ItemId, s.ItemName, s.Unit, s.CurrentQty, s.ReorderLevel,
                               isLow = s.CurrentQty <= s.ReorderLevel }).ToListAsync());

    [HttpGet("low")]
    public async Task<IActionResult> Low() =>
        Ok(await db.StockItems.Where(s => s.CurrentQty <= s.ReorderLevel).ToListAsync());

    // Only admin defines stock items; quantity changes through Load/Unload
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(StockItem item)
    {
        if (await db.StockItems.AnyAsync(s => s.ItemName == item.ItemName))
            return Conflict(new { message = "Item already exists" });
        item.ItemId = 0;
        db.StockItems.Add(item);
        await db.SaveChangesAsync();
        return Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StockItem input)
    {
        var s = await db.StockItems.FindAsync(id);
        if (s is null) return NotFound();
        s.ItemName = input.ItemName; s.Unit = input.Unit; s.ReorderLevel = input.ReorderLevel;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await db.LoadUnloads.AnyAsync(t => t.ItemId == id))
            return BadRequest(new { message = "Item has transactions and cannot be deleted" });
        var s = await db.StockItems.FindAsync(id);
        if (s is null) return NotFound();
        db.StockItems.Remove(s);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
