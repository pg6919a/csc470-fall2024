using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject cellPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float spacing = 1.1f;

    [Header("Simulation Settings")]
    public float simulationRate = 0.1f;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI deathMessageText;
    public TextMeshProUGUI congratsMessageText;

    private CellScript[,] grid;
    private float simulationTimer;
    private bool isRunning = false;
    private float gameTime;

    private float congratsTimer = 0f;
    private bool hasFoundSurvivalCondition = false;

    void Start()
    {
        InitializeGrid();
    }

    void Update()
    {
        if (isRunning)
        {
            simulationTimer -= Time.deltaTime;
            gameTime += Time.deltaTime;

            if (simulationTimer <= 0)
            {
                Simulate();
                simulationTimer = simulationRate;
            }

            if (timerText != null)
            {
                timerText.text = $"Time: {gameTime:F2} seconds";
            }

            if (hasFoundSurvivalCondition)
            {
                congratsTimer += Time.deltaTime;
                if (congratsTimer >= 5f)
                {
                    DisplayCongratsMessage();
                }
            }
            else
            {
                CheckForSurvivalCondition();
            }
        }
    }

    public void StartSimulation()
    {
        isRunning = true;
        gameTime = 0f;
        deathMessageText.text = "";
        congratsMessageText.text = "";
        congratsTimer = 0f;
        hasFoundSurvivalCondition = false;
    }

    public void StopSimulation()
    {
        isRunning = false;
        deathMessageText.text = $"You died. You survived for {gameTime:F2} seconds";
    }

    private void Simulate()
    {
        bool[,] nextStates = new bool[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int aliveNeighbors = CountNeighbors(x, y);

                if (grid[x, y].alive)
                {
                    nextStates[x, y] = (aliveNeighbors == 2 || aliveNeighbors == 3);
                }
                else
                {
                    nextStates[x, y] = (aliveNeighbors == 3);
                }
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y].alive = nextStates[x, y];
                if (grid[x, y].alive)
                {
                    grid[x, y].aliveCount++;
                }
                grid[x, y].SetColor();
            }
        }

        if (IsGameOver())
        {
            StopSimulation();
        }
    }

    private int CountNeighbors(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                {
                    if (grid[nx, ny].alive)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    private bool IsGameOver()
    {
        foreach (var cell in grid)
        {
            if (cell.alive) return false;
        }
        return true;
    }

    public void ResetGame()
    {
        StopSimulation();
        gameTime = 0f;
        deathMessageText.text = "";
        congratsMessageText.text = "";
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        if (grid != null)
        {
            foreach (var cell in grid)
            {
                if (cell != null)
                {
                    Destroy(cell.gameObject);
                }
            }
        }

        grid = new CellScript[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity);
                CellScript cellScript = cell.GetComponent<CellScript>();

                if (cellScript != null)
                {
                    cellScript.alive = (Random.value > 0.5f);
                    cellScript.xIndex = x;
                    cellScript.yIndex = y;
                    cellScript.aliveCount = 0;
                    grid[x, y] = cellScript;
                    cellScript.SetColor();
                }
                else
                {
                    Debug.LogError("CellScript missing on prefab.");
                }
            }
        }
    }

    private void CheckForSurvivalCondition()
    {
        for (int x = 0; x < gridWidth - 1; x++)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                if (grid[x, y].alive && grid[x + 1, y].alive && grid[x, y + 1].alive && grid[x + 1, y + 1].alive)
                {
                    hasFoundSurvivalCondition = true;
                    return;
                }
            }
        }
        hasFoundSurvivalCondition = false;
    }

    private void DisplayCongratsMessage()
    {
        congratsMessageText.text = "Congrats! You survived!";
    }
}
