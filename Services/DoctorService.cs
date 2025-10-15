using PruebaMiguelArias.Data;
using PruebaMiguelArias.Models;

namespace PruebaMiguelArias.Services;

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _context;

    public DoctorService(AppDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Doctor> GetAllDoctors()
    {
        return _context.Doctors.ToList();
    }
    
    public Doctor GetDoctorById(int id)
    {
        return _context.Doctors.Find(id);
    }
    
    public bool IsDoctorUnique(string fullName, string specialty, int? id = null)
    {
        return !_context.Doctors.Any(d => d.FullName == fullName && d.Specialty == specialty && d.Id != id);
    }
    
    public void CreateDoctor(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        _context.SaveChanges();
    }
    
    public void UpdateDoctor(int id, Doctor doctor)
    {
        var existingDoctor = _context.Doctors.Find(id);
        if (existingDoctor != null)
        {
            existingDoctor.Document = doctor.Document;
            existingDoctor.FullName = doctor.FullName;
            existingDoctor.Specialty = doctor.Specialty;
            existingDoctor.Email = doctor.Email;
            existingDoctor.Phone = doctor.Phone;

            _context.SaveChanges();
        }
        
    }
    
    public bool documentExists(string document)
    {
        return _context.Doctors.Any(d => d.Document == document);
    }
    
    public IEnumerable<string> GetAllSpecialties()
    {
        return _context.Doctors.Select(d => d.Specialty).Distinct().ToList();
    }
    
    public void DeleteDoctor(int id)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
            _context.SaveChanges();
        }
    }
}