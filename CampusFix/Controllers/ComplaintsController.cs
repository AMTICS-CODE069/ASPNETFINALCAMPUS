using CampusFix.Data;
using CampusFix.Filters;
using CampusFix.Models;
using CampusFix.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Controllers;

[SessionAuth]
public class ComplaintsController : Controller
{
    private const int PageSize = 8;
    private readonly AppDbContext _db;

    public ComplaintsController(AppDbContext db) => _db = db;

    private int CurrentUserId => HttpContext.Session.GetInt32("UserId") ?? 0;
    private bool IsAdmin => HttpContext.Session.GetString("Role") == "Admin";

    // ---------- READ (list with search, filter, paging) ----------
    public async Task<IActionResult> Index(string? search, ComplaintStatus? status, ComplaintCategory? category, int page = 1)
    {
        IQueryable<Complaint> query = _db.Complaints.Include(c => c.User);

        if (!IsAdmin) query = query.Where(c => c.UserId == CurrentUserId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Title.Contains(search) || c.Location.Contains(search));
        if (status.HasValue) query = query.Where(c => c.Status == status);
        if (category.HasValue) query = query.Where(c => c.Category == category);

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return View(new ComplaintListViewModel
        {
            Items = items,
            Search = search,
            Status = status,
            Category = category,
            Page = page,
            TotalPages = totalPages,
            TotalCount = total,
            IsAdmin = IsAdmin
        });
    }

    // ---------- READ (details + comments) ----------
    public async Task<IActionResult> Details(int id)
    {
        var complaint = await _db.Complaints
            .Include(c => c.User)
            .Include(c => c.Comments).ThenInclude(cm => cm.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (complaint == null) return NotFound();
        if (!IsAdmin && complaint.UserId != CurrentUserId) return RedirectToAction("AccessDenied", "Account");

        return View(complaint);
    }

    // ---------- CREATE ----------
    [HttpGet]
    public IActionResult Create() => View(new ComplaintFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ComplaintFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var complaint = new Complaint
        {
            Title = vm.Title.Trim(),
            Description = vm.Description.Trim(),
            Category = vm.Category!.Value,
            Priority = vm.Priority!.Value,
            Location = vm.Location.Trim(),
            UserId = CurrentUserId
        };

        _db.Complaints.Add(complaint);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Complaint submitted successfully!";
        return RedirectToAction(nameof(Details), new { id = complaint.Id });
    }

    // ---------- UPDATE (only owner, only while Pending) ----------
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var complaint = await _db.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();
        if (complaint.UserId != CurrentUserId || complaint.Status != ComplaintStatus.Pending)
        {
            TempData["Error"] = "You can only edit your own complaints while they are still Pending.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return View(new ComplaintFormViewModel
        {
            Id = complaint.Id,
            Title = complaint.Title,
            Description = complaint.Description,
            Category = complaint.Category,
            Priority = complaint.Priority,
            Location = complaint.Location
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ComplaintFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var complaint = await _db.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();
        if (complaint.UserId != CurrentUserId || complaint.Status != ComplaintStatus.Pending)
        {
            TempData["Error"] = "This complaint can no longer be edited.";
            return RedirectToAction(nameof(Details), new { id });
        }

        complaint.Title = vm.Title.Trim();
        complaint.Description = vm.Description.Trim();
        complaint.Category = vm.Category!.Value;
        complaint.Priority = vm.Priority!.Value;
        complaint.Location = vm.Location.Trim();
        complaint.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Complaint updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ---------- DELETE ----------
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var complaint = await _db.Complaints.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
        if (complaint == null) return NotFound();
        if (!CanDelete(complaint))
        {
            TempData["Error"] = "You are not allowed to delete this complaint.";
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(complaint);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var complaint = await _db.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();
        if (!CanDelete(complaint))
        {
            TempData["Error"] = "You are not allowed to delete this complaint.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _db.Complaints.Remove(complaint);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Complaint deleted.";
        return RedirectToAction(nameof(Index));
    }

    private bool CanDelete(Complaint c) =>
        IsAdmin || (c.UserId == CurrentUserId && c.Status == ComplaintStatus.Pending);

    // ---------- ADMIN: change status ----------
    [HttpPost, ValidateAntiForgeryToken, SessionAuth(Role = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, ComplaintStatus status, string? remark)
    {
        var complaint = await _db.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();

        complaint.Status = status;
        complaint.UpdatedAt = DateTime.Now;

        var text = $"Status changed to {Helpers.UiHelper.StatusText(status)}.";
        if (!string.IsNullOrWhiteSpace(remark)) text += " " + remark.Trim();
        if (text.Length > 500) text = text[..500];

        _db.Comments.Add(new Comment { ComplaintId = id, UserId = CurrentUserId, Text = text });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Status updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ---------- Comments ----------
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, string? text)
    {
        var complaint = await _db.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();
        if (!IsAdmin && complaint.UserId != CurrentUserId) return RedirectToAction("AccessDenied", "Account");

        if (string.IsNullOrWhiteSpace(text) || text.Length > 500)
        {
            TempData["Error"] = "Comment must be between 1 and 500 characters.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _db.Comments.Add(new Comment { ComplaintId = id, UserId = CurrentUserId, Text = text.Trim() });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }
}
