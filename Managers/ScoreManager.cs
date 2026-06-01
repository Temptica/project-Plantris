using Godot;
using ProjectPlantris.Scenes.Flowers;

namespace ProjectPlantris.Managers;

public partial class ScoreManager : Node
{
    public static ScoreManager Instance { get; private set; }

    public int Score
    {
        get;
        private set
        {
            field = value;
            EmitSignalScoreUpdated(field);
        }
    }

    public int Target { get; set; }

    [Signal]
    public delegate void ScoreUpdatedEventHandler(int score);

    [Signal]
    public delegate void TargetMetEventHandler();

    public override void _Ready()
    {
        Instance = this;
        MovementController.Instance.FlowerPlaced += OnFlowerPlaced;
        BuildingSelector.Instance.BuildingFilled += OnBuildingFilled;
    }

    private void OnFlowerPlaced(Flower flower)
    {
        var points = flower.Sprites.Count * 10;
        points += 2 ^ flower.Sprites.Count;
        AddPoints(points);
    }

    private void OnBuildingFilled()
    {
        AddPoints(200);
    }

    public bool MetTarget => Score >= Target;

    public void Reset()
    {
        Score = 0;
    }

    public void AddPoints(int points)
    {
        Score += points;
    }


}