using CampusFix.Models;
using Microsoft.AspNetCore.Identity;

namespace CampusFix.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db, IPasswordHasher<User> hasher)
    {
        if (db.Users.Any()) return;

        var admin = new User { FullName = "Campus Admin", Email = "admin@campusfix.com", Role = "Admin" };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

        var student = new User { FullName = "Riya Patel", Email = "student@campusfix.com", EnrollmentNo = "CS2024-017", Role = "Student" };
        student.PasswordHash = hasher.HashPassword(student, "Student@123");

        db.Users.AddRange(admin, student);
        db.SaveChanges();

        var c1 = new Complaint { Title = "Wi-Fi keeps disconnecting in Block B", Description = "The hostel Wi-Fi drops every 10-15 minutes after 9 PM, making online classes and assignments impossible.", Category = ComplaintCategory.Wifi, Priority = ComplaintPriority.High, Status = ComplaintStatus.InProgress, Location = "Hostel Block B, 2nd Floor", UserId = student.Id, CreatedAt = DateTime.Now.AddDays(-3) };
        var c2 = new Complaint { Title = "Projector not working in Room 204", Description = "The projector in classroom 204 shows a blank blue screen. Lectures are getting delayed.", Category = ComplaintCategory.Classroom, Priority = ComplaintPriority.Medium, Status = ComplaintStatus.Pending, Location = "Academic Block, Room 204", UserId = student.Id, CreatedAt = DateTime.Now.AddDays(-1) };
        var c3 = new Complaint { Title = "Water cooler leaking near library", Description = "Water is leaking from the cooler near the library entrance and the floor is slippery.", Category = ComplaintCategory.Water, Priority = ComplaintPriority.High, Status = ComplaintStatus.Resolved, Location = "Library Entrance", UserId = student.Id, CreatedAt = DateTime.Now.AddDays(-6), UpdatedAt = DateTime.Now.AddDays(-4) };
        var c4 = new Complaint { Title = "Mess food quality has dropped", Description = "Dinner has been served cold for the past week. Please look into the kitchen staff and menu quality.", Category = ComplaintCategory.Mess, Priority = ComplaintPriority.Low, Status = ComplaintStatus.Pending, Location = "Main Mess Hall", UserId = student.Id, CreatedAt = DateTime.Now.AddHours(-5) };

        db.Complaints.AddRange(c1, c2, c3, c4);
        db.SaveChanges();

        db.Comments.AddRange(
            new Comment { ComplaintId = c1.Id, UserId = admin.Id, Text = "Status changed to In Progress. Network team has been informed and will check the router this week.", CreatedAt = DateTime.Now.AddDays(-2) },
            new Comment { ComplaintId = c3.Id, UserId = admin.Id, Text = "Status changed to Resolved. Maintenance replaced the pipe fitting.", CreatedAt = DateTime.Now.AddDays(-4) }
        );
        db.SaveChanges();
    }
}
