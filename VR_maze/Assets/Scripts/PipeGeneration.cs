using System;
using System.Collections.Generic;
using UnityEngine;

public class PipeGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    enum CubeState { FILLED, EMPTY };
    enum PipeType { DIRECT, CORNER };

    private const int h = 10;
    private const int d = 5;
    private const int w = 10;
    private const float densityMovableCubes = 0.2f;
    private int freeCubeCount = 0;


    private System.Random random;

    private List<Tuple<Vector3Int, CubeState>> path;

    private Dictionary<Tuple<Vector3Int, Vector3Int>, Vector3> cornerRotations;

    private Dictionary<Tuple<Vector3Int, Vector3Int>, int> weights;

    public GameObject CubesPool;

    public GameObject FixedDirectPipe;
    public GameObject FixedCornerPipe;
    public GameObject DirectPipe;
    public GameObject CornerPipe;
    public GameObject EmptyCube;


    void Start()
    {
        random = new System.Random();
        path = new List<Tuple<Vector3Int, CubeState>>();
        weights = new Dictionary<Tuple<Vector3Int, Vector3Int>, int>();

        cornerRotations = new Dictionary<Tuple<Vector3Int, Vector3Int>, Vector3>();
        calculateCornerRotations();

        GenerateRandomWeights();

        while (!PrecalculatePath())
        {
            weights = new Dictionary<System.Tuple<Vector3Int, Vector3Int>, int>();
            path = new List<Tuple<Vector3Int, CubeState>>();
            GenerateRandomWeights();
        }

        GenerateCubes();

    }


    private void GenerateRandomWeights()
    {
        Vector3Int[] directions = { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1) };

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < d; j++)
            {
                for (int k = 0; k < w; k++)
                {
                    Vector3Int pos = new Vector3Int(i, j, k);
                    foreach (Vector3Int dir in directions)
                    {
                        Vector3Int new_pos = pos + dir;
                        if (new_pos.x < h && new_pos.y < d && new_pos.z < w)
                        {
                            int value = random.Next(1, 11);
                            weights.Add(new Tuple<Vector3Int, Vector3Int>(pos, new_pos), value);
                            if (dir.z == 1)
                            {
                                weights.Add(new Tuple<Vector3Int, Vector3Int>(new_pos, pos), (int)Mathf.Ceil(value * 2.0f));
                            }
                            else
                            {
                                weights.Add(new Tuple<Vector3Int, Vector3Int>(new_pos, pos), (int)Mathf.Ceil(value * 1.5f));
                            }
                        }
                    }
                }
            }
        }
    }

    private bool PrecalculatePath()  // using a greedy algorithm
    {

        Vector3Int[] directions = { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1),
                                    new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(0, 0, -1)};

        bool[,,] visited = new bool[h, d, w];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < d; j++)
            {
                for (int k = 0; k < w; k++)
                {
                    visited[i, j, k] = false;
                }
            }
        }

        Vector3Int current_pos = new Vector3Int(0, 0, 0);
        Vector3Int last_cube = new Vector3Int(h - 1, d - 1, w - 1);
        Vector3Int unidentified_cube = new Vector3Int(-1, -1, -1);



        while (true)
        {
            visited[current_pos.x, current_pos.y, current_pos.z] = true;
            float test_state = (float)random.NextDouble();
            Tuple<Vector3Int, CubeState> newEntry = new Tuple<Vector3Int, CubeState>(current_pos, CubeState.FILLED);
            if (test_state < densityMovableCubes)
            {
                newEntry = new Tuple<Vector3Int, CubeState>(current_pos, CubeState.EMPTY);
            }
            path.Add(newEntry);


            if (current_pos == last_cube)
                return true;

            int min_dist = int.MaxValue;
            Vector3Int next_pos = new Vector3Int(-1, -1, -1);
            foreach (Vector3Int dir in directions)
            {
                Vector3Int new_pos = current_pos + dir;
                Tuple<Vector3Int, Vector3Int> arc = new Tuple<Vector3Int, Vector3Int>(current_pos, new_pos);

                if (new_pos.x >= 0 && new_pos.x < h && new_pos.y >= 0 && new_pos.y < d &&
                    new_pos.z >= 0 && new_pos.z < w && !visited[new_pos.x, new_pos.y, new_pos.z])
                {
                    if (weights[arc] < min_dist)
                    {
                        min_dist = weights[arc];
                        next_pos = new_pos;
                    }
                }
            }
            if (next_pos == unidentified_cube)
                return false;
            current_pos = next_pos;
        }
    }


    private void calculateCornerRotations()
    {
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.back, Vector3Int.left), new Vector3(0, 180, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.right, Vector3Int.forward), new Vector3(0, 180, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.back, Vector3Int.right), new Vector3(0, 270, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.left, Vector3Int.forward), new Vector3(0, 270, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.forward, Vector3Int.right), new Vector3(0, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.left, Vector3Int.back), new Vector3(0, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.forward, Vector3Int.left), new Vector3(0, 90, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.right, Vector3Int.back), new Vector3(0, 90, 0));


        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.down, Vector3Int.left), new Vector3(0, 90, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.right, Vector3Int.up), new Vector3(0, 90, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.down, Vector3Int.right), new Vector3(90, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.left, Vector3Int.up), new Vector3(90, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.up, Vector3Int.right), new Vector3(270, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.left, Vector3Int.down), new Vector3(270, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.up, Vector3Int.left), new Vector3(270, 180, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.right, Vector3Int.down), new Vector3(270, 180, 0));


        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.down, Vector3Int.back), new Vector3(0, 0, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.forward, Vector3Int.up), new Vector3(0, 0, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.down, Vector3Int.forward), new Vector3(180, 0, 270));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.back, Vector3Int.up), new Vector3(180, 0, 270));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.up, Vector3Int.forward), new Vector3(180, 0, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.back, Vector3Int.down), new Vector3(180, 0, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.up, Vector3Int.back), new Vector3(0, 0, 270));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.forward, Vector3Int.down), new Vector3(0, 0, 270));

        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.forward, Vector3Int.forward), new Vector3(0, 0, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.back, Vector3Int.back), new Vector3(0, 0, 90));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.up, Vector3Int.up), new Vector3(90, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.down, Vector3Int.down), new Vector3(90, 0, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.left, Vector3Int.left), new Vector3(0, 90, 0));
        cornerRotations.Add(new Tuple<Vector3Int, Vector3Int>(Vector3Int.right, Vector3Int.right), new Vector3(0, 90, 0));


    }

    private void GenerateCubes()
    {
        for (int index = 0; index < path.Count; index++)
        {
            Tuple<Vector3Int, CubeState> currentBox = path[index];

            Vector3Int gridPosition = currentBox.Item1;
            Vector3Int previousGridPosition;
            Vector3Int nextGridPosition;

            if (index == 0)
            {
                previousGridPosition = new Vector3Int(0, 0, -1);
                nextGridPosition = path[index + 1].Item1;
            }
            else if (index == path.Count - 1)
            {
                previousGridPosition = path[index - 1].Item1;
                nextGridPosition = new Vector3Int(9, 4, 10);
            }
            else
            {
                previousGridPosition = path[index - 1].Item1;
                nextGridPosition = path[index + 1].Item1;
            }

            Vector3Int movementInOut = nextGridPosition - previousGridPosition;

            Vector3 localPos = new Vector3(gridPosition.x * 2, gridPosition.y * 2, gridPosition.z * 2);


            bool isDirect = (int)Mathf.Abs(movementInOut[0]) == 2 || (int)Mathf.Abs(movementInOut[1]) == 2 || (int)Mathf.Abs(movementInOut[2]) == 2;
            GameObject a = createPipeGameObject(localPos, currentBox.Item2, isDirect ? PipeType.DIRECT : PipeType.CORNER);
            a.transform.Rotate(cornerRotations[new Tuple<Vector3Int, Vector3Int>(gridPosition - previousGridPosition, nextGridPosition - gridPosition)]);
        }


    }

    private GameObject createPipeGameObject(Vector3 localPos, CubeState state, PipeType type)
    {
        GameObject a;
        if (state == CubeState.FILLED)
        {
            if (type == PipeType.DIRECT)
            {
                a = Instantiate(FixedDirectPipe, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            }
            else
            {
                a = Instantiate(FixedCornerPipe, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            }
        }
        else
        {
            a = Instantiate(EmptyCube, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            GameObject b;
            if (type == PipeType.DIRECT)
            {
                a.tag = "EmptyDirectBox";
                b = Instantiate(DirectPipe, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            }
            else
            {
                a.tag = "EmptyCornerBox";
                b = Instantiate(CornerPipe, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            }
            b.transform.SetParent(CubesPool.transform);
            b.transform.localPosition = new Vector3(1 + 2.5f * (freeCubeCount / 12), 7, 2 + 3.0f * (freeCubeCount % 12));
            b.transform.name = $"Free Cube {freeCubeCount}";
            b.GetComponent<BoxCollisonHandler>().setFreeBoxIndex(freeCubeCount);
            freeCubeCount++;
        }

        a.transform.SetParent(transform);
        a.transform.localPosition = localPos;
        string prefix = (state == CubeState.EMPTY) ? "Empty Cube" : "Filled Cube";
        a.transform.name = prefix + ": (" + localPos.x.ToString() + " , " + localPos.y.ToString() + " , " + localPos.z.ToString() + ")";

        return a;
    }

    public void HandleBoxCollision(Vector3Int gridPosition, GameObject emptyCube, GameObject cubeToInsert)
    {
        Debug.Log($"handler callled {gridPosition} {cubeToInsert.name} {emptyCube.name}");

        Vector3 localPos = emptyCube.transform.localPosition;
        Quaternion localRot = emptyCube.transform.localRotation;
        emptyCube.SetActive(false);
        GameObject insertedCube;
        if (cubeToInsert.tag == "CornerBox")
            insertedCube = Instantiate(FixedCornerPipe, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        else
            insertedCube = Instantiate(FixedDirectPipe, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        cubeToInsert.SetActive(false);
        insertedCube.transform.SetParent(transform);
        insertedCube.transform.localPosition = localPos;
        insertedCube.transform.localRotation = localRot;

        insertedCube.transform.name = "Filled Cube : (" + localPos.x.ToString() + " , " + localPos.y.ToString() + " , " + localPos.z.ToString() + ")";
    }
}