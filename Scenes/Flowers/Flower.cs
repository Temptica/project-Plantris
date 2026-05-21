using System.Linq;
using Godot;
using Godot.Collections;
using ProjectPlantris.Managers;

namespace ProjectPlantris.Scenes.Flowers;

[Tool, GlobalClass]
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
    [Export] public bool AllowRoof { get; set; }

    [ExportCategory("Generation")] [Export(PropertyHint.MultilineText)]
    private string Format;

    [ExportToolButton("Generate Sprites")] private Callable GenerateSprites => Callable.From(GenerateSpritesByFormat);

    private void GenerateSpritesByFormat()
    {
        if (string.IsNullOrEmpty(Format)) return;
        
        Sprites.Clear();
        var lines = Format.Split('\n');
        var y = lines.Length-1;
        foreach (var line in lines)
        {
            var x = 0;
            while (x < line.Length)
            {
                switch (line[x])
                {
                    case 'X' or 'x':
                        Sprites.Add(new FlowerPiece { X = x, Y = y});
                        break;
                    case 'A' or 'a':
                        Sprites.Add(new FlowerPiece { X = x, Y = y, IsAttachmentPoint = true});
                        break;
                    case 'F' or 'f':
                        Sprites.Add(new FlowerPiece { X = x, Y = y, IsFake = true});
                        break;
                }

                x++;
            }

            y--;
        }
        
        if (!Sprites.Any(s => s is { X: 0, Y: 0 }))
        {
            Sprites.Add(new FlowerPiece { IsFake = true });
        }

        ResourceSaver.Save(this);
    }

    public Sprite2D Sprite { get; set; } = null!;
    public Vector2 GridPosition { get; set; }
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
    public FlowerPiece LeftBottomPiece { get; private set; } = null!;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool Placed { get; private set; }

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
            AllowRoof = AllowRoof
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

        if (!Sprites.Any(s => s is { X: 0, Y: 0 }))
        {
            Sprites.Add(new FlowerPiece { IsFake = true });
        }

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
            Name = $"{FlowerName}Sprite"
        };
    }

    public void Confirm()
    {
        Placed = true;
        Sprite.ZIndex++;
    }

    public Sprite2D ShowSprite()
    {
        Sprite.Show();
        return Sprite;
    }

    public Sprite2D HideSprite()
    {
        if (Placed) return Sprite;
        Sprite.Hide();
        return Sprite;
    }

    public void SetPosition(Transform2D transform, bool isRoof)
    {
        var plot = LeftBottomPiece.Plot;
        if (plot is null || BuildingSelector.CurrentBuilding is null) return;

        Sprite.FlipH = plot.IsLeft;

        if (Sprite.GetParent() is null)
        {
            plot.AddChild(Sprite, OS.IsDebugBuild());
        }
        else
        {
            Sprite.Reparent(plot);
        }

        Vector2 localCenter;

        if (isRoof)
        {
            localCenter = new Vector2(1f, 1f) * (Mathf.FloorToInt(Height / 2f) + 1);
        }
        else
        {
            // --- Wall Alignment Math (Original Logic) ---
            var xDirection = plot.IsLeft ? 1f : -1f;
            const float yDirection = 1f;
            var width = Width > 1 ? Width / -4f : 0;

            localCenter = new Vector2(
                0.5f + width * xDirection,
                0.5f + (Height - 1) / 2f * yDirection
            );
        }

        Sprite.Position = transform * localCenter;
    }

    public void SetPosition(Vector2 pos)
    {
        Sprite.Position = pos;
    }

    public void SetColor(Color color)
    {
        Sprite.Modulate = color;
    }
}