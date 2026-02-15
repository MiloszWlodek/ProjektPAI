using System.ComponentModel.DataAnnotations;

namespace InfoInfo.Models.Campus;

public class Floor
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Budynek")]
    public int BuildingId { get; set; }

    public Building? Building { get; set; }

    [Required(ErrorMessage = "Nazwa piętra jest wymagana.")]
    [StringLength(80)]
    [Display(Name = "Nazwa piętra")]
    public string Name { get; set; } = string.Empty; // np. Parter, I piętro

    [Display(Name = "Poziom (liczba)")]
    public int Level { get; set; } // 0=parter, 1=1 piętro

    [StringLength(300)]
    [Display(Name = "Plik planu (opcjonalnie)")]
    public string? PlanImagePath { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<MovementPoint> MovementPoints { get; set; } = new List<MovementPoint>();
}
