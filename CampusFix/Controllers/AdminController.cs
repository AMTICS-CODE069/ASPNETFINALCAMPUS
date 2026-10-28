using CampusFix.Data;
using CampusFix.Filters;
using CampusFix.Models;
using CampusFix.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Controllers;

[SessionAuth(Role = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;
    public AdminController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Dashboard()
    {
        var total = await _db.Complaints.CountAsync();
        var resolved = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.Resolved);

        var byCategory = await _db.Complaints
            .GroupBy(c => c.Category)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        var vm = new DashboardViewModel
        {
            Total = total,
            Pending = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.Pending),
            InProgress = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.InProgress),
            Resolved = resolved,
            Rejected = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.Rejected),
            Students = await _db.Users.CountAsync(u => u.Role == "Student"),
            ResolutionRate = total == 0 ? 0 : Math.Round(resolved * 100.0 / total, 1),
            ByCategory = byCategory.OrderByDescending(x => x.Count).ToDictionary(x => x.Key.ToString(), x => x.Count),
            Recent = await _db.Complaints.Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt).Take(5).ToListAsync(),
            Urgent = await _db.Complaints.Include(c => c.User)
                .Where(c => c.Priority == ComplaintPriority.High &&
                            (c.Status == ComplaintStatus.Pending || c.Status == ComplaintStatus.InProgress))
                .OrderBy(c => c.CreatedAt).Take(5).ToListAsync()
        };

        return View(vm);
    }
}
