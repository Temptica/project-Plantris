extends Control

@onready var start_button: TextureButton = %StartButton
@export var settings_button: TextureButton

func  _ready() -> void:
	start_button.grab_focus()
	
