extends Control

@export var tile_scene: PackedScene
@onready var grid = $Background/Grid
@onready var tile_container = $Background/TileContainer

# 【追加】UIノードの取得
@onready var score_label = $ScoreLabel
@onready var game_over_label = $GameOverLabel

var board = []
var visual_tiles = []
var is_game_over = false

func _ready():
	# 最初からリセット関数を呼んで初期化する
	reset_game()

func _input(event):
	if is_game_over:
		# 【追加】ゲームオーバー時にスペースキー（ui_accept）で再スタート
		if event.is_action_pressed("ui_accept"):
			reset_game()
		return
		
	var moved = false
	if event.is_action_pressed("ui_up"): moved = move(Vector2(0, -1))
	elif event.is_action_pressed("ui_down"): moved = move(Vector2(0, 1))
	elif event.is_action_pressed("ui_left"): moved = move(Vector2(-1, 0))
	elif event.is_action_pressed("ui_right"): moved = move(Vector2(1, 0))
	
	if moved:
		spawn_tile()
		update_score() # 【追加】移動後にスコアを更新
		check_game_over()

func move(dir: Vector2) -> bool:
	var moved = false
	var merged = []
	for y in range(4):
		merged.append([false, false, false, false])

	var start_x = 3 if dir.x == 1 else 0
	var end_x = -1 if dir.x == 1 else 4
	var step_x = -1 if dir.x == 1 else 1

	var start_y = 3 if dir.y == 1 else 0
	var end_y = -1 if dir.y == 1 else 4
	var step_y = -1 if dir.y == 1 else 1

	for y in range(start_y, end_y, step_y):
		for x in range(start_x, end_x, step_x):
			if board[y][x] == 0:
				continue

			var curr_x = x
			var curr_y = y

			while true:
				var next_x = curr_x + dir.x
				var next_y = curr_y + dir.y

				if next_x < 0 or next_x >= 4 or next_y < 0 or next_y >= 4:
					break

				if board[next_y][next_x] == 0:
					board[next_y][next_x] = board[curr_y][curr_x]
					board[curr_y][curr_x] = 0
					
					visual_tiles[next_y][next_x] = visual_tiles[curr_y][curr_x]
					visual_tiles[curr_y][curr_x] = null
					
					var target_pos = grid.position + grid.get_child(next_y * 4 + next_x).position
					visual_tiles[next_y][next_x].target_position = target_pos
					
					curr_x = next_x
					curr_y = next_y
					moved = true
					
				elif board[next_y][next_x] == board[curr_y][curr_x] and not merged[next_y][next_x]:
					board[next_y][next_x] *= 2
					board[curr_y][curr_x] = 0
					merged[next_y][next_x] = true
					
					var moving_tile = visual_tiles[curr_y][curr_x]
					var static_tile = visual_tiles[next_y][next_x]
					visual_tiles[curr_y][curr_x] = null
					
					var target_pos = grid.position + grid.get_child(next_y * 4 + next_x).position
					moving_tile.target_position = target_pos
					get_tree().create_timer(0.15).timeout.connect(moving_tile.queue_free)
					
					static_tile.update_value(board[next_y][next_x])
					
					moved = true
					break
				else:
					break
	
	return moved

func spawn_tile():
	var empty_cells = []
	for y in range(4):
		for x in range(4):
			if board[y][x] == 0:
				empty_cells.append(Vector2(x, y))
	
	if empty_cells.size() > 0:
		var cell = empty_cells.pick_random()
		var val = 2 if randf() < 0.9 else 4
		
		board[cell.y][cell.x] = val
		
		var new_tile = tile_scene.instantiate()
		tile_container.add_child(new_tile)
		
		var start_pos = grid.position + grid.get_child(cell.y * 4 + cell.x).position
		new_tile.setup(val, start_pos)
		
		visual_tiles[cell.y][cell.x] = new_tile

# 【追加】盤面の数字の合計を計算して表示する
func update_score():
	var total_score = 0
	for y in range(4):
		for x in range(4):
			total_score += board[y][x]

	if score_label:
		score_label.text = "Score: " + str(total_score)

# 【追加】ゲームをまっさらにリセットする
func reset_game():
	is_game_over = false
	if game_over_label:
		game_over_label.hide()
	
	board.clear()
	visual_tiles.clear()
	
	for y in range(4):
		board.append([0, 0, 0, 0])
		visual_tiles.append([null, null, null, null])
		
	# 画面上の既存のタイルをすべて削除
	for child in tile_container.get_children():
		child.queue_free()
		
	await get_tree().process_frame
	
	spawn_tile()
	spawn_tile()
	update_score()

func check_game_over():
	for y in range(4):
		for x in range(4):
			if board[y][x] == 0: return
			
	for y in range(4):
		for x in range(4):
			if x < 3 and board[y][x] == board[y][x + 1]: return
			if y < 3 and board[y][x] == board[y + 1][x]: return

	is_game_over = true
	# 【追加】ゲームオーバーの文字を表示する
	if game_over_label:
		game_over_label.show()
