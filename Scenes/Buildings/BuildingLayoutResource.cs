using Godot;

namespace ProjectPlantris.Scenes.Buildings;

[GlobalClass, Tool]
public partial class BuildingLayoutResource : Resource
{
    [Export] public Texture2D Texture { get; set; } = null!;
    [Export] public int Depth { get; private set; }
    [Export] public int Width { get; private set; }
    [Export] public int Height { get; private set; }
    [Export] public float BuildingHeight { get; set; }
    [Export] public float BuildingWidth { get; set; }
    [Export] public float BuildingDepth { get; set; }
    [Export] public Vector2 GridOffset { get; set; }
    [Export] public float BuildingAngle { get; set; } = 20.75f;
}