using InfoInfo.Data;
using InfoInfo.Models.Campus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Controllers.Admin;

[Authorize(Roles = "admin")]
public class DirectionsAdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public DirectionsAdminController(ApplicationDbContext db) => _db = db;

    private async Task PopulateLookupsAsync(int? fromPointId = null, int? toPointId = null, int? toRoomId = null)
    {
        var points = await _db.MovementPoints.AsNoTracking()
            .Include(p => p.Floor)!.ThenInclude(f => f.Building)
            .OrderBy(p => p.Floor!.Building!.Code).ThenBy(p => p.Floor!.Level).ThenBy(p => p.Name)
            .Select(p => new { p.Id, Label = $"{p.Floor!.Building!.Code} {p.Floor.Name}: {p.Name}" })
            .ToListAsync();

        ViewBag.FromPointId = new SelectList(points, "Id", "Label", fromPointId);
        ViewBag.ToPointId = new SelectList(points, "Id", "Label", toPointId);

        var rooms = await _db.Rooms.AsNoTracking()
            .Include(r => r.Floor)!.ThenInclude(f => f.Building)
            .OrderBy(r => r.Floor!.Building!.Code).ThenBy(r => r.Floor!.Level).ThenBy(r => r.Name)
            .Select(r => new { r.Id, Label = $"{r.Floor!.Building!.Code} {r.Floor.Name}: {r.Name}" })
            .ToListAsync();

        ViewBag.ToRoomId = new SelectList(rooms, "Id", "Label", toRoomId);
        ViewBag.Kind = new SelectList(Enum.GetValues(typeof(DirectionKind)).Cast<DirectionKind>().Select(k => new { Id = (int)k, Name = k.ToString() }), "Id", "Name");
    }

    public async Task<IActionResult> Index()
    {
        var dirs = await _db.Directions.AsNoTracking()
            .Include(d => d.FromPoint)!.ThenInclude(p => p.Floor)!.ThenInclude(f => f.Building)
            .Include(d => d.ToPoint)!.ThenInclude(p => p.Floor)!.ThenInclude(f => f.Building)
            .Include(d => d.ToRoom)!.ThenInclude(r => r.Floor)!.ThenInclude(f => f.Building)
            .OrderBy(d => d.FromPointId).ThenBy(d => d.Kind)
            .ToListAsync();
        return View(dirs);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dir = await _db.Directions.AsNoTracking()
            .Include(d => d.FromPoint)!.ThenInclude(p => p.Floor)!.ThenInclude(f => f.Building)
            .Include(d => d.ToPoint)!.ThenInclude(p => p.Floor)!.ThenInclude(f => f.Building)
            .Include(d => d.ToRoom)!.ThenInclude(r => r.Floor)!.ThenInclude(f => f.Building)
            .FirstOrDefaultAsync(d => d.Id == id);

        return dir == null ? NotFound() : View(dir);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateLookupsAsync();
        return View(new Direction());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Direction dir)
    {
        // walidacja: tylko jedno z ToPointId / ToRoomId
        if (dir.ToPointId.HasValue && dir.ToRoomId.HasValue)
            ModelState.AddModelError("", "Wybierz albo docelowy punkt, albo docelową salę (nie oba).");

        if (!dir.ToPointId.HasValue && !dir.ToRoomId.HasValue)
            ModelState.AddModelError("", "Wybierz docelowy punkt albo docelową salę.");

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(dir.FromPointId, dir.ToPointId, dir.ToRoomId);
            return View(dir);
        }

        _db.Directions.Add(dir);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dir = await _db.Directions.FindAsync(id);
        if (dir == null) return NotFound();

        await PopulateLookupsAsync(dir.FromPointId, dir.ToPointId, dir.ToRoomId);
        return View(dir);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Direction dir)
    {
        if (id != dir.Id) return NotFound();

        if (dir.ToPointId.HasValue && dir.ToRoomId.HasValue)
            ModelState.AddModelError("", "Wybierz albo docelowy punkt, albo docelową salę (nie oba).");

        if (!dir.ToPointId.HasValue && !dir.ToRoomId.HasValue)
            ModelState.AddModelError("", "Wybierz docelowy punkt albo docelową salę.");

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(dir.FromPointId, dir.ToPointId, dir.ToRoomId);
            return View(dir);
        }

        _db.Update(dir);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var dir = await _db.Directions.AsNoTracking()
            .Include(d => d.FromPoint)
            .FirstOrDefaultAsync(d => d.Id == id);

        return dir == null ? NotFound() : View(dir);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var dir = await _db.Directions.FindAsync(id);
        if (dir != null)
        {
            _db.Directions.Remove(dir);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
