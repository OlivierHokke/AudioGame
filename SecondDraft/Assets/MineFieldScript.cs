using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineFieldScript : MonoBehaviour {
    public int SizeX = 10;
    public int SizeY = 10;
    public int MineCount = 7;
    public float GridCellSize = 2f;
    public int MaxPlacementTries = 1000;
    public Point2 StartPos = new Point2();
    public Point2 EndPos = new Point2(10, 10);
    public GameObject Mine;

    private bool[,] grid;
    private List<GameObject> mines = new List<GameObject>();
    private int placementTries = 0;

	// Use this for initialization
	void Start () 
    {
        if (OutsideGrid(StartPos))
            Debug.LogWarning("Start Position is outside the grid");
        if (OutsideGrid(EndPos))
            Debug.LogWarning("End Position is outside the grid");
        grid = new bool[SizeX, SizeY];
        placementTries = 0;
        PlaceMines();
        AddMines();
	}

    private void ResetGrid()
    {
        for (int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                grid[i, j] = false;
            }
    }

    private bool OutsideGrid(Point2 pos)
    {
        return pos.x < 0 || pos.x >= SizeX || pos.y < 0 || pos.y >= SizeY;
    }

    private void PlaceMines()
    {
        placementTries++;
        ResetGrid();
        int placedMines = 0;
        while (placedMines < MineCount)
        {
            int x = Random.Range(0, SizeX);
            int y = Random.Range(0, SizeY);
            if (!grid[x, y] && !(StartPos.x == x && StartPos.y == y) && !(EndPos.x == x && EndPos.y == y))
            {
                grid[x, y] = true;
                placedMines++;
            }
        }
        if (!PathExists() && placementTries < MaxPlacementTries)
            PlaceMines();
    }

    private bool PathExists()
    {
        var reachables = new List<Point2>();
        reachables.Add(StartPos);
        int marked = 1;
        while (marked != 0)
        {
            int reachableCount = reachables.Count;
            foreach(var reachable in reachables.ToArray())
            {
                var neighbors = new Point2[] { 
                    new Point2(reachable.x - 1, reachable.y),
                    new Point2(reachable.x + 1, reachable.y),
                    new Point2(reachable.x, reachable.y + 1),
                    new Point2(reachable.x, reachable.y - 1)
                };
                foreach(var neighbor in neighbors)
                {
                    if (!OutsideGrid(neighbor) && !grid[neighbor.x, neighbor.y] && !reachables.Contains(neighbor))
                        reachables.Add(neighbor);
                }
            }
            marked = reachables.Count - reachableCount;
        }
        return reachables.Contains(EndPos);
    }

    private void AddMines()
    {
        // Clear the old mines
        foreach (var mine in mines)
            GameObject.Destroy(mine);
        mines.Clear();

        // Add the current mines
        for(int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                if (grid[i, j])
                {
                    var mine = (GameObject)GameObject.Instantiate(Mine, this.transform.position + GetRelativeMinePosition(i, j), new Quaternion());
                    mine.transform.parent = this.transform;
                    mines.Add(mine);
                }
            }
    }

	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Vector3 cellSize = transform.localScale.divide(new Vector3(SizeX, 1, SizeY));
        for(int i = 0; i < SizeX; i++)
            for (int j = 0; j < SizeY; j++)
            {
                Gizmos.color = grid != null && grid[i, j] ? Color.red : Color.cyan;
                if (StartPos.x == i && StartPos.y == j)
                    Gizmos.color = Color.green;
                if (EndPos.x == i && EndPos.y == j)
                    Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(
                    transform.position + GetRelativeMinePosition(i, j),
                    cellSize);

            }
    }

    private Vector3 GetRelativeMinePosition(Point2 pos)
    {
        return GetRelativeMinePosition(pos.x, pos.y);
    }

    private Vector3 GetRelativeMinePosition(int i, int j)
    {
        Vector3 cellSize = transform.localScale.divide(new Vector3(SizeX, 1, SizeY));
        return - (transform.localScale / 2f)
            + cellSize / 2f
            + new Vector3((transform.localScale.x / (float)SizeX) * (float)i, 1, (transform.localScale.z / (float)SizeY) * (float)j);
    }
}
