extends Node3D

class_name BuildingGrid
var building: Building
var grid : Array[Plot] = []
var is_left: bool
var left_bottom_world_position: Vector2
var width: int

func set_values(left_side:bool, parent:Building):
	is_left = left_side
	building = parent
	width = parent.depth if left_side else parent.width
	for x in width:
		for y in parent.height:
			building = parent
			var plotWidth = parent.plotSize.x*(-x)-parent.plotSize.x/2.0
			var plotHeight = parent.plotSize.y*(y+1)-parent.plotSize.y/2.0
			var pos : Vector3
			
			if is_left: pos = Vector3(0,plotHeight,plotWidth)
			else: pos = Vector3(plotWidth,plotHeight,0)
			
			var plot = Plot.new(x,y,pos, is_left)
			grid.append(plot)
			add_child(plot)

func find_plot(x:int, y:int) -> Plot:
	if x < 0:
		x += width
	var result = grid.find_custom(func(p): return p.x == x and p.y == y)
	return grid[result]
