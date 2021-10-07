using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaze : MonoBehaviour
{
    // Start is called before the first frame update
    enum CellState {EMPTY, WALL, FIXED_EMPTY, FIXED_WALL};
    
    private readonly int h = 36;  // h must be even
    private readonly int w = 36; // w must be even
    private readonly int fixed_range = 5;

    private const float GridSpaceSize = 2.0f;
    private const float targetTime = 300.0f;
    private float currentTimer = targetTime;
    private System.Random random;
    private GameObject[,] gameGrid;
    private CellState[,] cellCurrentState;
    private int[,] CloseWallCount;

    public GameObject FloorTilePrefab;
    public GameObject Player;
    public GameObject GameElement;
    public GameObject Environment;

    void Start()
    {
        random = new System.Random();
        gameGrid = new GameObject[h, w];
        cellCurrentState = new CellState[h, w];
        CloseWallCount = new int[h, w];

        Player.transform.position = new Vector3(-(float)h * GridSpaceSize / 2, 0, -w * GridSpaceSize / 2);
        GameElement.transform.position = new Vector3(-(float)h * GridSpaceSize / 2 + 3, 5, -w * GridSpaceSize / 2 + 3);
        Environment.transform.localScale = new Vector3((float)h * GridSpaceSize, 1, w * GridSpaceSize);

        FillMaze(0, 0);

        // Here you can set fixed some cells : call LockCell(i,j) to prevent cell(i,j) from changing state

    }

    // Update is called once per frame
    void Update()
    {
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0.0f)
        {
            currentTimer = targetTime;
            ResetMaze();
        }
    }
  
    private void FillCell(int i, int j)
    {
        if (cellCurrentState[i, j] == CellState.FIXED_WALL || cellCurrentState[i, j] == CellState.FIXED_EMPTY)
        {
            return;
        }
        
        for (int di=-1; di<=1; di++)
        {
            for (int dj=-1; dj<=1; dj++)
            {
                if (i+di>=0 && i+di<h && j+dj>=0 && j + dj < w)
                {
                    CloseWallCount[i + di, j + dj]++;
                }
            }
        }

        cellCurrentState[i, j] = CellState.WALL;

        gameGrid[i, j] = Instantiate(FloorTilePrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        gameGrid[i, j].transform.SetParent(transform);
        gameGrid[i, j].transform.localPosition = new Vector3((-(float)h/ 2 + i) * GridSpaceSize, 0.05f, (-(float)w /2 + j) * GridSpaceSize);
        gameGrid[i, j].transform.localScale = new Vector3(GridSpaceSize, 1, GridSpaceSize);
        gameGrid[i, j].transform.name = "PlayGround : (" + i.ToString() + " , " + j.ToString() + ")";
    }

    private void DoEmptyCell(int i, int j)
    {
        if (cellCurrentState[i, j] != CellState.WALL)
        {
            return;
        }

        for (int di = -1; di <= 1; di++)
        {
            for (int dj = -1; dj <= 1; dj++)
            {
                if (i + di >= 0 && i + di < h && j + dj >= 0 && j + dj < w)
                {
                    CloseWallCount[i + di, j + dj]--;
                }
            }
        }

        Destroy(gameGrid[i, j]);
        gameGrid[i, j] = null;
        cellCurrentState[i, j] = CellState.EMPTY;
    }

    private void LockCell(int i, int j)
    {
        if (cellCurrentState[i, j] == CellState.WALL)
        {
            cellCurrentState[i, j] = CellState.FIXED_WALL;
        }
        else if (cellCurrentState[i, j] == CellState.EMPTY)
        {
            cellCurrentState[i, j] = CellState.FIXED_EMPTY;
        }
    }

    private void UnlockCell(int i, int j)
    {
        if (cellCurrentState[i, j] == CellState.FIXED_WALL)
        {
            cellCurrentState[i, j] = CellState.WALL;
        }
        else if (cellCurrentState[i, j] == CellState.FIXED_EMPTY)
        {
            cellCurrentState[i, j] = CellState.EMPTY;
        }
    }


    private void FillMaze(int current_i, int current_j)
    {
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if (Mathf.Abs(i - current_i) <= fixed_range && Mathf.Abs(j - current_j) <= fixed_range)
                {
                    continue;
                }

                float test = (float)random.NextDouble();
                float generateCriteria = 0.0f;
                if (CloseWallCount[i, j] == 8)
                {
                    generateCriteria = 0.0f;
                }
                else if (CloseWallCount[i, j] == 7)
                {
                    generateCriteria = 0.3f;
                }
                else if (CloseWallCount[i, j] >= 2 && CloseWallCount[i, j] < 7)
                {
                    generateCriteria = 0.5f;
                }
                else
                {
                    generateCriteria = 0.7f;
                }

                if (test <= generateCriteria)
                {
                    continue;
                }
                FillCell(i, j);
            }
        }
    }

    private void ResetMaze()
    {

        Vector3 PlayerPosition = Player.transform.position;

        int current_i = Mathf.FloorToInt(Mathf.Clamp((float)h /2 + PlayerPosition.x / GridSpaceSize, 0.0f, (float)h - 0.5f));
        int current_j = Mathf.FloorToInt(Mathf.Clamp((float)w /2 + PlayerPosition.z / GridSpaceSize, 0.0f, (float)w - 0.5f));

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if ((Mathf.Abs(i - current_i) > fixed_range || Mathf.Abs(j - current_j) > fixed_range))
                {
                    DoEmptyCell(i, j);
                }
            }
        }
        FillMaze(current_i, current_j);
    }

    public bool IsWall(int i, int j)
    {
        return (cellCurrentState[i, j] == CellState.WALL || cellCurrentState[i, j] == CellState.FIXED_WALL);
    }
}
