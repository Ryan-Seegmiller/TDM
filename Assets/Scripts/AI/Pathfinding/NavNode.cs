using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelGeneration;

namespace AI.Pathfinding
{
    /// <summary> A single pathfinding navigation node. </summary>
    [System.Serializable]
    public class NavNode
    {
        /// <summary> NavGrid position of this node. </summary>
        public int x, y;
        /// <summary> Neighbors to this node. </summary>
        [System.NonSerialized] internal List<NavNode> neighbors = new List<NavNode>();
        /// <summary> World position of this node. </summary>
        public Vector3 position => WorldGenerator.instance.navGrid.GetWorldPosition(this);

        #region Pathfinding Scores
        /// <summary> A* distance to goal. </summary>
        public int g = 0;
        /// <summary> A* huristic score. </summary>
        public int h = 0;
        /// <summary> Sum of NavNode.g and NavNode.h </summary>
        public int F { get { return g + h; } }

        internal NavNode previous = null;
        internal NavNode reversePrevious = null;
        #endregion

        /// <summary>
        /// NavNode constructor.
        /// </summary>
        /// <param name="x">Positon x on NavGrid</param>
        /// <param name="y">Positon y on NavGrid</param>
        public NavNode(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.neighbors = new List<NavNode>();
        }

        /// <summary> Reset A* values. /summary>
        public void Reset()
        {
            g = int.MaxValue;
            previous = null;
            reversePrevious = null;
        }
        public override string ToString() { return $"({x}, {y})"; }
    }
}