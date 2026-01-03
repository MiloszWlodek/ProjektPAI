using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektPAI.Data;

namespace ProjektPAI.Controllers;

public class BuildingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BuildingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Buildings.ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var building = await _context.Buildings
            .Include(b => b.Floors)
            .FirstOrDefaultAsync(b => b.Id == id);

        return View(building);
    }
}