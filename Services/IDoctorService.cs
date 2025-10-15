using PruebaMiguelArias.Models;

namespace PruebaMiguelArias.Services;

public interface IDoctorService
{
    IEnumerable<Doctor> GetAllDoctors();
    Doctor GetDoctorById(int id);
    
    bool documentExists(string document);
    bool IsDoctorUnique(string FullName, string specialty, int? id = null);
    void CreateDoctor(Doctor doctor);
    void UpdateDoctor(int id, Doctor doctor);
    IEnumerable<string> GetAllSpecialties();
    
    
}