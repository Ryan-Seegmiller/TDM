using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Generated level data.
    /// </summary>
    public class Dungeon
    {
        /// <summary> The level's seed. </summary>
        public int seed;
        /// <summary> Current level in play. </summary>
        public int level;
        /// <summary> Total enemies spawned. </summary>
        public int enemyCount;
        public List<DungeonCell> branch;
        /// <summary>
        /// Dungeon constructor.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="level"></param>
        public Dungeon(int seed, int level)
        {
            this.seed = seed;
            this.level = level;
            branch = new List<DungeonCell>();
        }
    }

    /// <summary>
    /// A single cell's data in the dungeon
    /// </summary>
    public class DungeonCell
    {
        #region Types
        public struct V3Int
        {
            public int x, y, z;
            public V3Int(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public static implicit operator Vector3Int(V3Int vec) => new Vector3Int(vec.x, vec.y, vec.z);
            public static explicit operator V3Int(Vector3Int vec) => new V3Int(vec.x, vec.y, vec.z);
        }
        public struct V2Float
        {
            public float x, y;
            public V2Float(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
            public static implicit operator Vector2(V2Float vec) => new Vector2(vec.x, vec.y);
            public static explicit operator V2Float(Vector2 vec) => new V2Float(vec.x, vec.y);
        }
        #endregion

        #region Properties
        public V3Int cell;
        public V2Float direction;
        public List<DungeonCell> branch;
        #endregion

        #region Constructor
        public DungeonCell(V3Int cell, V2Float direction)
        {
            this.cell = cell;
            this.direction = direction;
            this.branch = new List<DungeonCell>();
        }
        public DungeonCell(Vector3Int cell, Vector2 direction) : this((V3Int)cell, (V2Float)direction) { }
        #endregion
    }
}
