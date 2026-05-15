using System.Drawing;
using Godot;
using ProjectPlantris.Managers;
using ProjectPlantris.Scenes.Buildings;

namespace ProjectPlantris.Scenes;

public partial class Camera : Camera2D
{
    public static Camera Instance { get; private set; } = null!;

    [Export] public Control Menu { get; set; } 
    [Export] public Control Ui { get; set; }

    private const float CamMin = 10.0f;
    private const float CamMax = 50.0f;
    private const float SizeMin = 3.0f;
    private const float SizeMax = 18.0f;

    private readonly Vector3 _startPosition = new(278.0f, 97.0f, 95.61987f);

    private Vector3 _basePosition;
    public bool Started { get; private set; } = true;
    private float _originalSize;

    public override void _Ready()
    {
        Instance = this;

        BuildingSelector.Instance.BuildingChanged += OnBuildingChanged;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnBuildingChanged(Building building)
    {
    }

    public void Start()
    {
        Started = true;
        OnBuildingChanged(BuildingSelector.Instance.CurrentBuilding);
    }

    public void End()
    {
    }
}