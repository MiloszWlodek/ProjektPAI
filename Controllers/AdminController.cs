using Microsoft.AspNetCore.Mvc;
using ProjektPAI.Data;
using ProjektPAI.Models;

namespace ProjektPAI.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Buildings.ToList());
    }

    public IActionResult CreateBuilding()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateBuilding(Building building)
    {
        _context.Buildings.Add(building);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}