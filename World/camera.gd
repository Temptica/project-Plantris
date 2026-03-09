extends Camera3D

class_name Camera

static var Instance: Camera

@export var menu:Control
@export var ui : Control
var cam_min : float = 10
var cam_max : float = 50
var size_min: float = 3
var size_max : float = 18
var base_position: Vector3
var started: bool = false
@onready var start_pos: = Vector3(278,97,95.61987)
var original_size

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	Instance = self
	BuildingSelector.Instance.building_changed.connect(_on_building_changed)
	base_position = position
	original_size = size
	pass # Replace with function body.

func _on_building_changed(building: Building):
	if !started : return;
	var sizeDif = size_max-size_min
	
	var cam_val = (building.height-size_min)/sizeDif
	var camSize = lerpf(cam_min, cam_max, cam_val/1.0)
	var tween : Tween = create_tween().set_trans(Tween.TRANS_SINE)
	var pos = Vector3(building.camera_x_position, building.camera_y_position, global_position.z)
	tween.tween_property(self, "global_position", pos,2)
	tween.parallel().tween_property(self, "size", camSize,2)
	tween.tween_callback(func(): building.allow_building())
	
func start():
	started = true
	_on_building_changed(BuildingSelector.Instance.currentBuilding)

func end():
	var tween : Tween = create_tween().set_trans(Tween.TRANS_SINE)
	tween.tween_property(self, "global_position", start_pos,2)
	tween.parallel().tween_property(self, "size", original_size,2)
	tween.tween_callback(func(): menu.re_show(); ui.hide())

	
