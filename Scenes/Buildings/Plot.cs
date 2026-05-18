using System;
using Godot;
using ProjectPlantris.Scenes.Buildings.BuildingGrids;
using ProjectPlantris.Scenes.Flowers;

namespace ProjectPlantris.Scenes.Buildings;

[Tool]
public partial class Plot : Node2D, IComparable
{
    public int X { get; set; }
    public int Y { get; set; }

    public BuildingGrid Grid { get; private set; } = null!;
    public bool IsLeft { get; private set; }

    public FlowerPiece? CurrentFlowerPiece { get; private set; }
    public FlowerPiece? FlowerPiece { get; private set; }

    public bool IsEnabled;

    public PlotState State => GetState();

    private PlotState GetState()
    {
        if(!IsEnabled) return PlotState.Inactive;
        if ( FlowerPiece != null) return PlotState.Placed;
        return CurrentFlowerPiece is {IsFake:false} ? PlotState.Selected : PlotState.Available;

    }
    public static Plot Create(int x, int y, Vector2 position, BuildingGrid grid, bool isLeft = false)
    {
        var plot = new Plot
        {
            X = x,
            Y = y,
            Position = position,
            Grid = grid,
            IsLeft = isLeft,
        };
        return plot;
    }


    public override void _Draw()
    {
        DrawSetTransformMatrix(Grid.Transform);

        var rect = new Rect2(0, 0, 0.98f, 0.98f);
        var color = State switch
        {
            PlotState.Inactive => Colors.Transparent,
            PlotState.Available => Colors.White,
            PlotState.Selected => Colors.Yellow,
            PlotState.Placed => Colors.DarkRed,
            _ => Colors.RebeccaPurple,
        };

        DrawRect(rect, color, false, width: -2);
    }

    public bool IsAvailable() => State == PlotState.Available;

    public void SetPiece()
    {
        if (CurrentFlowerPiece == null) return;

        if (!CurrentFlowerPiece.IsFake)
        {
            FlowerPiece = CurrentFlowerPiece;
        }
    }

    public void SetCurrentPiece(FlowerPiece piece)
    {
        CurrentFlowerPiece = piece;
        CurrentFlowerPiece.Plot = this;
    }

    public void RemoveCurrent()
    {
        if (CurrentFlowerPiece == null) return;

        if (CurrentFlowerPiece.Plot == this)
        {
            CurrentFlowerPiece.Plot = null;
        }

        CurrentFlowerPiece = null;
    }

    public bool CanSet()
    {
        return CurrentFlowerPiece == null || CurrentFlowerPiece.IsFake || FlowerPiece == null;
    }

    public bool IsFree()
    {
        return FlowerPiece == null;
    }

    protected override void Dispose(bool disposing)
    {
        Grid = null!;
        CurrentFlowerPiece?.Plot = null;
        CurrentFlowerPiece = null;
        FlowerPiece = null;
        base.Dispose(disposing);
    }

    public void Enable()
    {
        IsEnabled  = true;
    }

    public void Disable()
    {
        IsEnabled  = false;
    }

    public int CompareTo(object? obj)
    {
        return obj is not Plot other ? 1 : State.CompareTo(other.State);
    }
}