using InfoInfo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Controllers.Admin;

[Authorize(Roles = "admin")]
public class CampusAdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public CampusAdminController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Buildings = await _db.Buildings.CountAsync();
        ViewBag.Floors = await _db.Floors.CountAsync();
        ViewBag.Rooms = await _db.Rooms.CountAsync();
        ViewBag.Points = await _db.MovementPoints.CountAsync();
        ViewBag.Directions = await _db.Directions.CountAsync();
        return View();
    }
}
