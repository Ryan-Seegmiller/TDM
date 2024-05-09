using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using AI.Pathfinding;

namespace LevelGeneration
{
    /// <summary>
    /// Procedural generator for world tiles.
    /// </summary>
    public class WorldGenerator : MonoBehaviour
    {
        #region Singleton
        /// <summary> WorldGenerator singleton. </summary>
        public static WorldGenerator instance;
        private void Singleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogWarning($"WorldGenerator :: Another instance of WorldGenerator attemted to exist.");
            }
        }
        #endregion

        #region Types
        /// <summary> Enum of cell types for data caching. </summary>
        public enum CellType { Empty, Room, Corridor }
        public delegate void OnDungeonLoaded(Dungeon dungeon);
        /// <summary>
        /// Corner edges of the world.
        /// </summary>
        public struct WorldBounds
        {
            public Vector2Int min;
            public Vector2Int max;

            public WorldBounds(Vector2Int min, Vector2Int max)
            {
                this.min = min;
                this.max = max;
            }
        }
        #endregion

        #region Properties
        public const int maxSize = 50;
        /// <summary> Loot table with 100% key chance. </summary>
        public AI.LootTable keyLootTable;
        /// <summary> Cell on WorldGenerator.tilemap where the player spawns.</summary>
        private Vector3Int spawnPos;
        /// <summary> Player spawn position. </summary>
        public Vector3 spawnPosition => tilemap.CellToWorld(spawnPos) + new Vector3(10, 0, -10);

        /// <summary> Seeded System.Random for procedural world generation. </summary>
        private System.Random worldRandom;
        private int _currentLevel = 0;
        public int CurrentLevel { get { return _currentLevel; } private set { _currentLevel = value; } }

        /// <summary> Current level data. </summary>
        public Dungeon dungeon;

        [Header("Tilemap")]
        /// <summary> Tilemap to generate to. </summary>
        public Tilemap tilemap;
        /// <summary> Corridor tile. </summary>
        public TileBase tileCorridor;
        /// <summary> Room tile </summary>
        public TileBase tileRoom;
        /// <summary> </summary>
        public TileBase endRoom;

        /// <summary> Prefab for the shop. </summary>
        [Header("Scene References")]
        public GameObject shopPrefab;

        [Header("Settings")]
        /// <summary> World random seed. Set before the world instancing. </summary>
        public int seed;
        /// <summary> Seed used during world instancing. If `WorldGenerator.seed == 0` this will be set to a random seed. </summary>
        private int runtimeSeed;

        /// <summary> Target number of rooms on the root branch. </summary>
        [Tooltip("Target number of rooms on the root branch. ")] public int roomCount = 10;
        /// <summary> Probability that a tile is a room. </summary>
        [Range(0, 1), Tooltip("Probability that a tile is a room")] public float roomChance = .5f;
        /// <summary> Probability of a branch starting. </summary>
        [Range(0, 1), Tooltip("Chance of a branch starting. ")] public float branchChance = .1f;
        /// <summary> Max degrees a branch can turn in a single step. </summary>
        [Range(10, 90), Tooltip("Max degrees a branch can turn in a single step. ")] public int maxCrawlDirectionChange = 55;
        /// <summary> Max number of AI per room tile. </summary>
        [Range(0, 5), Tooltip("Max number of AI per room tile.")] public int startEnemies = 2;
        /// <summary> 
        /// Multiplier for enemy count as the levels increase.
        /// 
        /// Function: `f(x)=s^x*m` where `s` is WorldGenerator.startEnemies, `m` is WorldGenerator.enemyIncreaseMul and `x` is the level. 
        /// </summary>
        [Range(0, 1)] public float enemyIncreaseMul = .5f;
        /// <summary> Probability of enemies spawning in a corridor. </summary>
        [Range(0, 1)] public float enemyInCorridorChance = .2f;
        /// <summary> Probability of a shop spawning in a room. </summary>
        [Range(0, 1)] public float shopChance = .15f;

        /// <summary> Dungeon pathfinding navigation grid. </summary>
        [Header("Pathfinding")]
        public NavGrid navGrid;
        /// <summary> Cube corner edges of the dungeon as a bounding box. </summary>
        private WorldBounds worldBounds = new WorldBounds();
        #endregion

        #region Main
        /// <summary>
        /// Load a specific level. (NOTE: This method is asynchronous.)
        /// </summary>
        /// <param name="level">The level to load.</param>
        /// <param name="onDungeonLoaded">Callback delegate.</param>
        public void LoadDungeon(int level, OnDungeonLoaded onDungeonLoaded)
        {
            //LoadingScreen.instance.Toggle();
            if (CurrentLevel == level && CurrentLevel != 0)
            {
                Debug.LogWarning($"WorldGenerator :: The requested level ({level}) and the current level ({CurrentLevel}) are the same.");
                return;
            }
            else if (level <= 1)
            {
                ClearDungeon();
                NewDungeon();
                dungeon = new Dungeon(runtimeSeed, CurrentLevel);
                StartCoroutine(GenerateDungeon(dungeon, onDungeonLoaded));
            }
            else if (CurrentLevel > level)
            {
                Debug.LogError($"WorldGenerator :: The requsted level ({level}) precedes the current level ({CurrentLevel}) and cannot be loaded in this verion. Maybe one day. �\\_(?)_/�");
            }
            else
            {
                while (CurrentLevel < level)
                {
                    ClearDungeon();
                    CurrentLevel++;
                    StartCoroutine(GenerateDungeon(new Dungeon(runtimeSeed, CurrentLevel), (Dungeon dungeon) =>
                    {
                        if (level == CurrentLevel) { onDungeonLoaded?.Invoke(dungeon); }
                    }));
                }
            }
            LoadingScreen.instance.Toggle();
        }
        /// <summary>
        /// Reset the dungeon and seeded random.
        /// </summary>
        private void NewDungeon()
        {
            // set runtime seed
            runtimeSeed = (seed != 0) ? seed : new System.Random().Next();
            worldRandom = new System.Random(runtimeSeed);
            CurrentLevel = 1;
        }
        /// <summary>
        /// Generates a dungeon from (0,0). (NOTE: This method is asynchronous.)
        /// </summary>
        /// <param name="dungeon">Dungeon Data.</param>
        /// <param name="onDungeonLoaded">Callback delegate.</param>
        /// <returns></returns>
        private IEnumerator GenerateDungeon(Dungeon dungeon, OnDungeonLoaded onDungeonLoaded)
        {
            Vector3Int cell = spawnPos = Vector3Int.zero;
            Vector2 crawlDirection = Vector2.right;
            int rooms = roomCount;

            tilemap.SetTile(cell, tileRoom);
            CrawlStep(ref cell, ref crawlDirection);
            tilemap.SetTile(cell, tileCorridor);
            CrawlStep(ref cell, ref crawlDirection);

            bool crawlDone = false;
            while (!crawlDone)
            {
                DungeonCell rootCell = new DungeonCell(cell, crawlDirection);
                dungeon.branch.Add(rootCell);

                SetTile(cell, ref rooms, dungeon);

                // create a branch
                if (Chance(branchChance, ref worldRandom))
                {
                    Vector3Int brenchCell = cell;
                    Vector2 branchDirection = (Quaternion.Euler(0, 0, 90 * (Chance(.5f, ref worldRandom) ? 1 : -1)) * crawlDirection).normalized;
                    int branchRooms = (int)(rooms * .5f);
                    bool branchCrawlDone = false;
                    while (!branchCrawlDone)
                    {
                        DungeonCell branchCell = new DungeonCell(brenchCell, branchDirection);
                        rootCell.branch.Add(branchCell);
                        SetTile(brenchCell, ref branchRooms, dungeon);
                        // continue this branch
                        if (branchRooms <= 0) { branchCrawlDone = true; }
                        else { CrawlStep(ref brenchCell, ref branchDirection); }
                        yield return null;
                    }
                }

                // continue this branch
                if (rooms <= 0)
                {
                    crawlDone = true;

                    // end door
                    CrawlStep(ref cell, ref crawlDirection);
                    tilemap.SetTile(cell, endRoom);
                }
                else { CrawlStep(ref cell, ref crawlDirection); }
                yield return null;
            }

            // set one enemy to always drop a key
            AI.AIBrain[] enemies = FindObjectsOfType<AI.AIBrain>();
            enemies[worldRandom.Next(enemies.Length)].lootTable = keyLootTable;

            // pathfinding
            GameObject go = new GameObject("NavGridTransformReference");
            go.transform.SetParent(transform);
            go.transform.position = tilemap.CellToWorld(new Vector3Int(worldBounds.min.x, worldBounds.max.y, 0)) + new Vector3(10, 0, -10);
            navGrid.transform = go.transform;
            Vector2Int bounds = worldBounds.max - worldBounds.min + Vector2Int.one;
            navGrid.size = bounds;
            navGrid.cellSize = 20;
            navGrid.Init();

            Debug.Log($"WorldGenerator :: Dungeon Complete. {{ seed : {dungeon.seed}, level : {dungeon.level}, enemyCount : {dungeon.enemyCount} }}");
            onDungeonLoaded?.Invoke(dungeon);
        }
        /// <summary>
        /// Delete instanced level.
        /// </summary>
        private void ClearDungeon()
        {
            tilemap.ClearAllTiles();

            // delete AI
            AI.AIBrain[] enemies = FindObjectsOfType<AI.AIBrain>();
            for (int i = enemies.Length - 1; i >= 0; i--) { Destroy(enemies[i].gameObject); }
        }
        #endregion

        #region Generation Methods
        /// <summary>
        /// Crawl to the next tile.
        /// </summary>
        /// <param name="cell">Stating UnityEngine.Tilemaps.Tilemap tile.</param>
        /// <param name="direction">Current crawl direction.</param>
        /// <param name="rand">Seeded System.Random for RNG.</param>
        private void CrawlStep(ref Vector3Int cell, ref Vector2 direction)
        {
            float rotation = worldRandom.Next(-maxCrawlDirectionChange, maxCrawlDirectionChange);
            direction = (Quaternion.Euler(0, 0, rotation) * direction).normalized;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                cell.x += (direction.x >= 0) ? 1 : -1;
            }
            else { cell.y += (direction.y >= 0) ? 1 : -1; }

            if (worldBounds.min.x > cell.x) { worldBounds.min.x = cell.x; }
            if (worldBounds.max.x < cell.x) { worldBounds.max.x = cell.x; }
            if (worldBounds.min.y > cell.y) { worldBounds.min.y = cell.y; }
            if (worldBounds.max.y < cell.y) { worldBounds.max.y = cell.y; }
        }
        /// <summary>
        /// Sets a single tile in the dungeon.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="rooms"></param>
        /// <param name="dungeon"></param>
        private void SetTile(Vector3Int cell, ref int rooms, Dungeon dungeon)
        {
            if (tilemap.GetTile(cell) == null) // do not overwrite an existing tile
            {
                System.Random roomRand = new System.Random(runtimeSeed + (cell.x * maxSize + cell.y));
                CellType cellType = Chance(roomChance, ref worldRandom) ? CellType.Room : CellType.Corridor;
                Vector3 roomPos = tilemap.CellToWorld(new Vector3Int(cell.x + 1, cell.y + 1, 0)) + new Vector3(-10, 0, 10);
                switch (cellType)
                {
                    case CellType.Room:
                        tilemap.SetTile(cell, tileRoom);
                        if (Chance(shopChance, ref roomRand))
                        {
                            Instantiate(shopPrefab, roomPos, Quaternion.identity, tilemap.transform.GetChild(tilemap.transform.childCount - 1));
                        }
                        else { SpawnAI(ref roomRand, roomPos, (int)Mathf.Pow(startEnemies, CurrentLevel * enemyIncreaseMul), dungeon, .8f); }
                        rooms--;
                        break;
                    case CellType.Corridor:
                        tilemap.SetTile(cell, tileCorridor);
                        if (Chance(enemyInCorridorChance, ref roomRand))
                        {
                            SpawnAI(ref roomRand, roomPos, (int)(Mathf.Pow(startEnemies, CurrentLevel * enemyIncreaseMul) * .5f), dungeon, .3f);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Spawn AI at a position.
        /// </summary>
        /// <param name="rand">Seeded System.Random for RNG.</param>
        /// <param name="roomPos">Max number of AI that can be spawned.</param>
        /// <param name="maxCount"></param>
        /// <param name="dungeon"></param>
        private void SpawnAI(ref System.Random rand, Vector3 roomPos, int maxCount, Dungeon dungeon, float scale = 1)
        {
            int count = rand.Next(maxCount) + 1;
            for (int i = 0; i < count; i++)
            {
                dungeon.enemyCount++;
                GameManager.instance.SpawnCharacter(roomPos,
                    (CharacterType)rand.Next(System.Enum.GetNames(typeof(CharacterType)).Length),
                    false,
                    new AI.AIBrain.PatrolArea(roomPos, Vector2.one * 20 * scale));
            }
        }
        #endregion

        #region RNG Methods
        /// <summary>
        /// True/False RNG based on a percentage.
        /// </summary>
        /// <param name="percent">Percent chance it is `true`.</param>
        /// <param name="rand">System.Random for RNG.</param>
        /// <returns></returns>
        protected static bool Chance(float percent, ref System.Random rand) => rand.NextDouble() < percent;
        /// <summary>
        /// True/False RNG base on a percentage.
        /// </summary>
        /// <param name="percent">Percent chance it is `true`.</param>
        /// <returns></returns>
        protected static bool Chance(float percent) => new System.Random().NextDouble() < percent;
        #endregion

        #region UnityCallbacks
        private void Awake()
        {
            Singleton();
        }
        private void OnEnable()
        {
            Singleton();
        }
        private void Start()
        {
            LoadDungeon(0, GameManager.instance.OnLevelLoaded);
        }
        private void OnDrawGizmos()
        {
            navGrid.DrawGizmos(Color.yellow);
        }
        #endregion
    }
}
