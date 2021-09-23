using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaze : MonoBehaviour
{
    // Start is called before the first frame update
    enum CellState {EMPTY, WALL, FIXED_EMPTY, FIXED_WALL};
    
    private readonly int h = 50;
    private readonly int w = 50;
    private readonly int fixed_range = 10;

    public float GridSpaceSize = 2.0f;
    private const float targetTime = 60.0f;
    private float currentTimer = targetTime;
    private System.Random random;
    private GameObject[,] gameGrid;
    CellState[,] cellCurrentState;

    public GameObject FloorTilePrefab;
    public GameObject Player;
    public GameObject Environment;

    void Start()
    {
        random = new System.Random();
        gameGrid = new GameObject[h, w];
        cellCurrentState = new CellState[h, w];

        Player.transform.position = new Vector3(-h * GridSpaceSize / 2, 0, -w * GridSpaceSize / 2);
        Environment.transform.localScale = new Vector3(h * GridSpaceSize + 1, 1, w * GridSpaceSize + 1);
        
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
        
        cellCurrentState[i, j] = CellState.WALL;

        gameGrid[i, j] = Instantiate(FloorTilePrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        gameGrid[i, j].transform.SetParent(transform);
        gameGrid[i, j].transform.localPosition = new Vector3((-w/ 2 + j) * GridSpaceSize, 0.05f, (-h/2 + i) * GridSpaceSize);
        gameGrid[i, j].transform.localScale = new Vector3(GridSpaceSize, 1, GridSpaceSize);
        gameGrid[i, j].transform.name = "PlayGround : (" + j.ToString() + " , " + i.ToString() + ")";
    }

    private void DoEmptyCell(int i, int j)
    {
        if (cellCurrentState[i, j] != CellState.WALL)
        {
            return;
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

                int test = random.Next(0, w * h);
                if (Mathf.Abs(test - (i * w + j)) > 300)
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

        int current_j = Mathf.FloorToInt(Mathf.Clamp((w/2 + PlayerPosition.x) / GridSpaceSize, 0.0f, w - 0.5f));
        int current_i = Mathf.FloorToInt(Mathf.Clamp((h/2 + PlayerPosition.z) / GridSpaceSize, 0.0f, h - 0.5f));

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
