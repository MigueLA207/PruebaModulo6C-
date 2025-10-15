using Microsoft.AspNetCore.Mvc;
using PruebaMiguelArias.Data;
using PruebaMiguelArias.Models;

namespace PruebaMiguelArias.Controllers;

public class PatientsController : Controller
{
    private readonly AppDbContext _context;

    public PatientsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var patients = _context.Patients.ToList();
        return View(patients);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Patient patient)
    {
        if (patient != null)
        {
            try
            {
                if (!_context.Patients.Any(p => p.Document == patient.Document))
                {
                    if (ModelState.IsValid)
                    {
                        _context.Patients.Add(patient);
                        _context.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Ya existe un paciente con el documento proporcionado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el paciente: {ex.Message}");
                ModelState.AddModelError(String.Empty, "Ocurrió un error inesperado al intentar crear el paciente.");
            }
        }
        return View(patient);
    }
    
    public IActionResult Edit(int id)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            TempData["message"] = "El paciente a editar no existe.";
            TempData["alertType"] = "danger";
            return NotFound();
        }
        return View(patient);
    }

    [HttpPost]
    public IActionResult Edit(int id, Patient patient)
    {
        if (id != patient.Id)
        {
            return NotFound(); 
        }
        
        bool documentExists = _context.Patients.Any(p => p.Document == patient.Document && p.Id != id);
        if (documentExists)
        {
            ModelState.AddModelError("Document", "Ya existe un paciente con el documento proporcionado.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                
                _context.Update(patient); 
                _context.SaveChanges();
            
                
                TempData["message"] = "Paciente actualizado exitosamente.";
                TempData["alertType"] = "success";
            
               
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar el paciente: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al guardar los cambios.");
            }
        }
        return View(patient);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            TempData["message"] = "El paciente a eliminar no existe.";
            TempData["alertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        return View(patient);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            TempData["message"] = "El paciente a eliminar no existe.";
            TempData["alertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        
        try
        {
            _context.Patients.Remove(patient);
            _context.SaveChanges();
            TempData["message"] = "Paciente eliminado exitosamente.";
            TempData["alertType"] = "success";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar el paciente: {ex.Message}");
            TempData["message"] = "Ocurrió un error inesperado al intentar eliminar el paciente.";
            TempData["alertType"] = "danger";
        }
        return RedirectToAction(nameof(Index));
    }
}






