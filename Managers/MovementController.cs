using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Godot;
using ProjectPlantris.Scenes;
using ProjectPlantris.Scenes.Buildings;
using ProjectPlantris.Scenes.Flowers;

namespace ProjectPlantris.Managers;

public partial class MovementController : Node
{
    [Export] private Flower? _selectedFlower;
    [Export] private Control _settings = null!;

    private Timer _timer = null!;
    private Vector2 _lastDirection = Vector2.Zero;

    private const float TimeoutTime = 0.2f;

    public static MovementController Instance { get; private set; } = null!;

    [Signal]
    public delegate void FlowerPlacedEventHandler(Flower flower);

    [Signal]
    public delegate void FlowerSelectedNextEventHandler();

    [Signal]
    public delegate void FlowerSelectedPrevEventHandler();

    [Signal]
    public delegate void FlowerMovedEventHandler();

    private static Building? CurrentBuilding => BuildingSelector.CurrentBuilding;

    public override void _Ready()
    {
        Instance = this;

        _timer = new Timer();
        _timer.Timeout += OnTimeout;
        AddChild(_timer);
    }

    public override void _ExitTree()
    {
        Instance = null!;
    }

    public void SetFlower(Flower flower)
    {
        _selectedFlower = flower;

        if (CurrentBuilding is null) return;

        switch (_selectedFlower.Type)
        {
            case Flower.FlowerType.Top:
            {
                var buildingHeight = CurrentBuilding.Height;
                var flowerHeight = _selectedFlower.MaxY;
                var y = buildingHeight - flowerHeight - 1;
                _selectedFlower.GridPosition = new Vector2(_selectedFlower.GridPosition.X, y);
                break;
            }

            case Flower.FlowerType.Bottom:
            {
                _selectedFlower.GridPosition = new Vector2(_selectedFlower.GridPosition.X, 0);
                break;
            }

            case Flower.FlowerType.Normal:
            {
                var y = CurrentBuilding.Height / 2.0f;

                if (y + _selectedFlower.MaxY + 1 > CurrentBuilding.Height)
                {
                    y = CurrentBuilding.Height - _selectedFlower.MaxY - 1;
                }

                _selectedFlower.GridPosition = new Vector2(_selectedFlower.GridPosition.X, y);
                break;
            }
        }

        DrawFlower();
    }

    private void OnTimeout()
    {
        if (!Input.IsAnythingPressed())
        {
            _timer.Stop();
            return;
        }

        if (Input.IsActionPressed("right"))
        {
            MoveRight();
        }
        else if (Input.IsActionPressed("left"))
        {
            MoveLeft();
        }

        if (Input.IsActionPressed("up"))
        {
            MoveUp();
        }
        else if (Input.IsActionPressed("down"))
        {
            MoveDown();
        }

        DrawFlower();
    }

    public override void _Input(InputEvent @event)
    {
        if (!Camera.Instance.Started)
        {
            return;
        }

        if (@event is not InputEventKey) return;

        if (@event.IsActionPressed("ui_cancel"))
        {
            _settings.Show();
            GetTree().Paused = true;
            return;
        }

        if (@event.IsActionPressed("right"))
        {
            MoveRight();
            _timer.Start(TimeoutTime);
        }
        else if (@event.IsActionPressed("left"))
        {
            MoveLeft();
            _timer.Start(TimeoutTime);
        }

        if (@event.IsActionPressed("up"))
        {
            MoveUp();
            _timer.Start(TimeoutTime);
        }
        else if (@event.IsActionPressed("down"))
        {
            MoveDown();
            _timer.Start(TimeoutTime);
        }

        if (@event.IsActionPressed("accept"))
        {
            Confirm();
            return;
        }

        if (@event.IsActionPressed("selector_right"))
        {
            EmitSignalFlowerSelectedNext();
            return;
        }

        if (@event.IsActionPressed("selector_left"))
        {
            EmitSignalFlowerSelectedPrev();
        }

        DrawFlower();
    }

    private void MoveRight()
    {
        if (_selectedFlower == null || CurrentBuilding is null) return;

        if (_selectedFlower.GridPosition.X < 0 && !CurrentBuilding.HasRightGrid) return;

        var rightMostNewPosition =
            _selectedFlower.GridPosition.X +
            _selectedFlower.Sprites.Max(sprite => sprite.X) +
            1;

        if (rightMostNewPosition >= CurrentBuilding.Width)
        {
            return;
        }

        _selectedFlower.GridPosition += Vector2.Right;
    }

