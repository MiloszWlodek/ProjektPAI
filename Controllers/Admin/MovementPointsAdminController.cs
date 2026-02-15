using InfoInfo.Data;
using InfoInfo.Models.Campus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Controllers.Admin;

[Authorize(Roles = "admin")]
public class MovementPointsAdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public MovementPointsAdminController(ApplicationDbContext db) => _db = db;

    private async Task PopulateFloorsAsync(int? selectedId = null)
    {
        var floors = await _db.Floors.AsNoTracking()
            .Include(f => f.Building)
            .OrderBy(f => f.Building!.Code).ThenBy(f => f.Level)
            .ToListAsync();

        var items = floors.Select(f => new
        {
            f.Id,
            Label = $"{f.Building?.Code ?? ""} {f.Building?.Name} – {f.Name}"
        }).ToList();

        ViewBag.FloorId = new SelectList(items, "Id", "Label", selectedId);
    }

    public async Task<IActionResult> Index()
    {
        var points = await _db.MovementPoints.AsNoTracking()
            .Include(p => p.Floor)!.ThenInclude(f => f.Building)
            .OrderBy(p => p.Floor!.Building!.Code).ThenBy(p => p.Floor!.Level).ThenBy(p => p.Name)
            .ToListAsync();

        return View(points);
    }

    public async Task<IActionResult> Details(int id)
    {
        var point = await _db.MovementPoints.AsNoTracking()
            .Include(p => p.Floor)!.ThenInclude(f => f.Building)
            .FirstOrDefaultAsync(p => p.Id == id);

        return point == null ? NotFound() : View(point);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateFloorsAsync();
        return View(new MovementPoint());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovementPoint point)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFloorsAsync(point.FloorId);
            return View(point);
        }

        _db.MovementPoints.Add(point);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var point = await _db.MovementPoints.FindAsync(id);
        if (point == null) return NotFound();

        await PopulateFloorsAsync(point.FloorId);
        return View(point);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MovementPoint point)
    {
        if (id != point.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateFloorsAsync(point.FloorId);
            return View(point);
        }

        _db.Update(point);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var point = await _db.MovementPoints.AsNoTracking()
            .Include(p => p.Floor)!.ThenInclude(f => f.Building)
            .FirstOrDefaultAsync(p => p.Id == id);

        return point == null ? NotFound() : View(point);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var point = await _db.MovementPoints.FindAsync(id);
        if (point != null)
        {
            _db.MovementPoints.Remove(point);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
