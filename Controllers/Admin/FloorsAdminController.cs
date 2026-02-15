using InfoInfo.Data;
using InfoInfo.Models.Campus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Controllers.Admin;

[Authorize(Roles = "admin")]
public class FloorsAdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public FloorsAdminController(ApplicationDbContext db) => _db = db;

    private async Task PopulateBuildingsAsync(int? selectedId = null)
    {
        var buildings = await _db.Buildings.AsNoTracking()
            .OrderBy(b => b.Code).ThenBy(b => b.Name)
            .ToListAsync();

        ViewBag.BuildingId = new SelectList(buildings, nameof(Building.Id), nameof(Building.Name), selectedId);
    }

    public async Task<IActionResult> Index()
    {
        var floors = await _db.Floors.AsNoTracking()
            .Include(f => f.Building)
            .OrderBy(f => f.Building!.Code).ThenBy(f => f.Level)
            .ToListAsync();
        return View(floors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var floor = await _db.Floors.AsNoTracking()
            .Include(f => f.Building)
            .Include(f => f.Rooms)
            .Include(f => f.MovementPoints)
            .FirstOrDefaultAsync(f => f.Id == id);

        return floor == null ? NotFound() : View(floor);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateBuildingsAsync();
        return View(new Floor());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Floor floor)
    {
        if (!ModelState.IsValid)
        {
            await PopulateBuildingsAsync(floor.BuildingId);
            return View(floor);
        }

        _db.Floors.Add(floor);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var floor = await _db.Floors.FindAsync(id);
        if (floor == null) return NotFound();
        await PopulateBuildingsAsync(floor.BuildingId);
        return View(floor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Floor floor)
    {
        if (id != floor.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateBuildingsAsync(floor.BuildingId);
            return View(floor);
        }

        _db.Update(floor);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var floor = await _db.Floors.AsNoTracking()
            .Include(f => f.Building)
            .FirstOrDefaultAsync(f => f.Id == id);
        return floor == null ? NotFound() : View(floor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var floor = await _db.Floors.FindAsync(id);
        if (floor != null)
        {
            _db.Floors.Remove(floor);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
