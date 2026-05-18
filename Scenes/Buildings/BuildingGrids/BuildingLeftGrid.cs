using Godot;

namespace ProjectPlantris.Scenes.Buildings.BuildingGrids;

[Tool]
public partial class BuildingLeftGrid : BuildingGrid
{
    public override void CreateGrid(int gridWidth, int gridHeight, float buildingWidth, float buildingHeight,
        float buildingAngle, Vector2 gridOffset)
    {
        var cellSizeHeight = buildingHeight / gridHeight;
        var cellSizeDepth = buildingWidth / gridWidth;
        var radDepth = Mathf.DegToRad(buildingAngle);

        var v = new Vector2(0, -cellSizeHeight);
        var u = new Vector2(Mathf.Cos(radDepth), Mathf.Sin(radDepth)) * cellSizeDepth;
        Transform = new Transform2D(u, v, Vector2.Zero);

        for (var row = 0; row < gridHeight; row++)// (y)
        {
            for (var col = 0; col < gridWidth; col++) // (x)
            {
                var pos = (-col - 1) * u + row * v;
                pos += gridOffset;

                // To draw skewed textures correctly, we use Transform2D
                // The basis vectors u and v define the axes of our skewed grid
                var plot = Plot.Create(col, row, pos, this, true);
                Grid.Add(plot);
                AddChild(plot);
            }
        }
        
        Grid.Sort();
    }
}