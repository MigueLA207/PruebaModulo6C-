using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaMiguelArias.Data;
using PruebaMiguelArias.Models;
using PruebaMiguelArias.Services;

namespace PruebaMiguelArias.Controllers;

public class AppointmentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    public AppointmentsController(AppDbContext context, IEmailService emailService, IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
    }
    
    private void PopulateFilterDropdowns()
    {
        ViewBag.Doctors = _context.Doctors.ToList();
        ViewBag.Patients = _context.Patients.ToList();
    }
    
    public IActionResult Index()
    {
        var appointments = _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .ToList().OrderBy(a => a.Id);
        
        PopulateFilterDropdowns();
        
        ViewBag.Title = "Gestión de Citas";
        return View(appointments);
    }

    public IActionResult Create()
    {
        ViewBag.Doctors = _context.Doctors.ToList();
        ViewBag.Patients = _context.Patients.ToList();
        return View();
    }
    

[HttpPost]
[ValidateAntiForgeryToken] // Buena práctica añadir esto
public async Task<IActionResult> Create(Appointment appointment)
{
    if (appointment != null)
    {
        
        DateTimeOffset appointmentDate = appointment.AppointmentDate;
        bool dateRepeatedDoctor = _context.Appointments.Any(a => a.DoctorId == appointment.DoctorId && a.AppointmentDate == appointmentDate);
        if (dateRepeatedDoctor)
        {
            ModelState.AddModelError("DoctorId", "El doctor ya tiene una cita en ese horario.");
        }
        
        bool dateRepeatedPatient = _context.Appointments.Any(a => a.PatientId == appointment.PatientId && a.AppointmentDate == appointmentDate);
        if (dateRepeatedPatient)
        {
            ModelState.AddModelError("PatientId", "El paciente ya tiene una cita en ese horario.");
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                appointment.Status = AppointmentStatus.Scheduled;
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                
                var notification = new Notification
                {
                    AppointmentId = appointment.Id,
                    NotificationDate = DateTimeOffset.Now,
                    sent = SentStatus.Failed 
                };
                
                try
                {

                    var patient = await _context.Patients.FindAsync(appointment.PatientId);
                    var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);

         
                    if (patient != null && doctor != null && !string.IsNullOrEmpty(patient.Email))
                    {
                        var subject = "Confirmación de Cita Agendada";
                        
                  
                        var messageBody = $@"
                            <html>
                            <body>
                                <h2>Hola {patient.FullName},</h2>
                                <p>Le confirmamos que su cita ha sido agendada con éxito.</p>
                                <hr>
                                <p><strong>Detalles de la Cita:</strong></p>
                                <ul>
                                    <li><strong>Doctor:</strong> {doctor.FullName}</li>
                                    <li><strong>Fecha y Hora:</strong> {appointment.AppointmentDate.ToString("dddd, dd MMMM yyyy 'a las' HH:mm 'hrs.'")}</li>
                                </ul>
                                <p>Gracias por su preferencia.</p>
                                <br>
                                <p><strong>Atentamente,<br/>{_configuration["EmailSettings:SenderName"]}</strong></p>
                            </body>
                            </html>";

                 
                        await _emailService.SendEmailAsync(patient.Email, subject, messageBody);
                        notification.sent = SentStatus.Sent;
                    }
                }
                catch (Exception ex)
                {
       
                    Console.WriteLine($"Error al enviar correo de confirmación: {ex.Message}");
                }
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
          
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la cita: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al intentar crear la cita.");
            }
        }
    }
    

    ViewBag.Doctors = _context.Doctors.ToList();
    ViewBag.Patients = _context.Patients.ToList();
    return View(appointment);
}
    
    public IActionResult Edit(int id)
    {
        var appointment = _context.Appointments.Find(id);
        if (appointment == null)
        {
            TempData["message"] = "La cita a editar no existe.";
            TempData["alertType"] = "danger";
            return NotFound();
        }
        
        ViewBag.Doctors = _context.Doctors.ToList();
        ViewBag.Patients = _context.Patients.ToList();
        return View(appointment);
    }

    [HttpPost]
    public IActionResult Edit(int id, Appointment appointment)
    {
        if (appointment != null)
        {
            try
            {
                DateTimeOffset appointmentDate = appointment.AppointmentDate;
                bool DateRepeatedDoctor = _context.Appointments.Any(a => a.DoctorId == appointment.DoctorId && a.AppointmentDate == appointmentDate);

                if (DateRepeatedDoctor) 
                {
                    ModelState.AddModelError("DoctorId", "El doctor ya tiene una cita en ese horario.");
                }
                
                bool DateRepeatedPatient = _context.Appointments.Any(a => a.PatientId == appointment.PatientId && a.AppointmentDate == appointmentDate);
                if (DateRepeatedPatient)
                {
                    ModelState.AddModelError("PatientId", "El paciente ya tiene una cita en ese horario.");
                }
                
                if (ModelState.IsValid)
                {
                    var existingAppointment = _context.Appointments.Find(id);
                    if (existingAppointment == null)
                    {
                        return NotFound();
                    }
                    
                    existingAppointment.DoctorId = appointment.DoctorId;
                    existingAppointment.PatientId = appointment.PatientId;
                    existingAppointment.AppointmentDate = appointment.AppointmentDate;
                    existingAppointment.Status = appointment.Status;
                    
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar la cita: {ex.Message}");
                ModelState.AddModelError(String.Empty, "Ocurrió un error inesperado al intentar editar la cita.");
            }
        }
        
        ViewBag.Doctors = _context.Doctors.ToList();
        ViewBag.Patients = _context.Patients.ToList();
        return View(appointment);
    }
    
    public IActionResult Cancel(int id)
    {
        var appointment = _context.Appointments.Find(id);
        if (appointment == null)
        {
            TempData["message"] = "La cita no existe.";
            TempData["alertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }

        appointment.Status = AppointmentStatus.Canceled;
        _context.Appointments.Update(appointment);
        _context.SaveChanges();

        TempData["message"] = "La cita fue cancelada.";
        TempData["alertType"] = "warning";
        return RedirectToAction(nameof(Index));
    }
    
    
    public IActionResult MarkAsAttended(int id)
    {
        var appointment = _context.Appointments.Find(id);
        if (appointment == null)
        {
            TempData["message"] = "La cita no existe.";
            TempData["alertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }

        appointment.Status = AppointmentStatus.Completed;
        _context.Appointments.Update(appointment);
        _context.SaveChanges();

        TempData["message"] = "La cita fue marcada como atendida.";
        TempData["alertType"] = "success";
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult ByPatient(int patientId)
    {
        var appointments = _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.PatientId == patientId)
            .ToList();
        
        PopulateFilterDropdowns();

        var patientName = appointments.FirstOrDefault()?.Patient.FullName ?? "";
        ViewBag.Title = $"Citas del paciente: {patientName}";
        return View("Index", appointments); 
    }
    

    public IActionResult ByDoctor(int doctorId)
    {
        var appointments = _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .ToList();
        
        PopulateFilterDropdowns();

        var doctorName = appointments.FirstOrDefault()?.Doctor.FullName ?? "";
        ViewBag.Title = $"Citas del doctor: {doctorName}";
        return View("Index", appointments); 
    }
    
}