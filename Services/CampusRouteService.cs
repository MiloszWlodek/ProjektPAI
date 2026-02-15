using InfoInfo.Data;
using InfoInfo.Models.Campus;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Services;

public record RouteStep(int FromPointId, DirectionKind Kind, int? ToPointId, int? ToRoomId, int WeightMeters);
public record RouteResult(int TotalMeters, IReadOnlyList<RouteStep> Steps, IReadOnlyList<int> VisitedPointIds);

public class CampusRouteService
{
    private readonly ApplicationDbContext _db;

    public CampusRouteService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<RouteResult?> FindShortestRouteAsync(int fromPointId, int toPointId, bool elevatorOnly, CancellationToken ct = default)
    {
        // Dijkstra po grafie (punkty ruchu), wagi w metrach
        var points = await _db.MovementPoints.AsNoTracking().ToListAsync(ct);
        var dirs = await _db.Directions.AsNoTracking()
            .Where(d => d.IsActive && d.ToPointId != null)
            .ToListAsync(ct);

        if (!points.Any(p => p.Id == fromPointId) || !points.Any(p => p.Id == toPointId))
            return null;

        var dist = new Dictionary<int, int>();
        var prev = new Dictionary<int, (int From, DirectionKind Kind, int Weight)>();
        var visited = new HashSet<int>();

        foreach (var p in points)
            dist[p.Id] = int.MaxValue;

        dist[fromPointId] = 0;

        // prosta kolejka priorytetowa
        var pq = new PriorityQueue<int, int>();
        pq.Enqueue(fromPointId, 0);

        while (pq.Count > 0)
        {
            var u = pq.Dequeue();
            if (visited.Contains(u)) continue;
            visited.Add(u);

            if (u == toPointId) break;

            foreach (var e in dirs.Where(d => d.FromPointId == u))
            {
                if (elevatorOnly && !e.ElevatorFriendly) continue;
                if (e.ToPointId == null) continue;

                var v = e.ToPointId.Value;
                var w = Math.Max(0, e.WeightMeters);

                var alt = dist[u] == int.MaxValue ? int.MaxValue : dist[u] + w;
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = (u, e.Kind, w);
                    pq.Enqueue(v, alt);
                }
            }
        }

        if (dist[toPointId] == int.MaxValue) return null;

        // odtworzenie ścieżki
        var steps = new List<RouteStep>();
        var cur = toPointId;
        while (cur != fromPointId)
        {
            if (!prev.TryGetValue(cur, out var p)) break;
            steps.Add(new RouteStep(p.From, p.Kind, cur, null, p.Weight));
            cur = p.From;
        }
        steps.Reverse();

        return new RouteResult(dist[toPointId], steps, visited.ToList());
    }
}
