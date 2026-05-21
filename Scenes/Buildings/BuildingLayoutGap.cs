using Godot;
using Godot.Collections;

namespace ProjectPlantris.Scenes.Buildings;

[Tool, GlobalClass]
public partial class BuildingLayoutGap : Resource
{
    [Export] public Array<BuildingLayoutGapNode> Gaps = [new()];
    [Export] public bool LeftGrid { get; set; }
    [Export] public bool RightGrid { get; set; }
    [Export] public bool RoofGrid { get; set; }
    [Export] public Vector2 GridOffset { get; set; }

    [ExportCategory("Generation")] [Export(PropertyHint.MultilineText)]
    private string _format = "X";

    [ExportToolButton("Generate Sprites")] private Callable GenerateSprites => Callable.From(GenerateSpritesByFormat);

    private void GenerateSpritesByFormat()
    {
        if (string.IsNullOrEmpty(_format)) return;
        //check if name contains nxn where n is number
        Gaps.Clear();
        var lines = _format.Split('\n');
        var y = lines.Length - 1;
        foreach (var line in lines)
        {
            var x = 0;
            while (x < line.Length)
            {
                var @char = line[x];
                if (@char is 'X' or 'x')
                {
                    Gaps.Add(new BuildingLayoutGapNode() { X = x + (int)GridOffset.X, Y = y + (int)GridOffset.Y });
                }

                x++;
            }

            y--;
        }
    }
}