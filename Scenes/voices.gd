extends AudioStreamPlayer

@export var voice_lines : Array[AudioStream] = []
var idx : int
var filled: bool

func _ready() -> void:
	BuildingSelector.Instance.building_changed.connect(_on_building_placed)
	BuildingSelector.Instance.building_filled.connect(_on_filled)
	idx = 0

func _on_building_placed(_b :Building):
	idx += 1
	var size = voice_lines.size()
	if idx >= voice_lines.size() - 1 : return
	stream = voice_lines[idx]
	play()

func _on_filled():
	stream = voice_lines[-1]
	play()
