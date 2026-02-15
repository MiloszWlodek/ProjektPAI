using System.ComponentModel.DataAnnotations;

namespace InfoInfo.Models.Campus;

public class Direction
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Punkt startowy")]
    public int FromPointId { get; set; }
    public MovementPoint? FromPoint { get; set; }

    [Required]
    [Display(Name = "Kierunek")]
    public DirectionKind Kind { get; set; }

    [Display(Name = "Aktywny")]
    public bool IsActive { get; set; } = true;

    [Range(0, 10000, ErrorMessage = "Waga (metry) musi być w zakresie 0..10000.")]
    [Display(Name = "Waga (metry)")]
    public int WeightMeters { get; set; } = 1;

    [Display(Name = "Tylko winda / dostępne bez schodów")]
    public bool ElevatorFriendly { get; set; } = true;

    // Docelowo: albo punkt, albo sala
    [Display(Name = "Docelowy punkt (opcjonalnie)")]
    public int? ToPointId { get; set; }
    public MovementPoint? ToPoint { get; set; }

    [Display(Name = "Docelowa sala (opcjonalnie)")]
    public int? ToRoomId { get; set; }
    public Room? ToRoom { get; set; }
}
