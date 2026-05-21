using Godot;
using System;
using ProjectPlantris.Managers;
using ProjectPlantris.Scenes.Buildings;
using ProjectPlantris.Scenes.Flowers;

public partial class BuildingPercentageProgressBar : ProgressBar
{
	private Building _currentBuilding;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_currentBuilding = BuildingSelector.CurrentBuilding!;
		BuildingSelector.Instance.BuildingChanged += OnBuildingChanged;
		MovementController.Instance.FlowerPlaced += OnFlowerPlaced;
		MaxValue = _currentBuilding.PlotCount;
	}

	private void OnBuildingChanged(Building building)
	{
		_currentBuilding = building;
		MaxValue = _currentBuilding.PlotCount;
		Value = 0;
	}
	private void OnFlowerPlaced(Flower flower)
	{
		var currentAmount = _currentBuilding.GetFreeSpotsCount();
		Value = MaxValue - currentAmount;
	}

	public override void _ExitTree()
	{
		BuildingSelector.Instance.BuildingChanged -= OnBuildingChanged;
		MovementController.Instance.FlowerPlaced -= OnFlowerPlaced;
	}
}
