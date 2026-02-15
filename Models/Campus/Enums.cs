using System.ComponentModel.DataAnnotations;

namespace InfoInfo.Models.Campus;

public enum DirectionKind
{
    [Display(Name = "Do przodu")]
    Forward = 0,

    [Display(Name = "W lewo")]
    Left = 1,

    [Display(Name = "W prawo")]
    Right = 2,

    [Display(Name = "Do tyłu")]
    Back = 3,
}

public enum RoomType
{
    [Display(Name = "Sala dydaktyczna")]
    Classroom = 0,

    [Display(Name = "Sala komputerowa")]
    ComputerLab = 1,

    [Display(Name = "Sekretariat")]
    Office = 2,

    [Display(Name = "Korytarz / komunikacja")]
    Corridor = 3,

    [Display(Name = "Biblioteka")]
    Library = 4,

    [Display(Name = "Inne")]
    Other = 99,
}
