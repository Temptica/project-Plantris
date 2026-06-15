using Godot;

namespace ProjectPlantris.Scenes;

public partial class MainMenu : Control
{
    private TextureButton _settingsButton = null!;
    private TextureButton _startButton = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _settingsButton = GetNode<TextureButton>("%SettingsButton");
        _settingsButton.Pressed += OnSettingsButtonPressed;
        _settingsButton.MouseEntered += () => _startButton.ReleaseFocus();
        
        _startButton = GetNode<TextureButton>("%StartButton");
        _startButton.Pressed += OnStartButtonPressed;
        _startButton.GrabFocus();
        _startButton.MouseEntered += () => _settingsButton.ReleaseFocus();
        
    }

    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/City.tscn");
    }

    private void OnSettingsButtonPressed()
    {
        _settingsButton.Disabled = true;
        _startButton.ReleaseFocus();
        _startButton.Disabled = true;
        _startButton.ReleaseFocus();

        var scene = GD.Load<PackedScene>("res://UI/Settings.tscn").Instantiate<UI.Settings>();
        scene.TreeExiting += () =>
        {
            _settingsButton.Disabled = false;
            _startButton.Disabled = false;
            _startButton.GrabFocus();
        };

        AddChild(scene);
    }
}