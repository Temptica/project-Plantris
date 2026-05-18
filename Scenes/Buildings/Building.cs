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

    public int Depth => LayoutResource.Depth;
    public int Width => LayoutResource.Width;
    public int Height => LayoutResource.Height;

    private Sprite2D _sprite = null!;
    private Flower? _currentFlower;

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
                LayoutResource.BuildingHeight, LayoutResource.BuildingAngle, LayoutResource.GridOffset);
            AddChild(RightGrid);
            Grids.Add(RightGrid);
        }

        if (HasLeftGrid)
        {
            LeftGrid = new BuildingLeftGrid();
            LeftGrid.CreateGrid(LayoutResource.Depth, LayoutResource.Height, LayoutResource.BuildingDepth,
                LayoutResource.BuildingHeight, LayoutResource.BuildingAngle, LayoutResource.GridOffset);
            AddChild(LeftGrid);
            Grids.Add(LeftGrid);
        }

        if (HasRoofGrid)
        {
            RoofGrid = new BuildingRoofGrid();
            var gridOffset = LayoutResource.GridOffset + new Vector2(0, LayoutResource.BuildingHeight);
            RoofGrid.CreateGrid(LayoutResource.Depth, LayoutResource.Width, LayoutResource.BuildingDepth,
                LayoutResource.BuildingHeight, LayoutResource.BuildingAngle, gridOffset);
            Grids.Add(RoofGrid);
        }

        _sprite = GetNode<Sprite2D>("Sprite2D");
        UpdateTexture();
    }

    private void UpdateTexture()
    {
        _sprite.Texture = LayoutResource.Texture;
    }

    public override void _Process(double delta)
    {
        if (!TurnOnDebug || (!OS.IsDebugBuild() && !Engine.IsEditorHint())) return;

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
                    var rect = new Rect2(0, 0, 1, 1);
                    DrawRect(rect, Colors.Magenta, false);

                    // If you were to draw a texture, it would now be correctly skewed:
                    // DrawTextureRect(someTexture, rect, false);
                    DrawRect(new Rect2(-0.1f, -0.1f, 0.2f, 0.2f), Colors.Green);
                }
            }
        }

        if (HasLeftGrid)
        {
            for (var row = 0; row < LayoutResource.Height; row++)
            {
                for (var col = 0; col < LayoutResource.Depth; col++)
                {
                    var pos = (-col - 1) * d + row * v;
                    pos += LayoutResource.GridOffset;

                    // To draw skewed textures correctly, we use Transform2D
                    // The basis vectors u and v define the axes of our skewed grid
                    var transform = new Transform2D(d, v, pos);

                    DrawSetTransformMatrix(transform);

                    // Now we draw in the skewed local space.
                    // A unit rectangle here will be skewed according to u and v.
                    // Note: Rect2(0, -1, 1, 1) means the rectangle starts at the 'bottom'
                    // in our grid (where v is up-pointing) and goes 1 unit up.
                    var rect = new Rect2(0, 0, 1, 1);
                    DrawRect(rect, Colors.Magenta, false);

                    // If you were to draw a texture, it would now be correctly skewed:
                    // DrawTextureRect(someTexture, rect, false);
                    DrawRect(new Rect2(-0.1f, -0.1f, 0.2f, 0.2f), Colors.Green);
                }
            }
        }

        if (HasRoofGrid)
        {
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
                    var rect = new Rect2(0, 0, 1, 1);
                    DrawRect(rect, Colors.Magenta, false);

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

        foreach (var plot in Grids.SelectMany(g => g.Grid))
        {
            plot.RemoveCurrent();
        }

        var isFlipped = flower.GridPosition.X < 0;
        var isRoof = flower.GridPosition.Y > Height;
        BuildingGrid grid = (isFlipped
            ? LeftGrid
            : isRoof
                ? RoofGrid
                : RightGrid)!;

        foreach (var piece in flower.Sprites)
        {
            var piecePosition = new Vector2(piece.X, piece.Y);

            if (isFlipped)
            {
                piecePosition.X *= -1;
            }
            else if (isRoof)
            {
                piecePosition.Y -= Height;
            }

            var position = flower.GridPosition + piecePosition;
            var slot = grid.GetPlot((int)position.X, (int)position.Y);

            slot?.SetCurrentPiece(piece);
        }

        if (_currentFlower != flower)
        {
            if (_currentFlower is not null)
            {
                var hideSprite = _currentFlower.HideSprite();

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

            _currentFlower = flower;
        }

        flower.SetPosition(grid.Transform);

        var flipped = flower.GridPosition.X < 0;
        var color = Colors.White;

        if (!CanPlace())
        {
            color = flipped ? Colors.DarkRed : Colors.Firebrick;
        }
        else if (flipped)
        {
            color = Colors.Gray;
        }

        flower.SetColor(color);
    }

    private bool CanPlace()
    {
        if (_currentFlower is null) return false;

        foreach (var flowerPiece in _currentFlower.Sprites)
        {
            if (flowerPiece.Plot is null || !flowerPiece.Plot.CanSet())
            {
                return false;
            }
        }

        return true;
    }

    public bool TrySetFlower()
    {
        if (_currentFlower == null || !CanPlace()) return false;

        foreach (var flowerPiece in _currentFlower.Sprites)
        {
            flowerPiece.Plot?.SetPiece();
        }

        _currentFlower.Confirm();
        _currentFlower.SetColor(_currentFlower.GridPosition.X >= 0 ? Colors.Gray : Colors.DarkGray);

        if (LeftGrid?.HasFreeSpot() ?? false) return true;
        if (RightGrid?.HasFreeSpot() ?? false) return true;
        if (RoofGrid?.HasFreeSpot() ?? false) return true;

        EmitSignalFull();

        return true;
    }

    public void RemoveCurrentFlower()
    {
        if (_currentFlower is null) return;

        if (LeftGrid != null)
        {
            foreach (var plot in LeftGrid.Grid)
            {
                plot.RemoveCurrent();
            }
        }

        if (RightGrid != null)
        {
            foreach (var plot in RightGrid.Grid)
            {
                plot.RemoveCurrent();
            }
        }

        _currentFlower.HideSprite();
    }

    public void Enable(Flower? flower = null)
    {
        LeftGrid?.Enable();
        RightGrid?.Enable();
        _currentFlower = flower;
        PositionFlower(_currentFlower);
    }

    public void Disable()
    {
        LeftGrid?.Disable();
        RightGrid?.Disable();
        RemoveCurrentFlower();
    }

    public bool HasTopSpace()
    {
        return (HasRightGrid && RightGrid!.Grid.Any(plot => plot.Y == Height - 1 && plot.IsFree())) ||
               (HasLeftGrid && LeftGrid!.Grid.Any(plot => plot.Y == Height - 1 && plot.IsFree()));
    }

    public bool HasBottomSpace()
    {
        return (HasRightGrid && RightGrid!.Grid.Any(plot => plot.Y == 0 && plot.IsFree())) ||
               (HasLeftGrid && LeftGrid!.Grid.Any(plot => plot.Y == 0 && plot.IsFree()));
    }
}