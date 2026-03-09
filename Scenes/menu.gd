extends Control

static var last_score :int
@onready var start_button: TextureButton = %StartButton
@onready var settings_button: TextureButton = %SettingsButton
@onready var score_label:Label = %ScoreLabel
@onready var score_title_label:Label = %ScoreTitleLabel
@export var settings: Control
@export var ui: Control
@export var camera: Camera

func _ready() -> void:
	start_button.grab_focus()
	start_button.pressed.connect(_on_start_pressed)
	settings_button.pressed.connect(_on_settings_pressed)
	ScoreManager.Instance.score_updated.connect(_on_score_updated)
	if last_score > 0:
		_on_score_updated(last_score)
	
func _notification(what):
	if what == NOTIFICATION_UNPAUSED:
		start_button.grab_focus()
	
func _on_start_pressed():
	hide()
	camera.start()
	ui.show()
	ScoreManager.Instance.score = 0
	pass

func _on_settings_pressed():
	settings.show()
	get_tree().paused = true
	pass

func _on_score_updated(score:float):
	if score == 0: return;
	score_label.show()
	score_label.text = str(int(score))
	score_title_label.show()

func re_show():
	show()
	start_button.grab_focus()
	last_score = int(ScoreManager.Instance.score)
	get_tree().change_scene_to_file("res://Scenes/city.tscn")
