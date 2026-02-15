using System.ComponentModel.DataAnnotations;

namespace InfoInfo.Models.Campus;

public class Building
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa budynku jest wymagana.")]
    [StringLength(120, ErrorMessage = "Nazwa budynku może mieć maks. 120 znaków.")]
    [Display(Name = "Nazwa budynku")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Symbol / litera")]
    public string? Code { get; set; }

    [StringLength(300)]
    [Display(Name = "Adres / lokalizacja")]
    public string? Location { get; set; }

    [StringLength(500)]
    [Display(Name = "Opis")]
    public string? Description { get; set; }

    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
