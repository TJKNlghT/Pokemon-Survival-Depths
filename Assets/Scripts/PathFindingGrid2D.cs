using System.Collections.Generic;
using UnityEngine;

public class PathFindingGrid2D : MonoBehaviour
{
    [System.Serializable]
    public class Node
    {
        public int x;
        public int y;
        public bool walkable;
        public Vector2 worldPos;

        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public Node parent;

        public Node(int x, int y, bool walkable, Vector2 worldPos)
        {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
            this.worldPos = worldPos;
        }
    }

    [Header("Grid Settings")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 1f;
    public Vector2 origin = Vector2.zero;

    [Header("Collision")]
    public LayerMask unwalkableMask;
    public float checkRadius = 0.3f;

    private Node[,] grid;

    private void Awake()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPoint = origin + new Vector2(x + 0.5f, y + 0.5f) * cellSize;
                bool walkable = !Physics2D.OverlapCircle(worldPoint, checkRadius, unwalkableMask);
                grid[x, y] = new Node(x, y, walkable, worldPoint);
            }
        }
    }

    public Node WorldToNode(Vector2 worldPos)
    {
        float percentX = (worldPos.x - origin.x) / (width * cellSize);
        float percentY = (worldPos.y - origin.y) / (height * cellSize);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(percentX * width), 0, width - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(percentY * height), 0, height - 1);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // 8-directional offsets
        int[,] offsets = new int[,]
        {
        {  1,  0 }, { -1,  0 }, {  0,  1 }, {  0, -1 }, // straight
        {  1,  1 }, {  1, -1 }, { -1,  1 }, { -1, -1 }  // diagonals
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int nx = node.x + offsets[i, 0];
            int ny = node.y + offsets[i, 1];

            if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                continue;

            Node neighbour = grid[nx, ny];
            if (!neighbour.walkable)
                continue;

            // prevent corner cutting for diagonal neighbours
            int dx = offsets[i, 0];
            int dy = offsets[i, 1];

            if (dx != 0 && dy != 0) // diagonal
            {
                Node side1 = grid[node.x + dx, node.y];     // horizontal step
                Node side2 = grid[node.x, node.y + dy];     // vertical step

                if (!side1.walkable || !side2.walkable)
                    continue; // one of the sides is blocked, don't squeeze through
            }

            neighbours.Add(neighbour);
        }

        return neighbours;
    }

    int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        // 8-direction distance:
        if (dx > dy)
            return 14 * dy + 10 * (dx - dy);
        else
            return 14 * dx + 10 * (dy - dx);
    }

    public List<Vector2> FindPath(Vector2 startWorld, Vector2 targetWorld)
    {
        Node startNode = WorldToNode(startWorld);
        Node targetNode = WorldToNode(targetWorld);

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        foreach (var n in grid)
        {
            n.gCost = int.MaxValue;
            n.hCost = 0;
            n.parent = null;
        }

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost ||
                    (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in GetNeighbours(current))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCost = current.gCost + GetDistance(current, neighbour);
                if (newCost < neighbour.gCost)
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null; // no path
    }

    List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current.worldPos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private void OnDrawGizmosSelected()
    {
        if (grid == null)
            return;

        foreach (var node in grid)
        {
            Gizmos.color = node.walkable ? Color.white : Color.black;
            Gizmos.DrawWireCube(node.worldPos, Vector3.one * (cellSize * 0.9f));
        }
    }
}

