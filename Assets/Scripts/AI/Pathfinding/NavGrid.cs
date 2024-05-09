using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{
    /// <summary> Pathfinding navigation grid. </summary>
    [System.Serializable]
    public class NavGrid
    {
        /// <summary> Size of the grid. </summary>
        public Vector2Int size = new Vector2Int(10, 10);
        /// <summary> Distance between nodes. </summary>
        public float cellSize = 1f;
        /// <summary> Reference transfrom for world position calculations. </summary>
        public Transform transform;
        /// <summary> Obstacle UnityEngine.LayerMask. </summary>
        public LayerMask layerMask = Physics.DefaultRaycastLayers;
#if UNITY_EDITOR
        public bool gizmos = true;
#endif

        /// <summary> Grid nodes. </summary>
        private NavNode[,] grid = new NavNode[0, 0];
        /// <summary>
        /// Public grid accessor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public NavNode this[int x, int y] => grid[x, y];
        [System.Obsolete("Offset is depricated; Legacy value from code import.")]
        private Vector3 Offset { get { return new Vector3(0, 0, 0) * cellSize; } }

        /// <summary>
        /// NavGrid constructor.
        /// </summary>
        /// <param name="size">NavGrid.size</param>
        /// <param name="cellSize">NavGrid.cellSize</param>
        /// <param name="transform">NavGrid.transfrom</param>
        public NavGrid(Vector2Int size, float cellSize = 1, Transform transform = null)
        {
            this.size = size;
            this.cellSize = cellSize;
            this.transform = transform;
            this.grid = new NavNode[0, 0];
        }

        /// <summary>
        /// Find a path to the target location.
        /// </summary>
        /// <param name="startPos">Starting position of the path.</param>
        /// <param name="endPos">Target end position of the path.</param>
        /// <param name="absoluteEnd">Only return a path if there is a path to the target position's closest node.</param>
        /// <returns></returns>
        public List<NavNode> FindPath(Vector3 startPos, Vector3 endPos, bool absoluteEnd = false)
        {
            if (Vector3.Distance(startPos, endPos) < cellSize)
            {
                Debug.LogWarning($"NavGrid.FindPath() :: startPos: {startPos} and endPos: {endPos} are too close to each other." +
                    $"\nExpected: >={cellSize}; Actual: {Vector3.Distance(startPos, endPos)}\n\n");
                return null;
            }
            NavNode startNode = GetNodeFromWorldPosition(startPos);
            if (startNode.neighbors.Count == 0) { startNode = GetNodeFromWorldPosition(startPos + Vector3.forward); }
            NavNode endNode = GetNodeFromWorldPosition(endPos);
            if (endNode.neighbors.Count == 0) { endNode = GetNodeFromWorldPosition(endPos + Vector3.forward); }
            return AStar(startNode, endNode, absoluteEnd);
        }
        /// <summary>
        /// Find a path to the target location.
        /// </summary>
        /// <param name="startT">Starting position of the path as a UnityEngine.Transform.</param>
        /// <param name="endT">Target end position of the path as a UnityEngine.Trasform.</param>
        /// <param name="absoluteEnd">Only return a path if there is a path to the target position's closest node.</param>
        /// <returns></returns>
        public List<NavNode> FindPath(Transform startT, Transform endT, bool absoluteEnd = false) { return FindPath(startT.position, endT.position, absoluteEnd); }
        /// <summary>
        /// Find a path to the target location.
        /// </summary>
        /// <param name="startT">Starting position of the path as a UnityEngine.Transform.</param>
        /// <param name="endPos">Target end position of the path.</param>
        /// <param name="absoluteEnd"></param>
        /// <returns></returns>
        public List<NavNode> FindPath(Transform startT, Vector3 endPos, bool absoluteEnd = false) { return FindPath(startT.position, endPos, absoluteEnd); }
        /// <summary>
        /// Find a path to the target location.
        /// </summary>
        /// <param name="startPos">Starting position of the path.</param>
        /// <param name="endT">Target end position of the path as a UnityEngine.Trasform.</param>
        /// <param name="absoluteEnd"></param>
        /// <returns></returns>
        public List<NavNode> FindPath(Vector3 startPos, Transform endT, bool absoluteEnd = false) { return FindPath(startPos, endT.position, absoluteEnd); }

        #region Grid Methods
        /// <summary> Initialize the navigation grid.</summary>
        [ContextMenu("Init()")]
        internal void Init()
        {
            Debug.Log("NavMesh.Init()");
            OnValidate();
            this.grid = new NavNode[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    grid[x, y] = new NavNode(x, y);
                }
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    //grid[x,y].neighbors = new List<NavNode>();
                    // horizontal neighbors
                    //if (x > 0) { AddNeighbor(x, y, x - 1, y); }
                    //if (x < size - 1) { AddNeighbor(x, y, x + 1, y); }
                    if (x > 0) { AddNeighborWithPhysicsCheck(x, y, x - 1, y); }
                    if (x < size.x - 1) { AddNeighborWithPhysicsCheck(x, y, x + 1, y); }
                    // vertical neighbors
                    //if (y > 0) { AddNeighbor(x, y, x, y - 1); }
                    //if (y < size - 1) { AddNeighbor(x, y, x, y + 1); }
                    if (y > 0) { AddNeighborWithPhysicsCheck(x, y, x, y - 1); }
                    if (y < size.y - 1) { AddNeighborWithPhysicsCheck(x, y, x, y + 1); }
                }
            }
        }
        /// <summary>
        /// Check if the neighbor node node is a valid connection.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        private void AddNeighborWithPhysicsCheck(int x, int y, int nX, int nY)
        {
            if (x == nX && y == nY) { return; }
            Vector3 dir = transform ? transform.TransformDirection(new Vector3(nX - x, 0, nY - y)).normalized : new Vector3(nX - x, 0, nY - y).normalized;
            Ray ray = new Ray(GetWorldPosition(x, y), dir);
            if (!Physics.Raycast(ray, cellSize, layerMask))
            {
                grid[x, y].neighbors.Add(grid[nX, nY]);
            }
        }
        [System.Obsolete("AddNeighbor is depricated; Legacy method from code import.", true)]
        private void AddNeighbor(int x, int y, int nX, int nY)
        {
            /*if (x == nX && y == nY) { return; }
            LevelData levelData = GameManager.instance.levelData;
            if (levelData[x, y].type == LevelData.TileType.Null && levelData[nX, nY].type == LevelData.TileType.Null)
            {
                grid[x, y].neighbors.Add(grid[nX, nY]);
            }*/
        }
        /// <summary>
        /// Get the world position of a node.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 GetWorldPosition(int x, int y)
        {
            Vector3 pos = new Vector3(x, 0, y) * cellSize - Offset;
            return (transform) ? transform.TransformPoint(pos) : pos;
        }
        /// <summary>
        /// Get the world position of a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Vector3 GetWorldPosition(NavNode node)
        {
            return GetWorldPosition(node.x, node.y);
        }
        /// <summary>
        /// Get the closest NavNode to a world position.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        internal NavNode GetNodeFromWorldPosition(Vector3 worldPos)
        {
            Vector3 localPos = ((transform) ? transform.InverseTransformPoint(worldPos) : worldPos) + Offset;
            int x = Mathf.RoundToInt(localPos.x / cellSize);
            int y = Mathf.RoundToInt(localPos.z / cellSize);
            x = Mathf.Clamp(x, 0, grid.GetLength(0) - 1);
            y = Mathf.Clamp(y, 0, grid.GetLength(1) - 1);
            return grid[x, y];
        }
        #endregion

        private int CalculateHeuristic(NavNode a, NavNode b)
        {
            Vector3 aPos = GetWorldPosition(a.x, a.y);
            Vector3 bPos = GetWorldPosition(b.x, b.y);
            return (int)Vector3.Distance(aPos, bPos);
        }

        #region A* (A-Star)
        private List<NavNode> AStar(NavNode startNode, NavNode endNode, bool absoluteEnd = false)
        {


            if (startNode == endNode)
            {
                Debug.LogWarning($"NavGrid.AStar() :: start node and end node are the same node.");
                return null;
            }
            List<NavNode> open = new List<NavNode>() { startNode };
            List<NavNode> closed = new List<NavNode>();

            // reset graph
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    grid[i, j].Reset();
                }
            }
            startNode.g = 0;
            startNode.h = CalculateHeuristic(startNode, endNode);
            NavNode closestNode = startNode;
            while (open.Count > 0)
            {
                NavNode currentNode = GetFittestNode(open);
                if (currentNode == endNode)
                {
                    // done!...ish
                    return CalculatePath(endNode);
                }
                if (currentNode.h < closestNode.h) { closestNode = currentNode; }

                // current node has been searched
                open.Remove(currentNode);
                closed.Add(currentNode);

                foreach (NavNode neighbor in currentNode.neighbors)
                {
                    if (closed.Contains(neighbor)) { continue; }

                    int g = (int)currentNode.g + CalculateHeuristic(currentNode, neighbor);
                    if (g < neighbor.g)
                    {
                        neighbor.previous = currentNode;
                        neighbor.g = g;
                        neighbor.h = CalculateHeuristic(neighbor, endNode);

                        if (!open.Contains(neighbor))
                        {
                            open.Add(neighbor);
                        }
                    }
                }
            }
            // no path
            if (absoluteEnd)
            {
                Debug.LogWarning($"NavGrid.AStar() :: Path not found, analyzed {closed.Count} nodes.");
                return null;
            }
            else
            {
                Debug.LogWarning($"NavGrid.AStar() :: Absolute path not found, returning closest node.");
                return CalculatePath(closestNode);
            }
        }
        #endregion
        #region A* Methods
        /// <summary>
        /// List the path from a "goal" to it's root.
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        List<NavNode> CalculatePath(NavNode goal)
        {
            List<NavNode> path = new List<NavNode>();
            path.Add(goal);
            NavNode currentNode = goal;
            while (currentNode.previous != null)
            {
                path.Add(currentNode.previous);
                currentNode = currentNode.previous;
            }
            path.Reverse();
            return path;
        }
        /// <summary>
        /// Get the node with the lowest cost.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        NavNode GetFittestNode(List<NavNode> nodes)
        {
            NavNode fittestNode = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].F < fittestNode.F)
                {
                    fittestNode = nodes[i];
                }
            }
            return fittestNode;
        }
        #endregion

        /// <summary>
        /// Validate grid settings.
        /// </summary>
        internal void OnValidate()
        {
            size.x = (size.x < 10) ? 10 : size.x;
            size.y = (size.y < 10) ? 10 : size.y;
            cellSize = (cellSize < 1) ? 1 : cellSize;
        }

        /// <summary>
        /// Draw grid gizmos (Editor only).
        /// </summary>
        /// <param name="gridColor">Color to draw the Gizmos.</param>
        internal void DrawGizmos(Color gridColor)
        {
#if UNITY_EDITOR
            if (!gizmos) { return; }
            Gizmos.color = gridColor;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Gizmos.DrawWireCube(GetWorldPosition(x, y), Vector3.one * Mathf.Clamp(cellSize, 1, 20) * .5f);
                }
            }
#endif
        }
    }
}