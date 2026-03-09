extends Node3D

class_name Plot
var x : int
var y : int
var flower_piece : FlowerPiece
var current_flower_piece : FlowerPiece
var is_left

func is_available() -> bool :  
	return flower_piece == null
	
func set_piece() :
	if current_flower_piece == null: return;
	
	if !current_flower_piece.fake: flower_piece = current_flower_piece

func set_current_piece(piece: FlowerPiece) -> bool :	
	current_flower_piece = piece 
	current_flower_piece.plot = self
	return true

func remove_current():
	if current_flower_piece == null: return
	if current_flower_piece.plot == self:
		current_flower_piece.plot = null
	current_flower_piece = null

func can_set():
	return (current_flower_piece == null || current_flower_piece.fake) || flower_piece == null

func is_free():
	return flower_piece == null

func _init(posX:int, posY:int, worldPos: Vector3, isLeft:bool):
	x = posX
	y = posY
	position = worldPos
	is_left = isLeft
