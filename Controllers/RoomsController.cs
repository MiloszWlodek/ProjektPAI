using Microsoft.AspNetCore.Mvc;
using ProjektPAI.Data;

namespace ProjektPAI.Controllers;

public class RoomsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RoomsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Details(int id)
    {
        var room = _context.Rooms.FirstOrDefault(r => r.Id == id);
        return View(room);
    }

    public IActionResult Search(string query)
    {
        var rooms = _context.Rooms
            .Where(r => r.Name.Contains(query))
            .ToList();

        return View(rooms);
    }
}
