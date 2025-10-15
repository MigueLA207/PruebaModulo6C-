using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PruebaMiguelArias.Models;

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Canceled
}

public class Appointment
{
    [Key]
    public int Id {get ; set;} 
    
    [Required]
    public int DoctorId {get ; set;}
    
    [ValidateNever]
    public Doctor? Doctor {get ; set;}
    
    [Required]
    public int PatientId {get ; set;}
    
    [ValidateNever]
    public Patient? Patient {get ; set;}
    
    [Required]
    public DateTimeOffset AppointmentDate {get ; set;}
    
    [Required]
    public AppointmentStatus Status {get ; set;}
    
    [ValidateNever]
    public Notification? Notification { get; set; }
}