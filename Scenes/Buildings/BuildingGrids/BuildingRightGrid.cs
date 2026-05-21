using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ProjectPlantris.Scenes.Buildings.BuildingGrids;

[Tool]
public partial class BuildingRightGrid : BuildingGrid
{
    public override void CreateGrid(int gridWidth, int gridHeight, float buildingWidth, float buildingHeight,
        float buildingAngle, Vector2 gridOffset, List<BuildingLayoutGap> gaps)
    {
        var cellSizeHeight = buildingHeight / gridHeight;
        var cellSizeWidth = buildingWidth / gridWidth;
        var radCol = Mathf.DegToRad(-buildingAngle);
        
        var v = new Vector2(0, -cellSizeHeight);
        var u = new Vector2(Mathf.Cos(radCol), Mathf.Sin(radCol)) * cellSizeWidth;
        Transform = new Transform2D(u, v, Vector2.Zero);

        for (var row = 0; row < gridHeight; row++) // y
        {
            for (var col = 0; col < gridWidth; col++) // x
            {
                var pos = col * u + row * v;
                pos += gridOffset;

                var plot = Plot.Create(col, row, pos, this);
                if(gaps.Any(g => g.Gaps.Any(gn => gn.Equals(plot)))) plot.IsGap = true;
                Plots.Add(plot);
                AddChild(plot,OS.IsDebugBuild());
            }
        }
        
        Plots.Sort();
    }
}