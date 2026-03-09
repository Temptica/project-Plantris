extends Node

class_name MovementController

@export var selectedFlower : Flower
var timer : Timer
var last_direction:Vector2 = Vector2.ZERO
const timeout_time = 0.3
static var Instance: MovementController

signal flower_placed(flower:Flower)
signal flower_select_next()
signal flower_select_prev()

func _get_current_building() -> Building: 
	return BuildingSelector.Instance.currentBuilding

func _ready() -> void:
	Instance = self
	timer = Timer.new()
	timer.timeout.connect(_on_timeout)
	add_child(timer)

func _exit_tree() -> void:
	Instance = null

func set_flower(flower: Flower):
	selectedFlower = flower
	
	match selectedFlower.flower_type:
		Flower.FlowerType.TOP:
			var buildingHeight = _get_current_building().height
			var flowerHeight = selectedFlower.max_y
			var y = buildingHeight-flowerHeight-1
			selectedFlower.grid_position.y = y
		Flower.FlowerType.BOTTOM:
			selectedFlower.grid_position.y = 0
		Flower.FlowerType.NORMAL:
			var y = _get_current_building().height/2
			if y + selectedFlower.max_y+1 > _get_current_building().height:
				y = _get_current_building().height - selectedFlower.max_y - 1
			selectedFlower.grid_position.y = y
	_draw_flower()

func _on_timeout():
	if !Input.is_anything_pressed():
		timer.stop()
		return;
	if Input.is_action_pressed("right"):
		move_right()
	elif Input.is_action_pressed("left"):
		move_left()
	if Input.is_action_pressed("up"):
		move_up()
		return
	elif Input.is_action_pressed("down"):
		move_down()
		return;

func _input(event):
	if event.is_action_pressed("right"):
		move_right()
		timer.start(timeout_time)
		return;
	if event.is_action_pressed("left"):
		move_left()
		timer.start(timeout_time)
		return;
	if event.is_action_pressed("up"):
		move_up()
		timer.start(timeout_time)
		return
	if event.is_action_pressed("down"):
		move_down()
		timer.start(timeout_time)
		return;
	if event.is_action_pressed("accept"):
		confirm()
		return
	if event.is_action_pressed("selector_right"):
		flower_select_next.emit()
		return;
	if event.is_action_pressed("selector_left"):
		flower_select_prev.emit()
		return;

func move_right():
	var currentBuilding = _get_current_building()
	var rightMostNewPosition = selectedFlower.grid_position.x + selectedFlower.sprites.map(func(s): return s.x).max() +1
	if rightMostNewPosition >= currentBuilding.width: 
		return     
	selectedFlower.grid_position.x += 1
	_draw_flower()

func move_left():
	var currentBuilding = _get_current_building()
	if selectedFlower.grid_position.x < 0:
		var rightMostNewPosition = selectedFlower.grid_position.x -selectedFlower.sprites.map(func(s): return s.x).max()
		if rightMostNewPosition <= -currentBuilding.depth: 
			return     
	else: 
		var leftMostNewPosition = selectedFlower.grid_position.x + selectedFlower.sprites.map(func(s): return s.x).min() -1
		if leftMostNewPosition < -currentBuilding.depth: 
			return     
	selectedFlower.grid_position.x -= 1
	_draw_flower()

func move_up():
	if selectedFlower.flower_type != Flower.FlowerType.NORMAL: return;
	var currentBuilding = _get_current_building()
	var upMostNewPosition = selectedFlower.grid_position.y + selectedFlower.sprites.map(func(s): return s.y).max() + 1
	if upMostNewPosition >= currentBuilding.height: 
		return     
	selectedFlower.grid_position.y += 1
	_draw_flower()

func move_down():
	if selectedFlower.flower_type != Flower.FlowerType.NORMAL: return;
	if selectedFlower.grid_position.y == 0:
		return
	selectedFlower.grid_position.y -= 1
	_draw_flower()

func confirm():
	#validate positon. place if correct
	if !_get_current_building().try_set_flower(): return
	flower_placed.emit(selectedFlower)
	
	pass

func _draw_flower():
	#remove flower from current spot and move it to new spot
	_get_current_building().position_flower(selectedFlower)
	pass
