extends Resource

class_name FlowerPiece
@export var x: int
@export var y: int
@export var depends_on_side: bool
@export var left_side: bool
var plot: Plot

func get_position()-> Vector2:
	return Vector2(x,y)
