extends ColorRect

@onready var label = $Label
var target_position: Vector2
var tile_value: int

# 生成された時の初期設定
func setup(val: int, start_pos: Vector2):
	position = start_pos
	target_position = start_pos
	update_value(val)

# 合体して数字が変わる時の処理
func update_value(val: int):
	tile_value = val
	label.text = str(val)
	
	# 簡易的な色分け（Unity版と同じような色味に）
	match val:
		2: color = Color("#eee4da"); label.add_theme_color_override("font_color", Color("#776e65"))
		4: color = Color("#ede0c8"); label.add_theme_color_override("font_color", Color("#776e65"))
		8: color = Color("#f2b179"); label.add_theme_color_override("font_color", Color.WHITE)
		16: color = Color("#f59563"); label.add_theme_color_override("font_color", Color.WHITE)
		32: color = Color("#f67c5f"); label.add_theme_color_override("font_color", Color.WHITE)
		64: color = Color("#f65e3b"); label.add_theme_color_override("font_color", Color.WHITE)
		_: color = Color("#edcf72"); label.add_theme_color_override("font_color", Color.WHITE) # 128以上

func _process(delta):
	# 常に「目標地点」に向かって滑らかに移動し続ける（UnityのLerpと同じ）
	position = position.lerp(target_position, delta * 15.0)
