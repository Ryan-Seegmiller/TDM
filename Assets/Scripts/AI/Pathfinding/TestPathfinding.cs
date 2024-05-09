using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{
    /// <summary>
    /// MonoBehaviour to test pathfinding.
    /// </summary>
    public class TestPathfinding : MonoBehaviour
    {
        public Transform start;
        public Transform end;

        public LevelGeneration.WorldGenerator worldGenerator;

        public List<NavNode> path;

        [ContextMenu("Pathfind")]
        public void Pathfind()
        {
            path = worldGenerator.navGrid.FindPath(start, end);
            if (path == null)
            {
                Debug.LogWarning($"TestPathfinding.Pathfind() :: TestPathfinding.path == null");
            }
            else
            {
                string msg = "TestPathfinding.Pathfind() :: Path: ";
                foreach (NavNode node in path) { msg += $"{node}, "; }
                Debug.LogWarning(msg);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            foreach (NavNode node in path)
            {
                Gizmos.DrawWireSphere(worldGenerator.navGrid.GetWorldPosition(node.x, node.y), 5f);
            }
        }
    }
}