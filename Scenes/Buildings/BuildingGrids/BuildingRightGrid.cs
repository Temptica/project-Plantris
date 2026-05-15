using Godot;

namespace ProjectPlantris.Scenes.Buildings.BuildingGrids;

[Tool]
public partial class BuildingRightGrid : BuildingGrid
{
    public override void CreateGrid(int gridWidth, int gridHeight, float buildingWidth, float buildingHeight,
        float buildingAngle, Vector2 gridOffset)
    {
        var cellSizeHeight = buildingHeight / gridHeight;
        var cellSizeWidth = buildingWidth / gridWidth;
        var radCol = Mathf.DegToRad(-buildingAngle);
        
        var v = new Vector2(0, -cellSizeHeight);
        var u = new Vector2(Mathf.Cos(radCol), Mathf.Sin(radCol)) * cellSizeWidth;
        Transform = new Transform2D(u, v, Vector2.Zero);

        for (var row = 0; row < gridHeight; row++)
        {
            for (var col = 0; col < gridWidth; col++)
            {
                var pos = col * u + row * v;
                pos += gridOffset;

                // To draw skewed textures correctly, we use Transform2D
                // The basis vectors u and v define the axes of our skewed grid

                var plot = Plot.Create(row, col, pos, this);
                Grid.Add(plot);
                AddChild(plot);
            }
        }
    }
}