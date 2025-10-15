using System.ComponentModel.DataAnnotations;

namespace PruebaMiguelArias.Models;

public class Patient
{
    [Key]
    public int Id {get ; set;} 
    [Required]
    public string Document {get ; set;}
    [Required]
    public string FullName {get ; set;}
    [Required]
    public string Email {get ; set;}
    [Required]
    public string Phone {get ; set;}

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}