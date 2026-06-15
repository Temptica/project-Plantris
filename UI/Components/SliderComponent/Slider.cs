using Godot;

namespace ProjectPlantris.UI.Components.SliderComponent;

[GlobalClass]
public partial class Slider : VBoxContainer
{
	public Label Label { get; private set; }

	public HSlider HSlider { get; private set; }
	[Export] public string TextFormat { get; set; } = " 0 0 0";
	[Export] public float MinValue { get; set; }
	[Export] public float MaxValue { get; set; } = 100;
	[Export] public float Step { get; set; } = 1;
	[Export] public float Value { get; set; } = 50;
	
	[Signal] public delegate void ValueChangedEventHandler(double value);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Label = GetNode<Label>("Label");
		HSlider = GetNode<HSlider>("Slider");
		HSlider.MinValue = MinValue;
		HSlider.MaxValue = MaxValue;
		HSlider.Step = Step;
		Label.Text = Value.ToString(TextFormat);
		HSlider.Value = Value;
		HSlider.ValueChanged += value =>
		{
			Label.Text = value.ToString(TextFormat);
			EmitSignalValueChanged(value);
		};
	}
	
	public void SetValue(float value)
	{
		HSlider.Value = value;
		Label.Text = value.ToString(TextFormat);
	}
}