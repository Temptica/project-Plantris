extends Camera3D

var cam_min : float = 10
var cam_max : float = 50
var size_min: float = 3
var size_max : float = 18
var base_position: Vector3
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	BuildingSelector.Instance.building_changed.connect(_on_building_changed)
	base_position = position
	pass # Replace with function body.

func _on_building_changed(building: Building):
	var sizeDif = size_max-size_min
	
	var cam_val = (building.height-size_min)/sizeDif
	var camSize = lerpf(cam_min, cam_max, cam_val/1.0)
	var tween : Tween = create_tween().set_trans(Tween.TRANS_SINE)
	var pos = Vector3(building.camera_x_position, building.camera_y_position, global_position.z)
	tween.tween_property(self, "global_position", pos,2)
	tween.parallel().tween_property(self, "size", camSize,2)
	tween.tween_callback(func(): building.allow_building())
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
