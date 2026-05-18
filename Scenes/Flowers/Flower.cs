using Godot;
using Godot.Collections;
using ProjectPlantris.Managers;

namespace ProjectPlantris.Scenes.Flowers;

[GlobalClass]
public partial class Flower : Resource
{
    public enum FlowerType
    {
        Normal,
        Bottom,
        Top,
    }

    [Export] public required string FlowerName { get; set; }
    [Export] public Array<FlowerPiece> Sprites { get; set; } = [new FlowerPiece()];
    [Export] public required Texture2D Texture { get; set; }
    [Export] public FlowerType Type { get; set; } = FlowerType.Normal;
    [Export] public bool AllowRoof { get; set; } = true;

    public Sprite2D Sprite { get; set; } = null!;
    public Vector2 GridPosition { get; set; }
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
    public FlowerPiece LeftBottomPiece { get; private set; } = null!;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public Flower Copy()
    {
        if (Width == 0)
        {
            Initialize();
        }

        var copy = new Flower
        {
            FlowerName = FlowerName,
            Sprites = Sprites,
            Texture = Texture,
            MinX = MinX,
            MaxX = MaxX,
            MinY = MinY,
            MaxY = MaxY,
            Width = Width,
            Height = Height,
            LeftBottomPiece = LeftBottomPiece,
            Type = Type,
        };

        copy.SetSprite();

        return copy;
    }

    public void Initialize()
    {
        if (Sprites.Count == 0) return;

        MinX = Sprites[0].X;
        MaxX = Sprites[0].X;
        MinY = Sprites[0].Y;
        MaxY = Sprites[0].Y;

        foreach (var sprite in Sprites)
        {
            MinX = Mathf.Min(MinX, sprite.X);
            MaxX = Mathf.Max(MaxX, sprite.X);
            MinY = Mathf.Min(MinY, sprite.Y);
            MaxY = Mathf.Max(MaxY, sprite.Y);

            if (sprite is { X: 0, Y: 0 })
            {
                LeftBottomPiece = sprite;
            }
        }

        Width = MaxX - MinX + 1;
        Height = MaxY - MinY + 1;

        SetSprite();
    }

    private void SetSprite()
    {
        Sprite = new Sprite2D
        {
            Texture = Texture,
        };
    }

    public void Confirm()
    {
    }

    public Sprite2D ShowSprite()
    {
        Sprite.Show();
        return Sprite;
    }

    public Sprite2D HideSprite()
    {
        Sprite.Hide();
        return Sprite;
    }

    public void SetPosition(Transform2D transform)
    {
        var plot = LeftBottomPiece.Plot;
        if (plot is null || BuildingSelector.Instance.CurrentBuilding is null) return;

        Sprite.FlipH = plot.IsLeft;

        if (Sprite.GetParent() is null)
        {
            plot.AddChild(Sprite);
        }
        else
        {
            Sprite.Reparent(plot);
        }

        var xDirection = plot.IsLeft ? 1f : -1f;
    
        const float yDirection = 1f; 

        var localCenter = new Vector2(
            0.5f + (Width - 1) / 2f * xDirection,
            0.5f + (Height - 1) / 2f * yDirection
        );
        
        Sprite.Position = transform * localCenter;
    }

    public void SetColor(Color color)
    {
        Sprite.Modulate = color;
    }
}