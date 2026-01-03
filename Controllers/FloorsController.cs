using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektPAI.Data;

namespace ProjektPAI.Controllers;

public class FloorsController : Controller
{
    private readonly ApplicationDbContext _context;

    public FloorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details(int id)
    {
        var floor = await _context.Floors
            .Include(f => f.MovementPoints)
            .FirstOrDefaultAsync(f => f.Id == id);

        return View(floor);
    }
}
