using Godot;
using Godot.Collections;
using ProjectPlantris.Managers;
using ProjectPlantris.Scenes.Buildings;

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

        copy.SetSpriteValues();

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
        Height = MaxY - MinY;

        SetSpriteValues();
    }

    private void SetSpriteValues()
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

    public void SetPosition()
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

        var offset = new Vector2(0, Height);

        if (Width > 1)
        {
            var unitOffset = (Width - 1) * BuildingSelector.Instance.CurrentBuilding.Unit / 2.0f;

            if (plot.IsLeft)
            {
                offset += new Vector2(0, unitOffset);
            }
            else
            {
                offset += new Vector2(-unitOffset, 0);
            }
        }

        Sprite.Position = offset;
    }

    public void SetColor(Color color)
    {
        Sprite.Modulate = color;
    }
}
