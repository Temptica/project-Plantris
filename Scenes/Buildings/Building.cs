using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectPlantris.Scenes.Buildings.BuildingGrids;
using ProjectPlantris.Scenes.Flowers;

namespace ProjectPlantris.Scenes.Buildings;

[Tool]
public partial class Building : Node2D
{
    [Export] public BuildingLayoutResource LayoutResource { get; set; } = new();

    [Export] public bool TurnOnDebug;

    [Export] public bool HasLeftGrid { get; set; } = true;
    [Export] public bool HasRightGrid { get; set; } = true;
    [Export] public bool HasRoofGrid { get; set; }

    [ExportToolButton("refresh", Icon = "Reload")]
    private Callable Refresh => Callable.From(UpdateTexture);

    public BuildingLeftGrid? LeftGrid { get; private set; }
    public BuildingRightGrid? RightGrid { get; private set; }
    public BuildingRoofGrid? RoofGrid { get; private set; }

    public List<BuildingGrid> Grids = [];

    public int PlotCount;

    public int Depth => LayoutResource.Depth;
    public int Width => LayoutResource.Width;
    public int Height => LayoutResource.Height;

    private Sprite2D _sprite = null!;
    public Flower? CurrentFlower { get; private set; }

    [Signal]
    public delegate void FullEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Grids = [];
        if (HasRightGrid)
        {
            RightGrid = new BuildingRightGrid();
            RightGrid.CreateGrid(LayoutResource.Width, LayoutResource.Height, LayoutResource.BuildingWidth,
                LayoutResource.BuildingHeight, LayoutResource.BuildingAngle, LayoutResource.GridOffset,
                LayoutResource.Gaps.Where(g => g.RightGrid).ToList());
            AddChild(RightGrid,OS.IsDebugBuild());
            Grids.Add(RightGrid);
        }

        if (HasLeftGrid)
        {
            LeftGrid = new BuildingLeftGrid();
            LeftGrid.CreateGrid(LayoutResource.Depth, LayoutResource.Height, LayoutResource.BuildingDepth,
                LayoutResource.BuildingHeight, LayoutResource.BuildingAngle, LayoutResource.GridOffset,
                LayoutResource.Gaps.Where(g => g.LeftGrid).ToList());
            AddChild(LeftGrid, OS.IsDebugBuild());
            Grids.Add(LeftGrid);
        }

        if (HasRoofGrid)
        {
            RoofGrid = new BuildingRoofGrid();
            var gridOffset = LayoutResource.GridOffset + new Vector2(0, -LayoutResource.BuildingHeight);
            RoofGrid.CreateGrid(LayoutResource.Width, LayoutResource.Depth, LayoutResource.BuildingWidth,
                LayoutResource.BuildingDepth, LayoutResource.BuildingAngle, gridOffset,
                LayoutResource.Gaps.Where(g => g.RoofGrid).ToList());
            AddChild(RoofGrid,OS.IsDebugBuild());
            Grids.Add(RoofGrid);
        }

