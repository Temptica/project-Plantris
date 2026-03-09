extends Control

@onready var master_label = %MasterLabel
@onready var master_slider = %MasterSlider
@onready var sfx_label = %SFXLabel
@onready var sfx_slider = %SFXSLider
@onready var music_laber = %MusicLabel
@onready var music_slider = %MusicSlider

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_set_master_label(AudioServer.get_bus_volume_linear(0)*100)
	_set_sfx_label(AudioServer.get_bus_volume_linear(1)*100)
	_set_music_label(AudioServer.get_bus_volume_linear(2)*100)
	
	pass # Replace with function body.

func _on_return_button_pressed() -> void:
	get_tree().paused =  false
	hide()

func _on_master_slider_value_changed(value: float) -> void:
	AudioServer.set_bus_volume_linear(0,value/100.0)
	_set_master_label(value)

func _on_sfx_slider_value_changed(value : float):
	AudioServer.set_bus_volume_linear(1,value/100.0)
	_set_sfx_label(value)

func _on_music_slider_value_changed(value : float):
	AudioServer.set_bus_volume_linear(2,value/100.0)
	_set_music_label(value)

func _set_master_label(value:float):
	var lbl = str(int(round(value))).pad_zeros(3)
	var result = ""
	for i in range(lbl.length()):
		result += lbl[i] + " "
		
	master_label.text = result
	master_slider.value = value

func _set_sfx_label(value:float):
	var lbl = str(int(round(value))).pad_zeros(3)
	var result = ""
	for i in range(lbl.length()):
		result += lbl[i] + " "
		
	sfx_label.text = result
	sfx_slider.value = value

func _set_music_label(value:float):
	var lbl = str(int(round(value))).pad_zeros(3)
	var result = ""
	for i in range(lbl.length()):
		result += lbl[i] + " "
		
	music_laber.text = result
	music_slider.value = value
