using Microsoft.EntityFrameworkCore;
using PruebaMiguelArias.Models;

namespace PruebaMiguelArias.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Patient> Patients { get; set; }
    public DbSet<Models.Doctor> Doctors { get; set; }
    public DbSet<Models.Appointment> Appointments { get; set; }
    
    public DbSet<Models.Notification> Notifications { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)  
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);  // Añadir opción de eliminación en cascada si lo deseas

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)  
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Notification)
            .WithOne(n => n.Appointment)
            .HasForeignKey<Notification>(n => n.AppointmentId)
            .IsRequired();
        
        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<string>();
    }
}