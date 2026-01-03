namespace ProjektPAI.Models;

public class Floor
{
    public int Id { get; set; }
    public int Level { get; set; }

    public int BuildingId { get; set; }
    public Building Building { get; set; }

    public ICollection<MovementPoint> MovementPoints { get; set; }
}
