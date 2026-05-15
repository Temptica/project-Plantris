using Godot;
using ProjectPlantris.Scenes.Buildings;

namespace ProjectPlantris.Scenes.Flowers;

[GlobalClass]
public partial class FlowerPiece : Resource
{
    [Export] public int X { get; set; }
    [Export] public int Y { get; set; }
    [Export] public bool IsFake { get; set; }

    public Plot? Plot { get; set; }
}