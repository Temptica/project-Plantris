using Godot;

namespace ProjectPlantris.Scenes.Buildings.BuildingGrids;

public partial class BuildingRoofGrid : BuildingGrid
{
    public override void CreateGrid(int gridWidth, int gridHeight, float buildingWidth, float buildingHeight,
        float buildingAngle, Vector2 gridOffset)
    {
        var cellSizeWidth = buildingWidth / gridWidth;
        var cellSizeDepth = buildingHeight / gridHeight;
        var radCol = Mathf.DegToRad(-buildingAngle);
        var radDepth = Mathf.DegToRad(buildingAngle);
        
        var d = new Vector2(Mathf.Cos(radDepth), Mathf.Sin(radDepth)) * cellSizeDepth;
        var u = new Vector2(Mathf.Cos(radCol), Mathf.Sin(radCol)) * cellSizeWidth;
        Transform = new Transform2D(u, -d, Vector2.Zero);
        
        for (var row = 0; row < gridHeight; row++) // y
        {
            for (var col = 0; col < gridWidth; col++) // x
            {
                var pos = col * u + row * -d;

                pos += gridOffset;

                // To draw skewed textures correctly, we use Transform2D
                // The basis vectors u and v define the axes of our skewed grid

                var plot = Plot.Create(col, row, pos, this);
                Grid.Add(plot);
                AddChild(plot);
            }
        }

        Grid.Sort();
    }

    public override void _Draw()
    {
        GD.Print("draw roof");
        base._Draw();
    }
}