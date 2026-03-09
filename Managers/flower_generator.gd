extends Node

class_name FlowerGenerator
static var Instance: FlowerGenerator

@export var flowers : Array[Flower] = []

func _enter_tree() -> void:
	Instance = self
	for f in flowers:
		f._initialize()

func _exit_tree() -> void:
	Instance = null

func get_random_flower() -> Flower:
	var currentBuilding = BuildingSelector.Instance.currentBuilding
	var max_width = min(currentBuilding.depth,currentBuilding.width) - 1
	var max_height = currentBuilding.height - 1
	var flower = flowers.filter(func(f:Flower): return _flower_filter(f, max_width,max_height)).pick_random()
	return flower.copy()

func _flower_filter(f:Flower, max_width: int, max_height: int)-> bool:
	if f._width > max_width || f._height > max_height: return false
	var currentBuilding = BuildingSelector.Instance.currentBuilding
	if f.flower_type == Flower.FlowerType.BOTTOM && !currentBuilding.has_bottom_space(): return false
	if f.flower_type == Flower.FlowerType.TOP && !currentBuilding.has_top_space(): return false
	return true
	
