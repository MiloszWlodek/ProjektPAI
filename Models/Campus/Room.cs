using System.ComponentModel.DataAnnotations;

namespace InfoInfo.Models.Campus;

public class Room
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Piętro")]
    public int FloorId { get; set; }
    public Floor? Floor { get; set; }

    [Required(ErrorMessage = "Nazwa sali jest wymagana.")]
    [StringLength(80)]
    [Display(Name = "Nazwa / numer sali")]
    public string Name { get; set; } = string.Empty; // np. 14, Sekretariat, Biblioteka

    [Display(Name = "Typ")]
    public RoomType RoomType { get; set; } = RoomType.Classroom;

    [StringLength(600)]
    [Display(Name = "Opis")]
    public string? Description { get; set; }

    [StringLength(300)]
    [Display(Name = "Ścieżka zdjęcia")]
    public string? PhotoPath { get; set; }

    [StringLength(300)]
    [Display(Name = "Tekst alternatywny zdjęcia")]
    public string? AltText { get; set; }

    [StringLength(300)]
    [Display(Name = "Ścieżka audio (opcjonalnie)")]
    public string? AudioPath { get; set; }

    // Jeśli sala jest dostępna z konkretnego punktu (np. wejście)
    [Display(Name = "Punkt wejścia (opcjonalnie)")]
    public int? EntryPointId { get; set; }
    public MovementPoint? EntryPoint { get; set; }
}
