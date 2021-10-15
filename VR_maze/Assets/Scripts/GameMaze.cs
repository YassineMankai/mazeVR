using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaze : MonoBehaviour
{
    // Start is called before the first frame update
    enum CellState { EMPTY, WALL };

    private readonly int h = 28;  // h must be even
    private readonly int w = 28; // w must be even
    private readonly int fixed_range = 2;

    private const float GridSpaceSize = 2.0f;
    private const float targetTime = 90.0f;
    private float currentTimer = targetTime;
    private System.Random random;
    private GameObject[,] gameGrid;
    private CellState[,] cellCurrentState;
    private int[,] CloseWallCount;

    public GameObject FloorTilePrefab;
    public GameObject Player;
    public GameObject Environment;
    public GameObject[] Portals;
    private static int nbResolvedMiniGame = 0;
    static public List<System.Tuple<int, int>> fixed_area; // 0 : player pos  ;  1 : first portal;  2: second portal; 3: ...

    void Start()
    {
        random = new System.Random();
        gameGrid = new GameObject[h, w];
        cellCurrentState = new CellState[h, w];
        CloseWallCount = new int[h, w];
        CloseWallCount = new int[h, w];

        Environment.transform.localScale = new Vector3((float)h * GridSpaceSize, 1, w * GridSpaceSize);

        if (fixed_area == null)
        {
            fixed_area = new List<System.Tuple<int, int>>();


            fixed_area.Add(new System.Tuple<int, int>(fixed_range, fixed_range)); // player pos

            fixed_area.Add(new System.Tuple<int, int>(random.Next(2 * h / 3, h - fixed_range - 1), random.Next(fixed_range, w / 3))); // 1st portal : Differences
            fixed_area.Add(new System.Tuple<int, int>(random.Next(fixed_range, h / 3), random.Next(2 * w / 3, w - fixed_range - 1))); // 2nd portal : PipePuzzle 
            //fixed_area.Add(new System.Tuple<int, int>(random.Next(2 * h / 3, h - fixed_range - 1), random.Next(2 * w / 3, w - fixed_range - 1))); // 3rd portal : ObjectCollector

        }

        for (int portal_index = 0; portal_index < Portals.Length; portal_index++)
        {
            int portal_i = fixed_area[portal_index + 1].Item1;
            int portal_j = fixed_area[portal_index + 1].Item2;
            Portals[portal_index].transform.position = new Vector3((-(float)h / 2 + portal_i) * GridSpaceSize, 0.0f, (-(float)w / 2 + portal_j) * GridSpaceSize);
            Portals[portal_index].GetComponent<SceneSwitch>().setOpen();
        }

        int player_i;
        int player_j;
        switch (SceneSwitch.getSourceScene())
        {
            case "Differences":
                player_i = fixed_area[1].Item1;
                player_j = fixed_area[1].Item2;
                nbResolvedMiniGame++;
                Portals[0].GetComponent<SceneSwitch>().setClosed();
                break;
            case "PipePuzzle":
                player_i = fixed_area[2].Item1;
                player_j = fixed_area[2].Item2;
                nbResolvedMiniGame++;
                Portals[1].GetComponent<SceneSwitch>().setClosed();
                break;
        /**    case "ObjectCollector":
                player_i = fixed_area[3].Item1;
                player_j = fixed_area[3].Item2;
                break; **/
            default:
                player_i = 1;
                player_j = 1;
                break;
        }
        player_i += fixed_range - 1;
        player_j += fixed_range - 1;
        Player.transform.position = new Vector3((-(float)h / 2 + player_i) * GridSpaceSize, 0, (-(float)w / 2 + player_j) * GridSpaceSize);


        GenerateMaze();
        EmptyFixedArea();
        FillMaze();

    }



    // Update is called once per frame
    void Update()
    {
        if (nbResolvedMiniGame == 2)
        {
            HandleVictory();
        }
        
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0.0f)
        {
            currentTimer = targetTime;
            Vector3 PlayerPosition = Player.transform.position;

            fixed_area[0] = new System.Tuple<int, int>(Mathf.FloorToInt(Mathf.Clamp((float)h / 2 + PlayerPosition.x / GridSpaceSize, 0.0f, (float)h - 0.5f)),
                                            Mathf.FloorToInt(Mathf.Clamp((float)w / 2 + PlayerPosition.z / GridSpaceSize, 0.0f, (float)w - 0.5f)));
            GenerateMaze();
            EmptyFixedArea();
            FillMaze();
        }
    }

    private void HandleVictory()
    {
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if (cellCurrentState[i, j] == CellState.WALL)
                {
                    FlipState(i, j);
                }
            }
        }
        GenerateMaze();
        ShowVictoryMenu();

    }

    private void ShowVictoryMenu()
    {
        Vector3 PlayerPosition = Player.transform.position;
        

    }

    private void GenerateMaze()
    {
        for (int j = 0; j < w * h * 4; j++)
        {
            int pos_i = random.Next(0, h);
            int pos_j = random.Next(0, w);

            float test = (float)random.NextDouble();

            float flipCriteria;

            if (CloseWallCount[pos_i, pos_j] >= 7)
            {
                flipCriteria = (cellCurrentState[pos_i, pos_j] == CellState.EMPTY) ? 1.0f : 0.0f;
            }

            else if (CloseWallCount[pos_i, pos_j] == 6)
            {
                flipCriteria = (cellCurrentState[pos_i, pos_j] == CellState.EMPTY) ? 0.1f : 0.9f;

            }
            else if (CloseWallCount[pos_i, pos_j] >= 2 && CloseWallCount[pos_i, pos_j] < 6)
            {
                flipCriteria = (cellCurrentState[pos_i, pos_j] == CellState.EMPTY) ? 0.4f : 0.6f;
            }
            else
            {
                flipCriteria = (cellCurrentState[pos_i, pos_j] == CellState.EMPTY) ? 0.7f : 0.3f;
            }

            if (test <= flipCriteria)
            {
                //FlipState(pos_i, pos_j);
            }

        }

    }
    private void EmptyFixedArea()
    {
        for (int i_fixed = 0; i_fixed < fixed_area.Count; i_fixed++)
        {
            int current_i = fixed_area[i_fixed].Item1;
            int current_j = fixed_area[i_fixed].Item2;

            for (int di = -fixed_range; di <= fixed_range; di++)
            {
                for (int dj = -fixed_range / 2; dj <= fixed_range / 2; dj++)
                {
                    if (cellCurrentState[current_i + di, current_j + dj] == CellState.WALL)
                    {
                        FlipState(current_i + di, current_j + dj);
                    }
                }
            }
        }
    }

    private void FlipState(int i, int j)
    {
        int dCount;
        if (cellCurrentState[i, j] == CellState.EMPTY)
        {
            dCount = 1;
            cellCurrentState[i, j] = CellState.WALL;
        }
        else
        {
            dCount = -1;
            cellCurrentState[i, j] = CellState.EMPTY;
        }

        for (int di = -1; di <= 1; di++)
        {
            for (int dj = -1; dj <= 1; dj++)
            {
                if (i + di >= 0 && i + di < h && j + dj >= 0 && j + dj < w)
                {
                    CloseWallCount[i + di, j + dj] += dCount;
                }
            }
        }
    }

    private void FillCell(int i, int j)
    {
        gameGrid[i, j] = Instantiate(FloorTilePrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        gameGrid[i, j].transform.SetParent(transform);
        gameGrid[i, j].transform.localPosition = new Vector3((-(float)h / 2 + i) * GridSpaceSize, 0.05f, (-(float)w / 2 + j) * GridSpaceSize);
        gameGrid[i, j].transform.localScale = new Vector3(GridSpaceSize, 1, GridSpaceSize);
        gameGrid[i, j].transform.name = "PlayGround : (" + i.ToString() + " , " + j.ToString() + ")";
    }

    private void DoEmptyCell(int i, int j)
    {
        Destroy(gameGrid[i, j]);
        gameGrid[i, j] = null;
        cellCurrentState[i, j] = CellState.EMPTY;
    }

    private void FillMaze()
    {
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if (cellCurrentState[i, j] == CellState.WALL)
                {
                    if (gameGrid[i, j] == null)
                    {
                        FillCell(i, j);
                    }
                }
                else
                {
                    if (gameGrid[i, j] != null)
                    {
                        DoEmptyCell(i, j);
                    }
                }
            }
        }
    }

}
