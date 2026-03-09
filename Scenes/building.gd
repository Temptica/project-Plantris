extends Node3D

class_name Building
@export var depth: int
@export var width: int
@export var height: int
@export var unit: float = 2.2
@export var texture: Texture2D
@export var camera_x_position: float
@export var camera_y_position: float
@export var draw_pos: int = -1
@export var background: bool = false
@onready var mesh: MeshInstance3D = %Mesh
@onready var sprite: Sprite3D = %FrontSprite
@onready var leftGrid: BuildingGrid = %LeftFlowers
@onready var rightGrid: BuildingGrid = %RightFlowers
var plotSize: Vector2
var current_flower:Flower
var can_build: bool

signal full

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	
	var boxMesh: BoxMesh = mesh.mesh;
	boxMesh.size = Vector3(width*unit, height*unit, depth*unit)
	#mesh.position = Vector3(0, boxMesh.size.y/2.0, 0)
	plotSize = Vector2(unit,unit)
	
	if texture != null:
		sprite.texture = texture
		sprite.render_priority = draw_pos
	
	leftGrid.position = Vector3(boxMesh.size.x/2.0, -boxMesh.size.y/2.0, boxMesh.size.z/2.0)
	rightGrid.position = Vector3(boxMesh.size.x/2.0, -boxMesh.size.y/2.0, -boxMesh.size.z/2.0)
	leftGrid.set_values(true,self)
	rightGrid.set_values(false,self)
	if background: return;
		
	MovementController.Instance.flower_placed.connect(func(_a): current_flower = null)
	
func position_flower(flower : Flower):
	if !can_build: return
	
	for plot in leftGrid.grid:
		plot.remove_current()
	
	for plot in rightGrid.grid:
		plot.remove_current()
	
	for piece in flower.sprites:
		var isFlipped = flower.grid_position.x < 0
		var piecePos: Vector2 = piece.get_position()
		if isFlipped: piecePos.x *= -1
		
		var pos = flower.grid_position + piecePos
		
		var grid: BuildingGrid = leftGrid if pos.x < 0 else rightGrid
		
		var slot = grid.find_plot(pos.x,pos.y)
		slot.set_current_piece(piece)
	
	if current_flower != flower:
		if current_flower != null:
			var hideSprite = current_flower.hide_sprite()
			if hideSprite.get_parent() == self: remove_child(hideSprite)
		var showSprite = flower.show_sprite()
		if showSprite.get_parent() == null: self.add_child(showSprite)
		else: showSprite.reparent(self)
		current_flower = flower
	flower.set_position()
	flower.set_color(Color.WHITE if _can_place() else Color.DARK_RED)

func _can_place() -> bool:
	for flowSprite in current_flower.sprites:
		if !flowSprite.plot.can_set(): return false
	return true

func try_set_flower() -> bool:
	if !_can_place(): return false
	
	for flowSprite in current_flower.sprites:
		flowSprite.plot.set_piece()
	current_flower.sprite.render_priority = 0
	for plot in leftGrid.grid:
		if plot.is_free() : return true
	
	for plot in rightGrid.grid:
		if plot.is_free() : return true
	
	full.emit()
	
	return true

func remove_current_flower() -> void:
	if current_flower == null: return
	for plot in leftGrid.grid:
		plot.remove_current()
	
	for plot in rightGrid.grid:
		plot.remove_current()
	current_flower.hide_sprite()

func allow_building():
	can_build = true;
	current_flower =MovementController.Instance.selectedFlower
	position_flower(current_flower)
