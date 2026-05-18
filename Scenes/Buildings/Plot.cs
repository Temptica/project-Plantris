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

    public PlotState State { get; private set; }

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

        var rect = new Rect2(0, 0, 1, 1);
        var color = State switch
        {
            PlotState.Inactive => Colors.Transparent,
            PlotState.Available => Colors.White,
            PlotState.Selected => Colors.Yellow,
            PlotState.Placed => Colors.DarkRed,
            _ => Colors.RebeccaPurple,
        };
        
        DrawRect(rect, color, false, width:-2);
    }
    
    public bool IsAvailable() => State == PlotState.Available;
    
    public void SetPiece()
    {
        if (CurrentFlowerPiece == null) return;
        
        if (!CurrentFlowerPiece.IsFake)
        {
            State = PlotState.Placed;
            FlowerPiece = CurrentFlowerPiece;
        }
    }
    
    public void SetCurrentPiece(FlowerPiece piece)
    {
        CurrentFlowerPiece = piece;
        CurrentFlowerPiece.Plot = this;

        if (piece.IsFake)
        {
            State = State == PlotState.Placed ? PlotState.Placed : PlotState.Available;
            return;
        }
        
        State = PlotState.Selected;
    }
    
    public void RemoveCurrent()
    {
        if(CurrentFlowerPiece == null) return;
        
        if(CurrentFlowerPiece.Plot == this)
        {
            CurrentFlowerPiece.Plot = null;
        }
        
        CurrentFlowerPiece = null;
        State = PlotState.Available;
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
        State = PlotState.Available;
    }
    
    public void Disable()
    {
        State = PlotState.Inactive;
    }

    public int CompareTo(object? obj)
    {
        return obj is not Plot other ? 1 : State.CompareTo(other.State);
    }
}