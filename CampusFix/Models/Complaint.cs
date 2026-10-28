using System.ComponentModel.DataAnnotations;

namespace CampusFix.Models;

public class Complaint
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public ComplaintCategory Category { get; set; }
    public ComplaintPriority Priority { get; set; } = ComplaintPriority.Medium;
    public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;

    [Required, StringLength(100)]
    public string Location { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public List<Comment> Comments { get; set; } = new();
}
