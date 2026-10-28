using System.ComponentModel.DataAnnotations;

namespace CampusFix.Models;

public class User
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    public string? EnrollmentNo { get; set; }

    [Required, StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Role { get; set; } = "Student";   // "Student" or "Admin"

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Complaint> Complaints { get; set; } = new();
}
