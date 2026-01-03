using System.Drawing;

namespace ProjektPAI.Models;

public class Building
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }

    public ICollection<Floor> Floors { get; set; }
}
