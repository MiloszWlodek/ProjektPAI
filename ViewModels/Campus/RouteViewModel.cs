using InfoInfo.Models.Campus;
using InfoInfo.Services;

namespace InfoInfo.ViewModels.Campus;

public class RouteViewModel
{
    public List<Room> Rooms { get; set; } = new();

    public int? FromRoomId { get; set; }
    public int? ToRoomId { get; set; }
    public bool ElevatorOnly { get; set; }

    public Room? FromRoom { get; set; }
    public Room? ToRoom { get; set; }

    public RouteResult? RouteResult { get; set; }
}
