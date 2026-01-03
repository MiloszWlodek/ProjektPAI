namespace ProjektPAI.Models;

public class MovementPoint
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }

    public int FloorId { get; set; }

    public ICollection<Direction> Directions { get; set; }
}
