using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektPAI.Data;

namespace ProjektPAI.Controllers;

public class NavigationController : Controller
{
    private readonly ApplicationDbContext _context;

    public NavigationController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Point(int id)
    {
        var point = await _context.MovementPoints
            .Include(p => p.Directions)
            .FirstOrDefaultAsync(p => p.Id == id);

        return View(point);
    }
}