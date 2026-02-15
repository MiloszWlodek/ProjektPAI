using InfoInfo.Data;
using InfoInfo.Models.Campus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Controllers.Admin;

[Authorize(Roles = "admin")]
public class RoomsAdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public RoomsAdminController(ApplicationDbContext db) => _db = db;

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

    private async Task PopulateEntryPointsAsync(int? floorId, int? selectedId = null)
    {
        var q = _db.MovementPoints.AsNoTracking().AsQueryable();
        if (floorId.HasValue) q = q.Where(p => p.FloorId == floorId.Value);

        var points = await q.OrderBy(p => p.Name).ToListAsync();
        var items = points.Select(p => new { p.Id, p.Name }).ToList();
        ViewBag.EntryPointId = new SelectList(items, "Id", "Name", selectedId);
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _db.Rooms.AsNoTracking()
            .Include(r => r.Floor)!.ThenInclude(f => f.Building)
            .OrderBy(r => r.Floor!.Building!.Code).ThenBy(r => r.Floor!.Level).ThenBy(r => r.Name)
            .ToListAsync();
        return View(rooms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var room = await _db.Rooms.AsNoTracking()
            .Include(r => r.Floor)!.ThenInclude(f => f.Building)
            .Include(r => r.EntryPoint)
            .FirstOrDefaultAsync(r => r.Id == id);
        return room == null ? NotFound() : View(room);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateFloorsAsync();
        await PopulateEntryPointsAsync(null);
        return View(new Room());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Room room)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFloorsAsync(room.FloorId);
            await PopulateEntryPointsAsync(room.FloorId, room.EntryPointId);
            return View(room);
        }

        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room == null) return NotFound();

        await PopulateFloorsAsync(room.FloorId);
        await PopulateEntryPointsAsync(room.FloorId, room.EntryPointId);
        return View(room);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Room room)
    {
        if (id != room.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateFloorsAsync(room.FloorId);
            await PopulateEntryPointsAsync(room.FloorId, room.EntryPointId);
            return View(room);
        }

        _db.Update(room);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var room = await _db.Rooms.AsNoTracking()
            .Include(r => r.Floor)!.ThenInclude(f => f.Building)
            .FirstOrDefaultAsync(r => r.Id == id);

        return room == null ? NotFound() : View(room);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room != null)
        {
            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
