using ProjektPAI.Models;

namespace ProjektPAI.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (context.Buildings.Any())
            return;

        var building = new Building
        {
            Name = "Budynek A",
            Description = "Zachodnia część budynku A – sale komputerowe",
            PhotoUrl = "/images/budynek_a.jpg",
            Floors = new List<Floor>()
        };

        var floor = new Floor
        {
            Level = 0,
            MovementPoints = new List<MovementPoint>()
        };

        var point1 = new MovementPoint
        {
            Description = "Wejście do zachodniej części",
            PhotoUrl = "/images/a_wejscie.jpg",
            Directions = new List<Direction>()
        };

        var point2 = new MovementPoint
        {
            Description = "Korytarz z salami komputerowymi",
            PhotoUrl = "/images/a_korytarz.jpg",
            Directions = new List<Direction>()
        };

        var room = new Room
        {
            Name = "Sala A-12",
            Description = "Sala komputerowa",
            PhotoUrl = "/images/a12.jpg"
        };

        context.Rooms.Add(room);
        context.SaveChanges();

        point1.Directions.Add(new Direction
        {
            Type = DirectionType.Forward,
            Weight = 5,
            ToPointId = point2.Id
        });

        point2.Directions.Add(new Direction
        {
            Type = DirectionType.Forward,
            Weight = 3,
            ToRoomId = room.Id
        });

        floor.MovementPoints.Add(point1);
        floor.MovementPoints.Add(point2);

        building.Floors.Add(floor);

        context.Buildings.Add(building);
        context.SaveChanges();
    }
}