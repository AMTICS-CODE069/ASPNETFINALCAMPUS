using System.ComponentModel.DataAnnotations;

namespace CampusFix.Models;

public enum ComplaintCategory
{
    Hostel,
    Mess,
    Classroom,
    Library,
    [Display(Name = "Wi-Fi")] Wifi,
    Electrical,
    Water,
    Cleanliness,
    Other
}

public enum ComplaintPriority
{
    Low,
    Medium,
    High
}

public enum ComplaintStatus
{
    Pending,
    [Display(Name = "In Progress")] InProgress,
    Resolved,
    Rejected
}
