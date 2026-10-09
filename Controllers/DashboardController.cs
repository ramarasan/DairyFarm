using DairyFarm.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DairyFarm.Api.Controllers;

[ApiController, Authorize]
[Route("api/[controller]")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var today = DateTime.Today;
        var weekEnd = today.AddDays(7);

        var result = new
        {
            totalCows = await db.Cows.CountAsync(c => c.Status == "Active"),
            newCows = await db.Cows.CountAsync(c => c.Status == "Active" && c.CowType == "New"),
            oldCows = await db.Cows.CountAsync(c => c.Status == "Active" && c.CowType == "Old"),
            todayMilkLitres = await db.MilkRecords.Where(m => m.RecordDate == today).SumAsync(m => (decimal?)m.Litres) ?? 0,
            yesterdayMilkLitres = await db.MilkRecords.Where(m => m.RecordDate == today.AddDays(-1)).SumAsync(m => (decimal?)m.Litres) ?? 0,
            visitsDueThisWeek = await db.DoctorVisits.CountAsync(v => v.NextVisitDate >= today && v.NextVisitDate <= weekEnd),
            lowStockItems = await db.StockItems.CountAsync(s => s.CurrentQty <= s.ReorderLevel),
            todayLoaded = await db.LoadUnloads.Where(t => t.TxnDate >= today && t.Direction == "Load").SumAsync(t => (decimal?)t.Quantity) ?? 0,
            todayUnloaded = await db.LoadUnloads.Where(t => t.TxnDate >= today && t.Direction == "Unload").SumAsync(t => (decimal?)t.Quantity) ?? 0
        };
        return Ok(result);
    }
}
