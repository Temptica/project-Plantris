extends Resource


class_name Flower
const WIND_SHADER = preload("res://Shaders/wind_wave.gdshader")

@export var flower_name: String
@export var sprites: Array[FlowerPiece]
@export var texture: Texture2D
@export var flower_type: FlowerType = FlowerType.NORMAL
var sprite: Sprite3D

enum FlowerType {NORMAL,BOTTOM,TOP}

var grid_position : Vector2
var min_x: int
var max_x: int
var min_y: int
var max_y: int
var _width : int = 0
var _height : int = 0
var left_bottom_piece: FlowerPiece

func copy() -> Flower: 
	if _width == 0:
		_initialize()
	
	var cpy = Flower.new()
	cpy.flower_name = flower_name
	cpy.sprites = sprites
	cpy.texture = texture
	cpy.sprite = Sprite3D.new()
	cpy._set_sprite_values()
	cpy.min_x = min_x
	cpy.max_x = max_x
	cpy.min_y = min_y
	cpy.max_y = max_y
	cpy._width = _width
	cpy._height = _height
	cpy.left_bottom_piece = left_bottom_piece
	cpy.flower_type = flower_type
	return cpy

func _initialize()-> void:
	#(size-1)/2
	var x_pos = sprites.map(func(s): return s.x)
	var y_pos = sprites.map(func(s): return s.y)
	min_x = x_pos.min()
	max_x = x_pos.max()
	min_y = y_pos.min()
	max_y = y_pos.max()
	_width = max_x-min_x+1 
	_height = max_y-min_y
	_set_sprite_values()
	left_bottom_piece = sprites.filter(func(s): return s.x == 0 && s.y == 0)[0]

func _set_sprite_values():
	sprite = Sprite3D.new()
	sprite.billboard = BaseMaterial3D.BILLBOARD_ENABLED
	sprite.transparent = true
	sprite.double_sided = false
	sprite.shaded = true
	sprite.render_priority = 1
	sprite.texture = texture

func confirm() -> void:
	sprite.render_priority = 0

func show_sprite() -> Sprite3D:
	sprite.show()
	return sprite

func hide_sprite() -> Sprite3D:
	sprite.hide()
	return sprite

func set_position():
	var plot :Plot = left_bottom_piece.plot
	sprite.flip_h = plot.is_left
	
	if sprite.get_parent() == null: plot.add_child(sprite)
	else: sprite.reparent(plot)
	var offset: Vector3 = Vector3(0,_height,0)
	
	if _width > 1:
		if plot.is_left: offset += Vector3(0,0,((_width-1)* BuildingSelector.Instance.currentBuilding.unit/2.0))
		else: offset += Vector3(-((_width-1)* BuildingSelector.Instance.currentBuilding.unit/2.0),0,0)
	
	sprite.position = offset
	
func set_color(color:Color):
	sprite.modulate = color
