@tool
extends RichTextEffect
class_name RichTextSkew

# Define the tag name: [skew]
var bbcode = "skew"

func _process_custom_fx(char_fx):
	# Get the "amount" parameter from the tag, default to 0.5
	var skew_amount = char_fx.env.get("amount", 0.5)
	
	# We manipulate the transform of the individual character
	# A skew is essentially shifting the X position based on the Y position
	var offset_x = (char_fx.range.y - char_fx.range.x) * skew_amount
	
	# Apply the transform
	char_fx.transform = char_fx.transform.translated(Vector2(char_fx.range.x * skew_amount, 0))
	char_fx.transform.x.y = skew_amount # This creates the actual slant
	
	return true
