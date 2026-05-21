using System;
using Godot;
using ProjectPlantris.Managers;
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
    public bool IsRoof { get; private set; }

    public FlowerPiece? CurrentFlowerPiece { get; private set; }
    public FlowerPiece? FlowerPiece { get; private set; }

    public bool IsEnabled;
    public bool IsGap;

    public PlotState State => GetState();

    public Area2D PlacementArea = null!;

    private PlotState GetState()
    {
        if (!IsEnabled) return PlotState.Inactive;
        if (FlowerPiece != null) return PlotState.Placed;
        return CurrentFlowerPiece is { IsFake: false } ? PlotState.Selected : PlotState.Available;
    }

    public static Plot Create(int x, int y, Vector2 position, BuildingGrid grid, bool isLeft = false,
        bool isGap = false, bool isRoof = false)
    {
        var plot = new Plot
        {
            X = x,
            Y = y,
            Position = position,
            Grid = grid,
            IsLeft = isLeft,
            IsGap = isGap,
            IsRoof = isRoof
        };

        if (Engine.IsEditorHint()) return plot;

        plot.PlacementArea = new Area2D();
        plot.AddChild(plot.PlacementArea);
        var collisionShape = new CollisionShape2D()
        {
            Shape = new RectangleShape2D()
            {
                Size = Vector2.One
            },
            Transform = grid.Transform,
            Position = grid.Transform * new Vector2(0.5f, 0.5f)
        };

        plot.PlacementArea.AddChild(collisionShape);

        return plot;
    }

    public override void _Draw()
    {
        DrawSetTransformMatrix(Grid.Transform);

        var rect = new Rect2(0, 0, 0.98f, 0.98f);
        var color = State switch
        {
            PlotState.Inactive => Colors.Transparent,
            PlotState.Available when IsGap => Colors.DarkSlateGray,
            PlotState.Available when IsAvailableForType() => Colors.White,
            PlotState.Available => Colors.DarkSlateGray,
            PlotState.Selected when CurrentFlowerPiece?.IsAttachmentPoint ?? false => Colors.DeepSkyBlue,
            PlotState.Selected => Colors.Yellow,
            PlotState.Placed => Colors.DarkRed,
            _ => Colors.RebeccaPurple,
        };

        DrawRect(rect, color, false, width: -4);
    }

    private bool IsAvailableForType()
    {
        if (IsGap) return false;
        var currentBuilding = BuildingSelector.CurrentBuilding;
        var currentFlower = BuildingSelector.CurrentBuilding?.CurrentFlower;
        
        if(currentBuilding is null || currentFlower is null) return true;

        if (currentFlower.AllowRoof && IsRoof) return true;
        if(IsRoof) return false;

        switch (currentFlower.Type)
        {
            case Flower.FlowerType.Top: return Y == currentBuilding.Height - 1;
            case Flower.FlowerType.Bottom: return Y == 0;
            case Flower.FlowerType.Normal:
            default: return true;
        }
    }

    public void SetPiece()
    {
        if (CurrentFlowerPiece == null) return;

        if (!CurrentFlowerPiece.IsFake || (IsRoof && CurrentFlowerPiece.IsEmptyForRoof))
        {
            FlowerPiece = CurrentFlowerPiece;
        }
    }

    public void SetCurrentPiece(FlowerPiece piece)
    {
        if (IsRoof && piece.IsEmptyForRoof) return;
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
        if (CurrentFlowerPiece == null || CurrentFlowerPiece.IsFake) return true;

        // Options:
        // - Normal piece but already had a flower piece, return false
        // - Any piece, this is a gap, only check for attachment points to be occupied
        // - Roof piece, only check for attachment points

        if (IsRoof && !CurrentFlowerPiece.IsAttachmentPoint) return true;
        
        if (FlowerPiece != null) return false;
        
        return !CurrentFlowerPiece.IsAttachmentPoint || !IsGap;
    }

    public bool IsFree()
    {
        return FlowerPiece == null && !IsGap;
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
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    public int CompareTo(object? obj)
    {
        return obj is not Plot other ? 1 : State.CompareTo(other.State);
    }
}