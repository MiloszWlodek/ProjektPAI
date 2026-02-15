using InfoInfo.Models.Campus;

namespace InfoInfo.ViewModels.Campus;

public class RoomSearchViewModel
{
    public string Query { get; set; } = string.Empty;
    public List<Room> Results { get; set; } = new();
}
