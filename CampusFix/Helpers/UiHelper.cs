using CampusFix.Models;

namespace CampusFix.Helpers;

public static class UiHelper
{
    public static string StatusClass(ComplaintStatus s) => s switch
    {
        ComplaintStatus.Pending => "bg-warning text-dark",
        ComplaintStatus.InProgress => "bg-info text-dark",
        ComplaintStatus.Resolved => "bg-success",
        _ => "bg-danger"
    };

    public static string StatusText(ComplaintStatus s) =>
        s == ComplaintStatus.InProgress ? "In Progress" : s.ToString();

    public static string PriorityClass(ComplaintPriority p) => p switch
    {
        ComplaintPriority.High => "bg-danger",
        ComplaintPriority.Medium => "bg-primary",
        _ => "bg-secondary"
    };

    public static string CategoryText(ComplaintCategory c) =>
        c == ComplaintCategory.Wifi ? "Wi-Fi" : c.ToString();

    public static string CategoryIcon(ComplaintCategory c) => c switch
    {
        ComplaintCategory.Hostel => "bi-building",
        ComplaintCategory.Mess => "bi-cup-hot",
        ComplaintCategory.Classroom => "bi-easel",
        ComplaintCategory.Library => "bi-book",
        ComplaintCategory.Wifi => "bi-wifi",
        ComplaintCategory.Electrical => "bi-lightning-charge",
        ComplaintCategory.Water => "bi-droplet",
        ComplaintCategory.Cleanliness => "bi-trash3",
        _ => "bi-three-dots"
    };
}