    private void MoveLeft()
    {
        if (_selectedFlower == null || CurrentBuilding is null) return;

        if (_selectedFlower.GridPosition.Y >= CurrentBuilding.Height && _selectedFlower.GridPosition.X == 0)
        {
            if (!CurrentBuilding.HasLeftGrid) return;

            var x = (_selectedFlower.GridPosition.Y - CurrentBuilding.Height) * -1 - 1;
            if (_selectedFlower.Type == Flower.FlowerType.Bottom)
            {
                _selectedFlower.GridPosition = new Vector2(x, 0);
                return;
            }

            var y = CurrentBuilding.Height - _selectedFlower.MaxY - 1;
            _selectedFlower.GridPosition = new Vector2(x, y);
            return;
        }

        if (_selectedFlower.GridPosition.X == 0)
        {
            if (!CurrentBuilding.HasLeftGrid) return;

            var rightMostNewPosition =
                _selectedFlower.GridPosition.X -
                _selectedFlower.Sprites.MaxBy(sprite => sprite.X)!.X;

            if (rightMostNewPosition <= -CurrentBuilding.Depth)
            {
                return;
            }
        }
        else
        {
            var leftMostNewPosition =
                _selectedFlower.GridPosition.X +
                _selectedFlower.Sprites.Min(sprite => sprite.X) -
                1;

            if (leftMostNewPosition < -CurrentBuilding.Depth)
            {
                return;
            }
        }

        _selectedFlower.GridPosition += Vector2.Left;
    }

    private void MoveUp()
    {
        if (_selectedFlower == null || CurrentBuilding is null) return;

        if (_selectedFlower.Type == Flower.FlowerType.Bottom &&
            (!_selectedFlower.AllowRoof || !CurrentBuilding.HasRoofGrid))
        {
            return;
        }

        var buildingHeight = CurrentBuilding.Height;

        if (_selectedFlower.Type == Flower.FlowerType.Bottom && _selectedFlower.GridPosition.Y < CurrentBuilding.Height)
        {
            var xPos = _selectedFlower.GridPosition.X;
            var yPos = CurrentBuilding.Height;
            if (xPos < 0)
            {
                yPos += Mathf.FloorToInt(xPos * -1) - 1;
                xPos = 0;
            }

            _selectedFlower.GridPosition = new Vector2(xPos, yPos);
            return;
        }

        var aboveNormalHeight = _selectedFlower.GridPosition.Y > buildingHeight;

        var upMostNewPosition =
            _selectedFlower.GridPosition.Y +
            _selectedFlower.Sprites.Where(sprite => !aboveNormalHeight || !sprite.IsEmptyForRoof)
                .Max(sprite => sprite.Y) +
            1;

        if (_selectedFlower.AllowRoof && CurrentBuilding.HasRoofGrid) buildingHeight += CurrentBuilding.Depth;


        if (upMostNewPosition >= buildingHeight)
        {
            return;
        }

        _selectedFlower.GridPosition -= Vector2.Up;
    }

    private void MoveDown()
    {
        if (_selectedFlower == null || CurrentBuilding is null) return;

        if (_selectedFlower.Type == Flower.FlowerType.Top)
        {
            return;
        }

        if (_selectedFlower.GridPosition.Y == 0)
        {
            return;
        }

        if (_selectedFlower.Type == Flower.FlowerType.Bottom &&
            _selectedFlower.GridPosition.Y == CurrentBuilding.Height)
        {
            _selectedFlower.GridPosition = new Vector2(_selectedFlower.GridPosition.X, 1);
        }

        _selectedFlower.GridPosition -= Vector2.Down;
    }

    private void Confirm()
    {
        if (_selectedFlower == null || CurrentBuilding is null) return;

        // Validate position. Place if correct.
        if (!CurrentBuilding.TrySetFlower())
        {
            return;
        }

        EmitSignalFlowerPlaced(_selectedFlower);
    }

    private void DrawFlower()
    {
        if (_selectedFlower == null || CurrentBuilding is null) return;

        // Remove flower from current spot and move it to new spot.
        CurrentBuilding.PositionFlower(_selectedFlower);
        EmitSignalFlowerMoved();
    }
}