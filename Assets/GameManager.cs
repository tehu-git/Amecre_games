using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI設定")]
    public Transform[] cells;
    public GameObject tilePrefab;
    public Transform tileContainer;
    
    [Header("追加したUI")]
    public TextMeshProUGUI scoreText;    // スコア表示用
    public GameObject gameOverText;      // ゲームオーバー表示用

    private int[,] board = new int[4, 4];
    private GameObject[,] visualTiles = new GameObject[4, 4];
    private bool isGameOver = false;

    void Start()
    {
        // 最初の起動時もリセット処理を呼んで初期化する
        ResetGame();
    }

    void Update()
    {
        // ゲームオーバー中の処理
        if (isGameOver)
        {
            // スペースキーが押されたら盤面をリセットして再スタート
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetGame();
            }
            return;
        }

        bool moved = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)) moved = Move(0, -1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) moved = Move(0, 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) moved = Move(-1, 0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) moved = Move(1, 0);

        if (moved)
        {
            SpawnTile();
            UpdateScore(); // 移動後にスコアを更新
            CheckGameOver();
        }
    }

    bool Move(int dirX, int dirY)
    {
        bool moved = false;
        bool[,] merged = new bool[4, 4];

        int startX = dirX == 1 ? 3 : 0;
        int endX = dirX == 1 ? -1 : 4;
        int stepX = dirX == 1 ? -1 : 1;

        int startY = dirY == 1 ? 3 : 0;
        int endY = dirY == 1 ? -1 : 4;
        int stepY = dirY == 1 ? -1 : 1;

        for (int y = startY; y != endY; y += stepY)
        {
            for (int x = startX; x != endX; x += stepX)
            {
                if (board[x, y] == 0) continue;

                int currentX = x;
                int currentY = y;

                while (true)
                {
                    int nextX = currentX + dirX;
                    int nextY = currentY + dirY;

                    if (nextX < 0 || nextX >= 4 || nextY < 0 || nextY >= 4) break;

                    // ① 移動先が空の場合
                    if (board[nextX, nextY] == 0)
                    {
                        board[nextX, nextY] = board[currentX, currentY];
                        board[currentX, currentY] = 0;

                        visualTiles[nextX, nextY] = visualTiles[currentX, currentY];
                        visualTiles[currentX, currentY] = null;
                        
                        Vector3 targetPos = cells[nextY * 4 + nextX].position;
                        visualTiles[nextX, nextY].GetComponent<Tile>().targetPosition = targetPos;

                        currentX = nextX;
                        currentY = nextY;
                        moved = true;
                    }
                    // ② 同じ数字で合体する場合
                    else if (board[nextX, nextY] == board[currentX, currentY] && !merged[nextX, nextY])
                    {
                        board[nextX, nextY] *= 2;
                        board[currentX, currentY] = 0;
                        merged[nextX, nextY] = true;

                        GameObject movingTile = visualTiles[currentX, currentY];
                        GameObject staticTile = visualTiles[nextX, nextY];
                        visualTiles[currentX, currentY] = null;

                        Vector3 targetPos = cells[nextY * 4 + nextX].position;
                        movingTile.GetComponent<Tile>().targetPosition = targetPos;
                        Destroy(movingTile, 0.15f);

                        staticTile.GetComponent<Tile>().UpdateValue(board[nextX, nextY]);

                        moved = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return moved;
    }

    void SpawnTile()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (board[x, y] == 0) emptyCells.Add(new Vector2Int(x, y));
            }
        }

        if (emptyCells.Count > 0)
        {
            Vector2Int randomCell = emptyCells[Random.Range(0, emptyCells.Count)];
            int val = Random.value < 0.9f ? 2 : 4;
            
            board[randomCell.x, randomCell.y] = val;

            GameObject newTile = Instantiate(tilePrefab, tileContainer);
            int index = randomCell.y * 4 + randomCell.x;
            
            newTile.GetComponent<Tile>().Setup(val, cells[index].position);
            visualTiles[randomCell.x, randomCell.y] = newTile;
        }
    }

    // 【追加】盤面の数字の合計を計算して表示する
    void UpdateScore()
    {
        int totalScore = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                totalScore += board[x, y];
            }
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore;
        }
    }

    // 【追加】ゲームをまっさらにリセットする
    void ResetGame()
    {
        isGameOver = false;
        
        // ゲームオーバーの文字を隠す
        if (gameOverText != null) gameOverText.SetActive(false);

        // 盤面のデータと画面上のタイルをすべて削除する
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                board[x, y] = 0;
                if (visualTiles[x, y] != null)
                {
                    Destroy(visualTiles[x, y]);
                    visualTiles[x, y] = null;
                }
            }
        }

        // 最初の2枚を生成し直す
        SpawnTile();
        SpawnTile();
        UpdateScore(); // スコア表示もリセット
    }

    void CheckGameOver()
    {
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                if (board[x, y] == 0) return;

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (x < 3 && board[x, y] == board[x + 1, y]) return;
                if (y < 3 && board[x, y] == board[x, y + 1]) return;
            }
        }

        isGameOver = true;
        
        // ゲームオーバーの文字を表示する
        if (gameOverText != null) gameOverText.SetActive(true);
    }
}