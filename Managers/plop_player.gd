extends AudioStreamPlayer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	MovementController.Instance.flower_placed.connect(_on_flower_placed)
	pass # Replace with function body.

func _on_flower_placed(_f:Flower):
	pitch_scale = randf_range(0.9,1.1)
	play()
