using Godot;
using Godot.Collections;
using ProjectPlantris.Scenes.Flowers;

namespace ProjectPlantris.Managers;

public partial class FlowerGenerator : Node
{
    public static FlowerGenerator Instance { get; private set; } = null!;

    [Export] public Array<Flower> Flowers { get; set; } = [];

    public override void _EnterTree()
    {
        Instance = this;

        foreach (var flower in Flowers)
        {
            flower.Initialize();
        }
    }

    public override void _ExitTree()
    {
        Instance = null!;
    }

    public Flower? GetRandomFlower()
    {
        var currentBuilding = BuildingSelector.CurrentBuilding;
        if (currentBuilding == null)
        {
            return null;
        }

        var maxWidth = Mathf.Min(currentBuilding.Depth, currentBuilding.Width) - 1;
        var maxHeight = currentBuilding.Height - 1;

        var validFlowers = new Array<Flower>();

        foreach (var flower in Flowers)
        {
            if (FlowerFilter(flower, maxWidth, maxHeight))
            {
                validFlowers.Add(flower);
            }
        }

        return validFlowers.PickRandom().Copy();
    }

    private static bool FlowerFilter(Flower flower, int maxWidth, int maxHeight)
    {
        if (flower.Width > maxWidth || flower.Height > maxHeight)
        {
            return false;
        }

        var currentBuilding = BuildingSelector.CurrentBuilding;

        return (flower.Type != Flower.FlowerType.Bottom || !(!currentBuilding?.HasBottomSpace() ?? false)) &&
               (flower.Type != Flower.FlowerType.Top || !(!currentBuilding?.HasTopSpace() ?? false));
    }
}