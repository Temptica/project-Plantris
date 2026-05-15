using System.Collections.Generic;
using Godot;

namespace ProjectPlantris.Scenes.Buildings.BuildingGrids;

[Tool]
public abstract partial class BuildingGrid : Node2D
{
    public List<Plot> Grid { get; } = [];
    public new Transform2D Transform { get; set; }
    
    public abstract void CreateGrid(int gridWidth, int gridHeight, float buildingWidth, float buildingHeight, float buildingAngle, Vector2 gridOffset);
    
    public Plot? GetPlot(int x, int y) => Grid.Find(p => p.X == x && p.Y == y);

    protected override void Dispose(bool disposing)
    {
        foreach (var plot in Grid)
        {
            plot.Dispose();
        }
        
        Grid.Clear();
        base.Dispose(disposing);
    }
    
    public void Enable()
    {
        foreach (var plot in Grid)
        {
            plot.Enable();
        }
    }
    
    public void Disable()
    {
        foreach (var plot in Grid)
        {
            plot.Disable();
        }
    }
}