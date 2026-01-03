using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektPAI.Data;
using ProjektPAI.Models;

namespace ProjektPAI.Controllers;

public class RouteController : Controller
{
    private readonly ApplicationDbContext _context;

    public RouteController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Find(int fromPointId, int toPointId)
    {
        var points = _context.MovementPoints
            .Include(p => p.Directions)
            .ToList();

        var distances = new Dictionary<int, double>();
        var previous = new Dictionary<int, int?>();
        var unvisited = new List<int>();

        foreach (var p in points)
        {
            distances[p.Id] = double.MaxValue;
            previous[p.Id] = null;
            unvisited.Add(p.Id);
        }

        distances[fromPointId] = 0;

        while (unvisited.Any())
        {
            var current = unvisited
                .OrderBy(p => distances[p])
                .First();

            unvisited.Remove(current);

            if (current == toPointId)
                break;

            var point = points.First(p => p.Id == current);

            foreach (var d in point.Directions)
            {
                if (!d.ToPointId.HasValue) continue;

                var alt = distances[current] + d.Weight;
                if (alt < distances[d.ToPointId.Value])
                {
                    distances[d.ToPointId.Value] = alt;
                    previous[d.ToPointId.Value] = current;
                }
            }
        }

        var path = new List<int>();
        int? step = toPointId;

        while (step != null)
        {
            path.Insert(0, step.Value);
            step = previous[step.Value];
        }

        ViewBag.Path = path;
        return View();
    }
}