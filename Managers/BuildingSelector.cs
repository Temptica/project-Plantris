using Godot;
using Godot.Collections;
using ProjectPlantris.Scenes;
using ProjectPlantris.Scenes.Buildings;

namespace ProjectPlantris.Managers;

public partial class BuildingSelector : Node2D
{
    [Export] public Array<Building> Buildings { get; set; } = [];

    public static BuildingSelector Instance { get; private set; } = null!;
    public static Building? CurrentBuilding { get; private set; }

    private int _currentCount;

    [Signal] public delegate void BuildingChangedEventHandler(Building building);
    [Signal] public delegate void BuildingFilledEventHandler();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        Instance = null!;
    }

    public override void _Ready()
    {
        SelectNextBuilding();
    }

    public void SelectNextBuilding()
    {
        _currentCount++;

        if (_currentCount > Buildings.Count)
        {
            Camera.Instance.End();
            return;
        }

        if (CurrentBuilding is not null)
        {
            CurrentBuilding.Full -= OnFilled;
            CurrentBuilding.Disable();
        }

        CurrentBuilding = Buildings[_currentCount - 1];
        CurrentBuilding.Full += OnFilled;
        CurrentBuilding.Enable();
        EmitSignalBuildingChanged(CurrentBuilding);
    }

    private void OnFilled()
    {
        EmitSignalBuildingFilled();
    }

    private void OnCompleted()
    {
        SelectNextBuilding();
    }

    public void Start()
    {
        EmitSignalBuildingChanged(CurrentBuilding);
    }
}