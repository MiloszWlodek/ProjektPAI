using System.ComponentModel.DataAnnotations;

namespace InfoInfo.Models.Campus;

public class MovementPoint
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Piętro")]
    public int FloorId { get; set; }
    public Floor? Floor { get; set; }

    [Required(ErrorMessage = "Nazwa punktu jest wymagana.")]
    [StringLength(120)]
    [Display(Name = "Nazwa punktu")]
    public string Name { get; set; } = string.Empty;

    [StringLength(600)]
    [Display(Name = "Opis miejsca")]
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

    [StringLength(200)]
    [Display(Name = "Link do Matterport (opcjonalnie)")]
    public string? MatterportUrl { get; set; }

    public ICollection<Direction> FromDirections { get; set; } = new List<Direction>();
    public ICollection<Direction> ToDirections { get; set; } = new List<Direction>();
}
