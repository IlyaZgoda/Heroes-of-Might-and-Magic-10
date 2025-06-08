using Assets._Project.Code;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int _width = 10;
    private int _height = 10;
    private GridCellData[,] _grid;
    public static Grid Instance;
    public float CellSize { get; private set; } = 1.0f; 

    public void Construct()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        _grid = new GridCellData[_width, _height];

        GameObject prefab = (GameObject)Resources.Load("Cell");

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                Vector3 position = new(x * CellSize, 0, z * CellSize);
                GameObject cell = GameObject.Instantiate(prefab, position, Quaternion.identity);
                cell.name = $"Cell_{x}_{z}";
                _grid[x, z] = new GridCellData(x, z, cell);
            }
        }
    }

    public GridCellData GetCell(int x, int z)
    {
        if (x >= 0 && x < _width && z >= 0 && z < _height)
            return _grid[x, z];
        return null;
    }

    public bool IsCellWalkable(int x, int z)
    {
        var cell = GetCell(x, z);

        return cell != null && !cell.IsOccupied;
    }

    public void ClearHighlights()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                var cell = _grid[x, z].CellObject.GetComponent<Cell>();
                if (cell != null)
                    cell.ClearHighlight();
            }
        }
    }

    public void HighlightReachableCells(Unit unit)
    {
        ClearHighlights();
        var reachable = GetReachableCells(unit);

        foreach (var pos in reachable)
        {
            var cell = GetCell(pos.x, pos.y).CellObject?.GetComponent<Cell>();
            cell?.HighlightMove();
        }
    }

    private List<Vector2Int> GetReachableCells(Unit unit)
    {
        List<Vector2Int> reachable = new();
        int maxSteps = unit.CurrentMovementPoints;

        Queue<Vector2Int> frontier = new();
        HashSet<Vector2Int> visited = new();

        Vector2Int start = new(unit.GridX, unit.GridZ);
        frontier.Enqueue(start);
        visited.Add(start);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            int cost = Mathf.Abs(current.x - unit.GridX) + Mathf.Abs(current.y - unit.GridZ);

            if (cost > 0 && cost <= maxSteps)
                reachable.Add(current);

            foreach (var dir in new[] {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = current + dir;

                if (!IsInsideGrid(neighbor.x, neighbor.y)) continue;
                if (visited.Contains(neighbor)) continue;
                if (!IsCellWalkable(neighbor.x, neighbor.y)) continue;

                var path = Pathfinding.Instance.FindPath(unit.GridX, unit.GridZ, neighbor.x, neighbor.y);
                if (path.Count > 0 && path.Count <= maxSteps)
                {
                    frontier.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return reachable;
    }

    public void HighlightAttackableCells(Unit unit)
    {
        ClearHighlights();

        var attackable = GetAttackableCells(unit);

        foreach (var pos in attackable)
        {
            var cell = GetCell(pos.x, pos.y)?.CellObject?.GetComponent<Cell>();
            if (cell != null)
                cell.HighlightShootAttack();
        }
    }

    private List<Vector2Int> GetAttackableCells(Unit unit)
    {
        List<Vector2Int> attackable = new();

        int attackRange = unit.ShootRange;
        Vector2Int start = new(unit.GridX, unit.GridZ);

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                Vector2Int pos = new(x, z);
                int dist = Mathf.Abs(pos.x - start.x) + Mathf.Abs(pos.y - start.y);

                if (dist > 0 && dist <= attackRange)
                {
                    attackable.Add(pos);
                }
            }
        }

        Debug.Log($"Cells highlighted: {attackable.Count}");
        return attackable;
    }

    private bool IsInsideGrid(int x, int z)
    {
        return x >= 0 && x < _width && z >= 0 && z < _height;
    }
}
