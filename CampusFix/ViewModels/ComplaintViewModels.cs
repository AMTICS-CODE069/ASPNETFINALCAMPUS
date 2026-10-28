using System.ComponentModel.DataAnnotations;
using CampusFix.Models;

namespace CampusFix.ViewModels;

public class ComplaintFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Give your complaint a short title")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be 5 to 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please describe the problem")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be 10 to 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pick a category")]
    public ComplaintCategory? Category { get; set; }

    [Required(ErrorMessage = "Pick a priority")]
    public ComplaintPriority? Priority { get; set; } = ComplaintPriority.Medium;

    [Required(ErrorMessage = "Tell us where the problem is")]
    [StringLength(100, ErrorMessage = "Location is too long")]
    [Display(Name = "Location (Block / Room / Area)")]
    public string Location { get; set; } = string.Empty;
}

public class ComplaintListViewModel
{
    public List<Complaint> Items { get; set; } = new();
    public string? Search { get; set; }
    public ComplaintStatus? Status { get; set; }
    public ComplaintCategory? Category { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }
    public bool IsAdmin { get; set; }
}

public class DashboardViewModel
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Rejected { get; set; }
    public int Students { get; set; }
    public double ResolutionRate { get; set; }
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public List<Complaint> Recent { get; set; } = new();
    public List<Complaint> Urgent { get; set; } = new();
}
