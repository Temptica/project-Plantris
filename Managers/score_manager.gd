extends Node

class_name ScoreManager
static var Instance

var score:int = 0
var target: int = 0

signal score_updated(score:int)
signal target_met

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	Instance = self
	MovementController.Instance.flower_placed.connect(_on_flower_placed)
	BuildingSelector.Instance.building_filled.connect(_on_building_filled)
	pass # Replace with function body.

func _on_flower_placed(flower:Flower):
	var points = flower.sprites.size() * 10
	points += 2**flower.sprites.size()
	_add_points(points)

func _on_building_filled():
	_add_points(200)

func met_target() -> bool:
	return score > target

func _add_points(value:int):
	score += value
	score_updated.emit(score)
	if met_target():
		target_met.emit()
