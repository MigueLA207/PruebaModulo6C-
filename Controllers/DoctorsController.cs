using Microsoft.AspNetCore.Mvc;
using PruebaMiguelArias.Models;
using PruebaMiguelArias.Services;

namespace PruebaMiguelArias.Controllers;

public class DoctorsController : Controller
{
    private readonly IDoctorService _doctorService;
    
    public DoctorsController(IDoctorService passengerService)
    {
        _doctorService = passengerService;
    }

    public IActionResult Index(string specialty)
    {
        var doctors = _doctorService.GetAllDoctors();

        if (!string.IsNullOrEmpty(specialty))
        {
            doctors = doctors.Where(d => d.Specialty == specialty);
        }

        ViewBag.Specialties = _doctorService.GetAllSpecialties();
        ViewBag.SpecialtyFilter = specialty;

        return View(doctors);
    }

    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    

    [HttpPost]
    public IActionResult Create(Doctor doctor)
    {
        try
        {

            if (_doctorService.IsDoctorUnique(doctor.FullName, doctor.Specialty))
            {
                if (!_doctorService.documentExists(doctor.Document))
                {
                    if (ModelState.IsValid)
                    {
                        Console.WriteLine("JOA MANi");
                        _doctorService.CreateDoctor(doctor);
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Ya existe un médico con el documento proporcionado.");
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Ya existe un médico con el mismo nombre y especialidad.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear el doctor: {ex.Message}");
            ModelState.AddModelError(String.Empty, "Ocurrió un error inesperado al intentar crear el médico.");
        }
        
        return View(doctor);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var doctor = _doctorService.GetDoctorById(id);
        if (doctor == null)
        {
            TempData["message"] = "El doctor a editar no existe.";
            TempData["alertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        return View(doctor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Doctor doctor)
    {
        if (id == null || id != doctor.Id)
        {
            TempData["message"] = "El ID del doctor no es válido.";
            TempData["alertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    
        try
        {
            if (_doctorService.IsDoctorUnique(doctor.FullName, doctor.Specialty, id))
            {
                if (!_doctorService.documentExists(doctor.Document) || 
                    _doctorService.GetDoctorById(id).Document == doctor.Document)
                {
                    if (ModelState.IsValid)
                    {
                        _doctorService.UpdateDoctor(id, doctor);
                        TempData["message"] = "Doctor actualizado exitosamente.";
                        TempData["alertType"] = "success";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un médico con el documento proporcionado.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ya existe un médico con el mismo nombre y especialidad.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar el doctor: {ex.Message}");
            ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al intentar actualizar el médico.");
        }

        // Aquí retornamos la vista con el modelo y errores si algo falló
        return View(doctor);
    }
    
}