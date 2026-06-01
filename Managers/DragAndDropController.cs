using Godot;
using Godot.Collections;
using ProjectPlantris.Scenes.Buildings;
using ProjectPlantris.Scenes.Flowers;

namespace ProjectPlantris.Managers;

public partial class DragAndDropController : Node2D
{
    private Flower? _selectedFlower = null;
    private bool _isDragging = false;

    public override void _Process(double delta)
    {
        // If we are dragging an item, make it stick to the mouse cursor
        if (_isDragging && _selectedFlower != null)
        {
            _selectedFlower.SetPosition(GetGlobalMousePosition());
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Detect when the player releases the left mouse button
        if (_isDragging && @event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false })
        {
            DropItem();
        }
    }

    // Call this method from your UI Button
    public void StartDraggingItem(Flower node)
    {
        if (_isDragging) return;

        // Instantiate the item and add it to the scene
        _selectedFlower = node;
        
        _isDragging = true;
    }

    private void DropItem()
    {
        _isDragging = false;

        // Query the physics space to see what is under the mouse right now
        var targetZone = GetDropZoneAtMouse();
        
        BuildingSelector.CurrentBuilding!.PositionFlower(_selectedFlower, targetZone);
        
        
    }

    private Plot? GetDropZoneAtMouse()
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        
        // Configure a point query at the current mouse position
        var query = new PhysicsPointQueryParameters2D();
        query.Position = GetGlobalMousePosition();
        query.CollideWithAreas = true;  // Crucial for Area2Ds
        query.CollideWithBodies = false; // Ignore standard rigid/static bodies

        Array<Dictionary> results = spaceState.IntersectPoint(query);

        // Iterate through physics objects under the mouse
        foreach (var result in results)
        {
            var collider = result["collider"];
            if (collider.AsGodotObject() is Area2D zone && zone.GetParent() is Plot {IsEnabled: true} plot )
            {
                return plot; // Found a valid drop zone!
            }
        }

        return null;
    }
}