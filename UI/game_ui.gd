extends Control

@onready var flowerButton1 = %Flower1
@onready var flowerButton2 = %Flower2
@onready var flowerButton3 = %Flower3
@onready var flowerGenerator = %FlowerGenerator
@onready var panel: Panel = %HighlightPanel
@onready var panelTexture: TextureRect = %HighlightPanelTexture
@onready var flowerSelectionContainer:HBoxContainer = %FlowerSelectionContainer
@onready var movementController: MovementController = MovementController.Instance
@onready var next_button: Button = %NextButton
@onready var score_label: Label = %ScoreLabel

var flower1: Flower
var flower2: Flower
var flower3: Flower

var selectedIndex: int = 1

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	set_new_flower(1)
	set_new_flower(2)
	set_new_flower(3)
	
	movementController.flower_placed.connect(_on_flower_placed)
	movementController.flower_select_prev.connect(_on_flower_select_prev)
	movementController.flower_select_next.connect(_on_flower_select_next)
	ScoreManager.Instance.score_updated.connect(_on_score_updated)
	BuildingSelector.Instance.building_changed.connect(_on_building_changed)
	movementController.set_flower(flower1)
	
	pass # Replace with function body.

func _on_flower_placed(flower):
	if flower == flower1:
		set_new_flower(1)
	elif flower == flower2:
		set_new_flower(2)
	elif flower == flower3:
		set_new_flower(3)

func _on_flower_select_prev():
	selectedIndex -=1
	if selectedIndex <1 :
		selectedIndex = 3
	
	_select_flower()

func _on_flower_select_next():
	selectedIndex +=1
	if selectedIndex > 3 :
		selectedIndex = 1
	
	_select_flower()

func _select_flower():
	if selectedIndex == 1:
		flowerButton1.hide()
		panelTexture.texture = flower1.texture
		flowerSelectionContainer.move_child(panel, 0)
		flowerSelectionContainer.move_child(flowerButton2,1)
		flowerSelectionContainer.move_child(flowerButton3,2)
		flowerButton2.show()
		flowerButton3.show()
		movementController.set_flower(flower1)
	elif selectedIndex == 2:
		flowerButton2.hide()
		panelTexture.texture = flower2.texture
		flowerSelectionContainer.move_child(flowerButton1,0)
		flowerSelectionContainer.move_child(panel, 1)
		flowerSelectionContainer.move_child(flowerButton3,2)
		flowerButton1.show()
		flowerButton3.show()
		movementController.set_flower(flower2)
	elif selectedIndex == 3:
		flowerButton3.hide()
		panelTexture.texture = flower3.texture
		flowerSelectionContainer.move_child(flowerButton1,0)
		flowerSelectionContainer.move_child(flowerButton2,1)
		flowerSelectionContainer.move_child(panel, 2)
		flowerButton1.show()
		flowerButton2.show()
		movementController.set_flower(flower3)

func set_new_flower(number : int):
	if number <1 : number = 1
	elif number > 3: number = 3
	
	if number == 1:
		flower1 = flowerGenerator.get_random_flower()
		flowerButton1.texture_normal = flower1.texture
	elif number == 2:
		flower2 = flowerGenerator.get_random_flower()
		flowerButton2.texture_normal = flower2.texture
	elif number == 3:
		flower3 = flowerGenerator.get_random_flower()
		flowerButton3.texture_normal = flower3.texture
	
	_select_flower()

func _on_flower_button_pressed(idx: int) -> void:
	if idx == selectedIndex: return
	
	selectedIndex = idx
	_select_flower()

func _on_next_building_pressed() -> void:
	BuildingSelector.Instance.select_next_building()
	next_button.release_focus()
	pass # Replace with function body.

func _on_score_updated(score:int):
	score_label.text = str(score)

func _on_building_changed(_building:Building):
	set_new_flower(selectedIndex)
