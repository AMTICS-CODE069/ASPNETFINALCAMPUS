using CampusFix.Data;
using CampusFix.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.Total = await _db.Complaints.CountAsync();
        ViewBag.Resolved = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.Resolved);
        ViewBag.Students = await _db.Users.CountAsync(u => u.Role == "Student");
        return View();
    }

    public IActionResult Error() => View();
}
