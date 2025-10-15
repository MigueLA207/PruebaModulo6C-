using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaMiguelArias.Data;

namespace PruebaMiguelArias.Controllers;

public class NotificationsController : Controller
{
    
    private readonly AppDbContext _context;
    public NotificationsController(AppDbContext context)
    {
        _context = context;

    }
    
    public IActionResult Index()
    {
        var notifications = _context.Notifications.ToList();
        return View(notifications);
    }
    
    
    
}