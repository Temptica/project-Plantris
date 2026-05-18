using Godot;
using ProjectPlantris.Managers;
using ProjectPlantris.Scenes.Buildings;
using ProjectPlantris.Scenes.Flowers;

namespace ProjectPlantris.UI;

public partial class GameUi : Control
{
    private TextureButton _flowerButton1 = null!;
    private TextureButton _flowerButton2 = null!;
    private TextureButton _flowerButton3 = null!;
    private static FlowerGenerator FlowerGenerator => FlowerGenerator.Instance;
    private Panel _panel = null!;
    private TextureRect _panelTexture = null!;
    private HBoxContainer _flowerSelectionContainer = null!;
    private MovementController _movementController = null!;
    private Button _nextButton = null!;
    private Label _scoreLabel = null!;

    private Flower? _flower1;
    private Flower? _flower2;
    private Flower? _flower3;

    private int _selectedIndex = 1;

    public override void _Ready()
    {
        _flowerButton1 = GetNode<TextureButton>("%Flower1");
        _flowerButton2 = GetNode<TextureButton>("%Flower2");
        _flowerButton3 = GetNode<TextureButton>("%Flower3");
        _panel = GetNode<Panel>("%HighlightPanel");
        _panelTexture = GetNode<TextureRect>("%HighlightPanelTexture");
        _flowerSelectionContainer = GetNode<HBoxContainer>("%FlowerSelectionContainer");
        _movementController = MovementController.Instance;
        _nextButton = GetNode<Button>("%NextButton");
        _scoreLabel = GetNode<Label>("%ScoreLabel");

        SetNewFlower(1);
        SetNewFlower(2);
        SetNewFlower(3);

        _movementController.FlowerPlaced += OnFlowerPlaced;
        _movementController.FlowerSelectedPrev += OnFlowerSelectPrev;
        _movementController.FlowerSelectedNext += OnFlowerSelectNext;

        // ScoreManager.Instance.ScoreUpdated += OnScoreUpdated;
        BuildingSelector.Instance.BuildingChanged += OnBuildingChanged;

        if (_flower1 is not null)
        {
            _movementController.SetFlower(_flower1);
        }
    }

    public override void _ExitTree()
    {
        _movementController.FlowerPlaced -= OnFlowerPlaced;
        _movementController.FlowerSelectedPrev -= OnFlowerSelectPrev;
        _movementController.FlowerSelectedNext -= OnFlowerSelectNext;

        // if (ScoreManager.Instance is not null)
        // {
        //     ScoreManager.Instance.ScoreUpdated -= OnScoreUpdated;
        // }

        BuildingSelector.Instance.BuildingChanged -= OnBuildingChanged;
    }

    private void OnFlowerPlaced(Flower flower)
    {
        if (flower == _flower1)
        {
            SetNewFlower(1);
        }
        else if (flower == _flower2)
        {
            SetNewFlower(2);
        }
        else if (flower == _flower3)
        {
            SetNewFlower(3);
        }
    }

    private void OnFlowerSelectPrev()
    {
        _selectedIndex--;

        if (_selectedIndex < 1)
        {
            _selectedIndex = 3;
        }

        SelectFlower();
    }

    private void OnFlowerSelectNext()
    {
        _selectedIndex++;

        if (_selectedIndex > 3)
        {
            _selectedIndex = 1;
        }

        SelectFlower();
    }

    private void SelectFlower()
    {
        switch (_selectedIndex)
        {
            case 1:
                _flowerButton1.Hide();
                _panelTexture.Texture = _flower1!.Texture;
                _flowerSelectionContainer.MoveChild(_panel, 0);
                _flowerSelectionContainer.MoveChild(_flowerButton2, 1);
                _flowerSelectionContainer.MoveChild(_flowerButton3, 2);
                _flowerButton2.Show();
                _flowerButton3.Show();
                _movementController.SetFlower(_flower1);
                break;
            case 2:
                _flowerButton2.Hide();
                _panelTexture.Texture = _flower2!.Texture;
                _flowerSelectionContainer.MoveChild(_flowerButton1, 0);
                _flowerSelectionContainer.MoveChild(_panel, 1);
                _flowerSelectionContainer.MoveChild(_flowerButton3, 2);
                _flowerButton1.Show();
                _flowerButton3.Show();
                _movementController.SetFlower(_flower2);
                break;
            case 3:
                _flowerButton3.Hide();
                _panelTexture.Texture = _flower3!.Texture;
                _flowerSelectionContainer.MoveChild(_flowerButton1, 0);
                _flowerSelectionContainer.MoveChild(_flowerButton2, 1);
                _flowerSelectionContainer.MoveChild(_panel, 2);
                _flowerButton1.Show();
                _flowerButton2.Show();
                _movementController.SetFlower(_flower3);
                break;
        }
    }

    private void SetNewFlower(int number)
    {
        if (number < 1)
        {
            number = 1;
        }

        switch (number)
        {
            case 1:
            {
                _flower1 = FlowerGenerator.GetRandomFlower();

                if (_flower1 is not null)
                {
                    _flowerButton1.TextureNormal = _flower1.Texture;
                }

                break;
            }
            case 2:
            {
                _flower2 = FlowerGenerator.GetRandomFlower();

                if (_flower2 is not null)
                {
                    _flowerButton2.TextureNormal = _flower2.Texture;
                }

                break;
            }
            default:
            {
                _flower3 = FlowerGenerator.GetRandomFlower();

                if (_flower3 is not null)
                {
                    _flowerButton3.TextureNormal = _flower3.Texture;
                }

                break;
            }
        }

        SelectFlower();
    }

    private void OnFlowerButtonPressed(int index)
    {
        if (index == _selectedIndex)
        {
            return;
        }

        _selectedIndex = index;
        SelectFlower();
    }

    private void OnNextBuildingPressed()
    {
        BuildingSelector.Instance.SelectNextBuilding();
        _nextButton.ReleaseFocus();
    }

    private void OnScoreUpdated(int score)
    {
        _scoreLabel.Text = score.ToString();
    }

    private void OnBuildingChanged(Building building)
    {
        SetNewFlower(_selectedIndex);
    }
}
