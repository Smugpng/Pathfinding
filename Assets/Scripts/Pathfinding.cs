using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class Pathfinding : MonoBehaviour
{
    public int GridWidth, GridHeight;
    public float Probability;

    [Header("New Start Pos")]
    [SerializeField] private Vector2Int newStart;
    [Header("New Goal Pos")]
    [SerializeField] private Vector2Int newGoal;
    [Header("The coordinates I want to change")]
    [SerializeField] public Vector2Int CoordToChange;

    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int start = new Vector2Int(0, 1);
    private Vector2Int goal = new Vector2Int(4, 4);
    private Vector2Int next;
    private Vector2Int current;
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };
    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0, 1, 0, 0, 0 },
        { 0, 1, 0, 1, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 1, 0, 0, 0 },
        { 0, 1, 1, 1, 0, 1, 0, 0, 0 },
        { 0, 1, 1, 1, 0, 1, 0, 0, 0 },
        { 0, 1, 1, 1, 0, 1, 0, 0, 0 },
        { 0, 1, 1, 1, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 0 }
    };
    private void Start()
    {
        newStart = start;
        newGoal = goal;
        grid[newStart.x, newStart.y] = 0;
        grid[newGoal.x, newGoal.y] = 0;
        GenerateRandomGrid(GridWidth, GridHeight, Probability);
    }
    private void Update()
    {
        FindPath(start, goal);
    }
    private void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = RandomCheck(obstacleProbability);
            }
        }

    }
    private int RandomCheck(float chance)
    {
        if (chance < Random.Range(0, 10))
        {
            return 0;
        }
        else return 1;

    }
    private void OnDrawGizmos()
    {
        float cellSize = 1f;
        // Draw grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f,
                cellSize));
            }
        }
        // Draw path
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y *
            cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }
        // Draw start and goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new
        Vector3(cellSize, 0.1f, cellSize));
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new
        Vector3(cellSize, 0.1f, cellSize));
    }
    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 &&
        point.y < grid.GetLength(0);
    }
    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int,
        Vector2Int>();
        cameFrom[start] = start;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (current == goal)
            {
                break;
            }
            foreach (Vector2Int direction in directions)
            {
                next = current + direction;
                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !
                cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }
        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }
        // Trace path from goal to start
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }
    public void ClearPath()
    {
        newStart = start;
        newGoal = goal;
        path.Clear();
    }

    public void UpdatePositions()
    {
        start = newStart;
        goal = newGoal;
        grid[newStart.x, newStart.y] = 0;
        grid[newGoal.x, newGoal.y] = 0;
    }

    public void AddObstacle(Vector2Int obstacle)
    {
        if (grid[obstacle.x, obstacle.y] == 1)
        {
            grid[obstacle.x, obstacle.y] = 0;
            ClearPath();
        }
        else
        {
            grid[obstacle.x, obstacle.y] = 1;
            ClearPath();
        }
    }

}
[CustomEditor(typeof(Pathfinding)), CanEditMultipleObjects]
public class PathFindingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Pathfinding myScript = (Pathfinding)target;

        if (GUILayout.Button("Change positions"))
        {
            myScript.UpdatePositions();
            myScript.ClearPath();
        }

        if (GUILayout.Button("Add/remove Obstacle"))
        {
            myScript.AddObstacle(myScript.CoordToChange);
        }
    }
}
