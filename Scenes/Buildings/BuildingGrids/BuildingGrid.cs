using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectPlantris.Managers;

namespace ProjectPlantris.Scenes.Buildings.BuildingGrids;

[Tool]
public abstract partial class BuildingGrid : Node2D
{
    public List<Plot> Grid { get; } = [];
    public new Transform2D Transform { get; set; }

    public abstract void CreateGrid(int gridWidth, int gridHeight, float buildingWidth, float buildingHeight,
        float buildingAngle, Vector2 gridOffset, List<BuildingLayoutGap> gaps);

    public Plot? GetPlot(int x, int y) => Grid.Find(p => p.X == x && p.Y == y);

    public override void _Ready()
    {
        MovementController.Instance.FlowerMoved += QueueRedraw;
        BuildingSelector.Instance.BuildingChanged += _ => QueueRedraw();

        QueueRedraw();
    }

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

    public override void _Draw()
    {
        Grid.Sort();
        var i = 0;
        foreach (var plot in Grid)
        {
            MoveChild(plot, i++);
            plot.QueueRedraw();
        }

        base._Draw();
    }

    public bool HasFreeSpot()
    {
        return Grid.Any(p => p.IsFree());
    }
}