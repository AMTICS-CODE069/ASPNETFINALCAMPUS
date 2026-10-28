using System.ComponentModel.DataAnnotations;

namespace CampusFix.Models;

public class Comment
{
    public int Id { get; set; }

    [Required, StringLength(500)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int ComplaintId { get; set; }
    public Complaint? Complaint { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}
