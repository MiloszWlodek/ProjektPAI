using InfoInfo.Models.Campus;

namespace InfoInfo.ViewModels.Campus;

public class PointViewModel
{
    public MovementPoint Point { get; set; } = default!;

    public Direction? Forward { get; set; }
    public Direction? Left { get; set; }
    public Direction? Right { get; set; }
    public Direction? Back { get; set; }
}
