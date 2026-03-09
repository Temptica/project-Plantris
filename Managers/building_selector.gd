extends Node3D

class_name BuildingSelector

static var Instance : BuildingSelector

@export var buildings: Array[Building]

var currentBuilding: Building
var currentCount = 0

signal building_changed(building:Building)
signal building_filled

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	select_next_building()

func select_next_building():
	currentCount += 1
	if currentCount  > buildings.size(): 
		Camera.Instance.end()
		return
	
	if currentBuilding != null: 
		currentBuilding.full.disconnect(_on_filled)
		currentBuilding.remove_current_flower()
	
	currentBuilding = buildings[currentCount-1]
	currentBuilding.full.connect(_on_filled)
	building_changed.emit(currentBuilding)

func _on_filled():
	building_filled.emit()

func _on_completed():
	select_next_building()

func _enter_tree() -> void:
	Instance = self

func _exit_tree() -> void:
	Instance = null
	
func start():
	building_changed.emit(currentBuilding)
	
