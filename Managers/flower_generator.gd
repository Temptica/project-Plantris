extends Node

class_name FlowerGenerator
static var Instance: FlowerGenerator

@export var flowers : Array[Flower] = []

func _enter_tree() -> void:
	Instance = self

func _exit_tree() -> void:
	Instance = null

func get_random_flower() -> Flower:
	var flower = flowers.pick_random()
	return flower.copy()