        _sprite = GetNode<Sprite2D>("Sprite2D");
        UpdateTexture();
        PlotCount = Grids.SelectMany(g => g.Plots).Count(p => !p.IsGap);
    }

    private void UpdateTexture()
    {
        _sprite.Texture = LayoutResource.Texture;
    }

    public override void _Process(double delta)
    {
        if (!TurnOnDebug || !Engine.IsEditorHint()) return;

        _sprite.Texture = LayoutResource.Texture;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (!TurnOnDebug || !Engine.IsEditorHint()) return;

        var cellSizeHeight = LayoutResource.BuildingHeight / LayoutResource.Height;
        var cellSizeWidth = LayoutResource.BuildingWidth / LayoutResource.Width;
        var cellSizeDepth = LayoutResource.BuildingDepth / LayoutResource.Depth;
        var radCol = Mathf.DegToRad(-LayoutResource.BuildingAngle);
        var radDepth = Mathf.DegToRad(LayoutResource.BuildingAngle);


        // Define basis vectors for the skew
        // We normalize them if we want to use them as a transform basis and keep drawing in 'cellSize' units
        // Or we use them directly and draw in '1.0' units.
        var u = new Vector2(Mathf.Cos(radCol), Mathf.Sin(radCol)) * cellSizeWidth;
        var v = new Vector2(0, -cellSizeHeight);
        var d = new Vector2(Mathf.Cos(radDepth), Mathf.Sin(radDepth)) * cellSizeDepth;

        if (HasRightGrid)
        {
            var gaps = LayoutResource.Gaps.Where(g => g?.RightGrid ?? false).SelectMany(g => g.Gaps).ToList();
            for (var row = 0; row < LayoutResource.Height; row++)
            {
                for (var col = 0; col < LayoutResource.Width; col++)
                {
                    var pos = col * u + row * v;
                    pos += LayoutResource.GridOffset;

                    // To draw skewed textures correctly, we use Transform2D
                    // The basis vectors u and v define the axes of our skewed grid
                    var transform = new Transform2D(u, v, pos);

                    DrawSetTransformMatrix(transform);

                    // Now we draw in the skewed local space.
                    // A unit rectangle here will be skewed according to u and v.
                    // Note: Rect2(0, -1, 1, 1) means the rectangle starts at the 'bottom'
                    // in our grid (where v is up-pointing) and goes 1 unit up.
                    var rect = new Rect2(0, 0, 0.98f, 0.98f);
                    var color = gaps.Any(g => g.X == col && g.Y == row) ? Colors.Red : Colors.RebeccaPurple;
                    DrawRect(rect, color, false);

                    // If you were to draw a texture, it would now be correctly skewed:
                    // DrawTextureRect(someTexture, rect, false);
                    DrawRect(new Rect2(-0.1f, -0.1f, 0.2f, 0.2f), Colors.Green);
                }
            }
        }

        if (HasLeftGrid)
        {
            var gaps = LayoutResource.Gaps.Where(g => g?.LeftGrid ?? false).SelectMany(g => g.Gaps).ToList();
            for (var row = 0; row < LayoutResource.Height; row++)
            {
                for (var col = LayoutResource.Depth; col > 0; col--)
                {
                    var pos = -col * d + row * v;
                    pos += LayoutResource.GridOffset;

                    // To draw skewed textures correctly, we use Transform2D
                    // The basis vectors u and v define the axes of our skewed grid
                    var transform = new Transform2D(d, v, pos);

                    DrawSetTransformMatrix(transform);

                    // Now we draw in the skewed local space.
                    // A unit rectangle here will be skewed according to u and v.
                    // Note: Rect2(0, -1, 1, 1) means the rectangle starts at the 'bottom'
                    // in our grid (where v is up-pointing) and goes 1 unit up.
                    var rect = new Rect2(0, 0, 0.98f, 0.98f);
                    var flippedCol = (Width - 1) - col; 

                    var color = gaps.Any(g => g.X == flippedCol && g.Y == row) 
                        ? Colors.Red 
                        : Colors.RebeccaPurple;
                    DrawRect(rect, color, false);

                    // If you were to draw a texture, it would now be correctly skewed:
                    // DrawTextureRect(someTexture, rect, false);
                    DrawRect(new Rect2(-0.1f, -0.1f, 0.2f, 0.2f), Colors.Green);
                }
            }
        }

        if (HasRoofGrid)
        {
            //Intentional ? due to editor
            var gaps = LayoutResource.Gaps.Where(g => g?.RoofGrid ?? false).SelectMany(g => g.Gaps).ToList();
            for (var row = 0; row < LayoutResource.Depth; row++)
            {
                for (var col = 0; col < LayoutResource.Width; col++)
                {
                    var pos = col * u + row * -d;

                    // If you want the roof grid at the TOP of the building instead of the bottom, 
                    // you can add the height vector: pos += LayoutResource.Height * v;
                    pos += LayoutResource.GridOffset + new Vector2(0, -LayoutResource.BuildingHeight);

                    // Transform2D takes X_basis, Y_basis, and Origin.
                    // For a flat floor, X is our width vector (u), and Y is our depth vector (-d).
                    var transform = new Transform2D(u, -d, pos);

                    DrawSetTransformMatrix(transform);

                    // Now we draw in the skewed local space.
                    // A unit rectangle here will be skewed according to u and v.
                    // Note: Rect2(0, -1, 1, 1) means the rectangle starts at the 'bottom'
                    // in our grid (where v is up-pointing) and goes 1 unit up.
                    var rect = new Rect2(0, 0, 0.98f, 0.98f);
                    var color = gaps.Any(g => g.X == col && g.Y == row) ? Colors.Red : Colors.RebeccaPurple;
                    DrawRect(rect, color, false);

                    // If you were to draw a texture, it would now be correctly skewed:
                    // DrawTextureRect(someTexture, rect, false);
                    DrawRect(new Rect2(-0.1f, -0.1f, 0.2f, 0.2f), Colors.Green);
                }
            }
        }

        // Reset transform
        DrawSetTransformMatrix(Transform2D.Identity);
    }

    private static Vector2 GetCellPosition(int row, int col, float cellWidth, float angleCol, float angleRow)
    {
        // Convert angles from degrees to radians
        var radCol = Mathf.DegToRad(angleCol);
        var radRow = Mathf.DegToRad(angleRow);

        // Define basis vectors
        var u = new Vector2(cellWidth * Mathf.Cos(radCol), cellWidth * Mathf.Sin(radCol)); // Column vector
        var v = new Vector2(cellWidth * Mathf.Cos(radRow), cellWidth * Mathf.Sin(radRow)); // Row vector

        // Calculate position
        return col * u + row * v;
    }

    public void PositionFlower(Flower? flower)
    {
        if (flower is null) return;

        foreach (var plot in Grids.SelectMany(g => g.Plots))
        {
            plot.RemoveCurrent();
        }

        var isFlipped = flower.GridPosition.X < 0;
        var isRoof = flower.GridPosition.Y >= Height;

        BuildingGrid grid = (isRoof
            ? RoofGrid
            : isFlipped
                ? LeftGrid
                : RightGrid)!;
        
        var maxLocalX = flower.Sprites.Count != 0 ? flower.Sprites.Max(p => p.X) : 0;

        foreach (var piece in flower.Sprites)
        {
            var piecePosition = new Vector2(piece.X, piece.Y);

            if (isRoof)
            {
                piecePosition.Y -= Height; 
                if (isFlipped)
                {
                    piecePosition.X = maxLocalX - piecePosition.X;
                }
            }
            else
            {
                if (isFlipped)
                {
                    piecePosition.X = maxLocalX - piecePosition.X - Mathf.FloorToInt((maxLocalX + 1) / 2F) + Depth;
                }
            }

            var position = flower.GridPosition + piecePosition;
            var slot = grid.GetPlot((int)position.X, (int)position.Y);

            slot?.SetCurrentPiece(piece);
        }

        if (CurrentFlower != flower)
        {
            if (CurrentFlower is not null)
            {
                var hideSprite = CurrentFlower.HideSprite();

                if (hideSprite.GetParent() == this)
                {
                    RemoveChild(hideSprite);
                }
            }

            var showSprite = flower.ShowSprite();

            if (showSprite.GetParent() is null)
            {
                AddChild(showSprite);
            }
            else
            {
                showSprite.Reparent(this);
            }

            CurrentFlower = flower;
        }

        flower.SetPosition(grid.Transform, isRoof);
        var color = Colors.White;

        if (!CanPlace())
        {
            color = isFlipped ? Colors.DarkRed : Colors.Firebrick;
        }
        else if (isFlipped)
        {
            color = Colors.Gray;
        }

        flower.SetColor(color);
    }

    public void PositionFlower(Flower flower, Plot plot)
    {
        flower.GridPosition = plot.Position - CalculateFlowerLocalCenter(flower);

        PositionFlower(flower);
    }

    private static Vector2 CalculateFlowerLocalCenter(Flower flower)
    {
        var minX = float.MaxValue;
        var maxX = float.MinValue;
        var minY = float.MaxValue;
        var maxY = float.MinValue;

        foreach (var piece in flower.Sprites)
        {
            if (piece.X < minX) minX = piece.X;
            if (piece.X > maxX) maxX = piece.X;
            if (piece.Y < minY) minY = piece.Y;
            if (piece.Y > maxY) maxY = piece.Y;
        }

        // Find the midpoint of the flower's bounding box
        var centerX = (minX + maxX) / 2f;
        var centerY = (minY + maxY) / 2f;

        // Round to nearest integer grid space so it snaps cleanly to slots
        return new Vector2(Mathf.Round(centerX), Mathf.Round(centerY));
    }

    private bool CanPlace()
    {
        return CurrentFlower is not null &&
               CurrentFlower.Sprites.All(flowerPiece => flowerPiece.Plot is not null && flowerPiece.Plot.CanSet());
    }

    public int GetFreeSpotsCount()
    {
        return Grids.SelectMany(g => g.Plots).Count(p => p.IsFree());
    }

    public bool TrySetFlower()
    {
        if (CurrentFlower == null || !CanPlace()) return false;

        foreach (var flowerPiece in CurrentFlower.Sprites)
        {
            flowerPiece.Plot?.SetPiece();
        }

        CurrentFlower.Confirm();
        CurrentFlower.SetColor(CurrentFlower.GridPosition.X >= 0 ? Colors.Gray : Colors.DarkGray);

        if (!Grids.Any(g => g.HasFreeSpot())) EmitSignalFull();

        return true;
    }

    public void RemoveCurrentFlower()
    {
        if (CurrentFlower is null) return;

        foreach (var plot in Grids.SelectMany(g => g.Plots))
        {
            plot.RemoveCurrent();
        }

        CurrentFlower.HideSprite();
    }

    public void Enable(Flower? flower = null)
    {
        foreach (var grid in Grids)
        {
            grid.Enable();
        }

        CurrentFlower = flower;
        PositionFlower(CurrentFlower);
    }

    public void Disable()
    {
        foreach (var grid in Grids)
        {
            grid.Disable();
        }

        RemoveCurrentFlower();
    }

    public bool HasTopSpace()
    {
        return (HasRightGrid && RightGrid!.Plots.Any(plot => plot.Y == Height - 1 && plot.IsFree())) ||
               (HasLeftGrid && LeftGrid!.Plots.Any(plot => plot.Y == Height - 1 && plot.IsFree()));
    }

    public bool HasBottomSpace()
    {
        return (HasRightGrid && RightGrid!.Plots.Any(plot => plot.Y == 0 && plot.IsFree())) ||
               (HasLeftGrid && LeftGrid!.Plots.Any(plot => plot.Y == 0 && plot.IsFree()));
    }

    public bool HasRoofSpace()
    {
        return RoofGrid?.HasFreeSpot() ?? false;
    }
}