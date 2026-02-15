using InfoInfo.Data;
using InfoInfo.Models.Campus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Controllers.Admin;

[Authorize(Roles = "admin")]
public class BuildingsAdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public BuildingsAdminController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Buildings.AsNoTracking().OrderBy(b => b.Code).ThenBy(b => b.Name).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var b = await _db.Buildings.AsNoTracking().Include(x => x.Floors).FirstOrDefaultAsync(x => x.Id == id);
        return b == null ? NotFound() : View(b);
    }

    public IActionResult Create() => View(new Building());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Building building)
    {
        if (!ModelState.IsValid) return View(building);
        _db.Buildings.Add(building);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var b = await _db.Buildings.FindAsync(id);
        return b == null ? NotFound() : View(b);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Building building)
    {
        if (id != building.Id) return NotFound();
        if (!ModelState.IsValid) return View(building);

        _db.Update(building);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var b = await _db.Buildings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return b == null ? NotFound() : View(b);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var b = await _db.Buildings.FindAsync(id);
        if (b != null)
        {
            _db.Buildings.Remove(b);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
