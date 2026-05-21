using Godot;
using ProjectPlantris.Scenes.Buildings;

namespace ProjectPlantris.Scenes.Flowers;

[GlobalClass]
public partial class FlowerPiece : Resource
{
    [Export] public int X { get; set; }
    [Export] public int Y { get; set; }
    [Export] public bool IsFake { get; set; }
    [Export] public bool IsAttachmentPoint { get; set; }

    public bool IsEmptyForRoof => Y != 0;
    public Plot? Plot { get; set; }
}