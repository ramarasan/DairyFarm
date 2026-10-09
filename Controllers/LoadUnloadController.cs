using System.Security.Claims;
using DairyFarm.Api.Data;
using DairyFarm.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DairyFarm.Api.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class LoadUnloadController(AppDbContext db) : ControllerBase
{
    int Me => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? direction)
    {
        var q = db.LoadUnloads.AsQueryable();
        if (from.HasValue) q = q.Where(t => t.TxnDate >= from.Value.Date);
        if (to.HasValue) q = q.Where(t => t.TxnDate < to.Value.Date.AddDays(1));
        if (!string.IsNullOrEmpty(direction)) q = q.Where(t => t.Direction == direction);

        return Ok(await (from t in q
                         join s in db.StockItems on t.ItemId equals s.ItemId
                         orderby t.TxnDate descending
                         select new { t.TxnId, t.TxnDate, t.Direction, t.ItemId, s.ItemName, s.Unit,
                                      t.Quantity, t.VehicleNo, t.PartyName, t.Remarks }).ToListAsync());
    }

    // Stock is updated by the DB trigger. Block loading more than available.
    [HttpPost]
    public async Task<IActionResult> Create(LoadUnloadTxn t)
    {
        if (t.Direction is not ("Load" or "Unload")) return BadRequest(new { message = "Direction must be Load or Unload" });
        if (t.Quantity <= 0) return BadRequest(new { message = "Quantity must be greater than 0" });

        var item = await db.StockItems.FindAsync(t.ItemId);
        if (item is null) return NotFound(new { message = "Stock item not found" });
        if (t.Direction == "Load" && item.CurrentQty < t.Quantity)
            return BadRequest(new { message = $"Only {item.CurrentQty} {item.Unit} available" });

        t.TxnId = 0; t.EnteredBy = Me; t.TxnDate = DateTime.Now;
        db.LoadUnloads.Add(t);
        await db.SaveChangesAsync();
        return Ok(t);
    }

    // Admin only. Deleting does NOT reverse stock, so correct stock with a new entry instead.
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await db.LoadUnloads.FindAsync(id);
        if (t is null) return NotFound();
        db.LoadUnloads.Remove(t);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
