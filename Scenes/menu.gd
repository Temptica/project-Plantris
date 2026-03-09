extends Control

@onready var start_button: TextureButton = %StartButton
@onready var settings_button: TextureButton = %SettingsButton

func  _ready() -> void:
	start_button.grab_focus()
	
