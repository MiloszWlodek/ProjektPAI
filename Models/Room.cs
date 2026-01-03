namnamespace ProjektPAI.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }

    public int FloorId { get; set; }
}
