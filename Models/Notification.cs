using System.ComponentModel.DataAnnotations;

namespace PruebaMiguelArias.Models;

public enum SentStatus
{
    Sent,
    Failed
}

public class Notification
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    [Required]
    public DateTimeOffset NotificationDate { get; set; }
    [Required]
    public SentStatus sent { get; set; }
    
}