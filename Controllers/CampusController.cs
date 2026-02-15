using InfoInfo.Data;
using InfoInfo.Models.Campus;
using InfoInfo.Services;
using InfoInfo.ViewModels.Campus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Controllers;

public class CampusController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly CampusRouteService _route;

    public CampusController(ApplicationDbContext db, CampusRouteService route)
    {
        _db = db;
        _route = route;
    }

    public async Task<IActionResult> Index()
    {
        var buildings = await _db.Buildings.AsNoTracking()
            .OrderBy(b => b.Code).ThenBy(b => b.Name)
            .ToListAsync();
        return View(buildings);
    }

    public async Task<IActionResult> Building(int id)
    {
        var building = await _db.Buildings.AsNoTracking()
            .Include(b => b.Floors.OrderBy(f => f.Level))
            .FirstOrDefaultAsync(b => b.Id == id);

        if (building == null) return NotFound();
        return View(building);
    }

    public async Task<IActionResult> Floor(int id)
    {
        var floor = await _db.Floors.AsNoTracking()
            .Include(f => f.Building)
            .Include(f => f.Rooms.OrderBy(r => r.Name))
            .Include(f => f.MovementPoints.OrderBy(p => p.Name))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (floor == null) return NotFound();
        return View(floor);
    }

    public async Task<IActionResult> Point(int id)
    {
        var point = await _db.MovementPoints.AsNoTracking()
            .Include(p => p.Floor)!.ThenInclude(f => f.Building)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (point == null) return NotFound();

        var dirs = await _db.Directions.AsNoTracking()
            .Where(d => d.FromPointId == id)
            .OrderBy(d => d.Kind)
            .ToListAsync();

        // Przypnij po Kind
        var vm = new PointViewModel
        {
            Point = point,
            Forward = dirs.FirstOrDefault(d => d.Kind == DirectionKind.Forward),
            Left = dirs.FirstOrDefault(d => d.Kind == DirectionKind.Left),
            Right = dirs.FirstOrDefault(d => d.Kind == DirectionKind.Right),
            Back = dirs.FirstOrDefault(d => d.Kind == DirectionKind.Back),
        };

        return View(vm);
    }

    public async Task<IActionResult> Room(int id)
    {
        var room = await _db.Rooms.AsNoTracking()
            .Include(r => r.Floor)!.ThenInclude(f => f.Building)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (room == null) return NotFound();
        return View(room);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? q)
    {
        q ??= string.Empty;
        q = q.Trim();

        var results = await _db.Rooms.AsNoTracking()
            .Include(r => r.Floor)!.ThenInclude(f => f.Building)
            .Where(r => q == "" || r.Name.Contains(q) || (r.Description != null && r.Description.Contains(q)))
            .OrderBy(r => r.Floor!.Building!.Code).ThenBy(r => r.Floor!.Level).ThenBy(r => r.Name)
            .Take(100)
            .ToListAsync();

        var vm = new RoomSearchViewModel { Query = q, Results = results };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Route(int? fromRoomId, int? toRoomId, bool elevatorOnly = false)
    {
        var rooms = await _db.Rooms.AsNoTracking()
            .Include(r => r.Floor)!.ThenInclude(f => f.Building)
            .OrderBy(r => r.Floor!.Building!.Code).ThenBy(r => r.Floor!.Level).ThenBy(r => r.Name)
            .ToListAsync();

        var vm = new RouteViewModel
        {
            Rooms = rooms,
            FromRoomId = fromRoomId,
            ToRoomId = toRoomId,
            ElevatorOnly = elevatorOnly
        };

        if (fromRoomId.HasValue && toRoomId.HasValue)
        {
            var fromRoom = rooms.FirstOrDefault(r => r.Id == fromRoomId.Value);
            var toRoom = rooms.FirstOrDefault(r => r.Id == toRoomId.Value);

            if (fromRoom?.EntryPointId != null && toRoom?.EntryPointId != null)
            {
                var route = await _route.FindShortestRouteAsync(fromRoom.EntryPointId.Value, toRoom.EntryPointId.Value, elevatorOnly);
                vm.RouteResult = route;
                vm.FromRoom = fromRoom;
                vm.ToRoom = toRoom;
            }
        }

        return View(vm);
    }

    public async Task<IActionResult> Go(int id)
    {
        var dir = await _db.Directions.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (dir == null || !dir.IsActive) return NotFound();

        if (dir.ToRoomId.HasValue) return RedirectToAction(nameof(Room), new { id = dir.ToRoomId.Value });
        if (dir.ToPointId.HasValue) return RedirectToAction(nameof(Point), new { id = dir.ToPointId.Value });

        return RedirectToAction(nameof(Index));
    }
}
