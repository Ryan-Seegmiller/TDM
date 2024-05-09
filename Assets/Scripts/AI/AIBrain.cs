using System.Collections.Generic;
using UnityEngine;
using AI.Pathfinding;
using LevelGeneration;

namespace AI
{
    /// <summary>
    /// AI base class. Internally runs a statemachine.
    /// </summary>
    [RequireComponent(typeof(Movement))]
    public class AIBrain : MonoBehaviour, IPauseable
    {
        #region Types
        /// <summary> Statemachine states. </summary>
        public enum State { Patrol, Chase }
        /// <summary> AI sight values. </summary>
        [System.Serializable]
        public struct Sight
        {
            /// <summary> How far the AI can see. </summary>
            public float distance;
            /// <summary> Width the feild of view. </summary>
            public float fov;
            /// <summary> Offset relative to transform to start vision checks. </summary>
            public Vector3 headPosition;
            /// <summary> Sight layermask for obstacles. </summary>
            public LayerMask mask;

            public Sight(float distance, float fov, Vector3 headPosition)
            {
                this.distance = distance;
                this.fov = fov;
                this.headPosition = headPosition;
                this.mask = Physics.DefaultRaycastLayers;
            }

            /// <summary>
            /// Check if a positon is in sight.
            /// </summary>
            /// <param name="position">Root position</param>
            /// <param name="forward">Root forward direction</param>
            /// <param name="target">Target's position</param>
            /// <returns>bool</returns>
            public bool InSight(Vector3 position, Vector3 forward, Vector3 target)
            {
                if (Vector3.Distance(target, position) > distance) { return false; }
                if (Vector3.Angle(forward.normalized, (target - position).normalized) > fov * .5f) { return false; }
                return true;
            }
            /// <summary>
            /// Check if a positon is in sight.
            /// </summary>
            /// <param name="position">Root position</param>
            /// <param name="forward">Root forward direction</param>
            /// <param name="target">Target GameObject</param>
            /// <returns type="bool"></returns>
            public bool InSight(Vector2 position, Vector2 forward, GameObject target) => InSight(position, forward, target.transform.position);
            /// <summary>
            /// Check if a positon is in sight.
            /// </summary>
            /// <param name="position">Root position</param>
            /// <param name="forward">Root forward direction</param>
            /// <param name="target">Target Transform</param>
            /// <returns type="bool"></returns>
            public bool InSight(Vector2 position, Vector2 forward, Transform target) => InSight(position, forward, target.position);
            /// <summary>
            /// Forward direction of the FOV's right extreme.
            /// </summary>
            /// <param name="forward">Root forward direction</param>
            /// <returns>Vector2 Direction</returns>
            public Vector2 Right(Vector2 forward) => Quaternion.AngleAxis(fov * .5f, Vector3.back) * forward;
            /// <summary>
            /// Forward direction of the FOV's left extreme.
            /// </summary>
            /// <param name="forward">Root forward direction</param>
            /// <returns>Vector2 Direction</returns>
            public Vector2 Left(Vector2 forward) => Quaternion.AngleAxis(-fov * .5f, Vector3.back) * forward;
            /// <summary>
            /// Forward direction of the FOV rotated by a percent of the FOV.
            /// </summary>
            /// <param name="forward">Root forward direction</param>
            /// <param name="percent">Percent of the FOV (-1.00 is left extreme, 1.00 is right extreme)</param>
            /// <returns>Vector2 Direction</returns>
            public Vector2 FOVAngle(Vector2 forward, float percent) => Quaternion.AngleAxis(fov * .5f * percent, Vector3.back) * forward;
        }
        /// <summary> Area an AI patrols while waiting to spot the player. </summary>
        [System.Serializable]
        public struct PatrolArea
        {
            /// <summary> Whether this area is a circle or a rectangle. `true` make it a circle. </summary>
            public bool useCircle; // false is rect
            /// <summary> The center of the area. </summary>
            public Vector3 center;

            /// <summary> Radius of the patrol area if it is a circle. </summary>
            public float radius;
            /// <summary> Width and length of the area if it is a rectangle. </summary>
            public Vector2 rect;

            /// <summary>
            /// AIBrain.PatrolArea constructor. [Circle area]
            /// </summary>
            /// <param name="center">AIBrain.PatrolArea.center</param>
            /// <param name="radius">AIBrain.PatrolArea.radius</param>
            public PatrolArea(Vector3 center, float radius)
            {
                this.useCircle = true;
                this.center = center;
                this.radius = radius;
                this.rect = Vector2.zero;
            }
            /// <summary>
            /// AIBrain.PatrolArea constructor. [Rectangle area]
            /// </summary>
            /// <param name="center">AIBrain.PatrolArea.center</param>
            /// <param name="rect">AIBrain.PatrolArea.rect</param>
            public PatrolArea(Vector3 center, Vector2 rect)
            {
                this.useCircle = false;
                this.center = center;
                this.radius = 0;
                this.rect = rect;
            }

            /// <summary>
            /// Get a random location in the patrol area.
            /// </summary>
            /// <returns>Position</returns>
            public Vector3 GetRandomInArea()
            {
                System.Random rand = new System.Random();
                if (useCircle)
                {
                    return new Vector3((float)rand.NextDouble(), 0, (float)rand.NextDouble()).normalized * ((float)rand.NextDouble() * radius) + center;
                }
                else
                {
                    Vector3 generated;
                    generated.x = rand.Next((int)(rect.x * .5f)) * (rand.Next(2) == 1 ? 1 : -1);
                    generated.y = 0;
                    generated.z = rand.Next((int)(rect.y * .5f)) * (rand.Next(2) == 1 ? 1 : -1);
                    return generated + center;
                }
            }
            /// <summary>
            /// Checks if a position is in the patrol area.
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            public bool InArea(Vector3 position)
            {
                if (useCircle)
                {
                    return Vector3.Distance(position, center) <= radius;
                }
                else
                {
                    Vector3 offset = position - center;
                    return (offset.x < rect.x * .5f && offset.x > -rect.x * .5f) && (offset.z < rect.y * .5f && offset.z > -rect.y * .5f);
                }
            }

            public static bool operator ==(PatrolArea lhs, PatrolArea rhs)
            {
                if (lhs.useCircle != rhs.useCircle) { return false; }

                if (lhs.useCircle && rhs.useCircle)
                {
                    return lhs.radius == rhs.radius;
                }
                else
                {
                    return lhs.rect == rhs.rect;
                }
            }
            public static bool operator !=(PatrolArea lhs, PatrolArea rhs) => !(lhs.radius == rhs.radius);

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override string ToString()
            {
                return $"{center} : {(useCircle ? radius : rect)}";
            }
        }

        /// <summary> A path the AI can follow throught the dungeon. </summary>
        [System.Serializable]
        public class Path
        {
            /// <summary> List of NavNode that make a path. </summary>
            public List<NavNode> path;
            private int _currentNode;
            /// <summary> Current target node. </summary>
            public NavNode CurrentNode => (_currentNode >= path.Count) ? path[path.Count - 1] : path[_currentNode];

            /// <summary>
            /// Path constructor.
            /// </summary>
            /// <param name="path">Path.path</param>
            public Path(List<NavNode> path)
            {
                this.path = path;
                this._currentNode = 0;
            }
            /// <summary>
            /// Path constructor. (Generates a path)
            /// </summary>
            /// <param name="position">Position to start the path.</param>
            /// <param name="targetPosition">Target position of the path. </param>
            public Path(Vector3 position, Vector3 targetPosition) : this(LevelGeneration.WorldGenerator.instance.navGrid.FindPath(position, targetPosition))
            {
                this.path ??= new List<NavNode>();
            }

            /// <summary>
            /// Checks if the path has been completed; if so, return true.
            /// Then checks if the `position` has reached the current target node; if so, target the next node.
            /// </summary>
            /// <param name="position">Current position along the path.</param>
            /// <param name="tolerance">How close the position needs to be to a node to consider it having arrived.</param>
            /// <returns></returns>
            public bool CheckPath(Vector3 position, float tolerance)
            {
                path ??= new List<NavNode>();
                if (_currentNode >= path.Count) { return true; }
                tolerance = tolerance < .01f ? .01f : tolerance;
                _currentNode = (Vector3.Distance(position, CurrentNode.position) <= tolerance) ? _currentNode + 1 : _currentNode;
                if (_currentNode >= path.Count) { return true; }
                return false;
            }

            public override string ToString()
            {
                string str = $"{{ \"_currentNode\": {_currentNode}, \"path\": {{";
                foreach (NavNode node in path) { str += $" {node},"; }
                return str + " }";
            }
        }
        #endregion

        #region Properties
        /// <summary> UnityEngine.Component.transform.position </summary>
        public Vector3 position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        protected Movement movement;
        protected Character character;
        /// <summary> Current state of the AI. </summary>
        public State state;
        /// <summary> Loot table the entity will drop an item from when it dies. </summary>
        public LootTable lootTable;

        [Header("Traversal")]
        /// <summary> Assigned PatrolArea. </summary>
        public PatrolArea patrolArea = new PatrolArea(new Vector2(0, 0), new Vector3(4, 4));
        /// <summary> Distance the AI will consider it having arrived to it's destination. </summary>
        public float targetLocationTolerance = .2f;
        /// <summary> Chance of AI roaming outside it's room. </summary>
        [Range(0, 1), Tooltip("Chance of AI roaming outside it's room.")]
        public float roamChance = .1f;
        /// <summary> Distance the AI will attack from. </summary>
        public float attackDistance = .8f;
        /// <summary> Multiplier of the distance the AI will flee from the player. Multiplied by AIBrain.attackDistance. </summary>
        [Range(.001f, .9f), Tooltip("Multiplier of attack distance")] public float fleeDistanceMul = .5f;
        /// <summary> Distance the AI will flee from the player. </summary>
        protected float fleeDistance => attackDistance * fleeDistanceMul;
        /// <summary> Last world position where the AI "saw" the player.</summary>
        protected Vector3 playerLastKnown;
        /// <summary> World position the AI is going to. </summary>
        protected Vector3 targetLocation;
        /// <summary> Used to tell the forward direction the AI is going in. </summary>
        protected Vector2 lastInput = Vector2.up;

        [Header("Senses")]
        /// <summary> This AIs sight. </summary>
        [SerializeField] protected Sight sight = new Sight(10, 60, new Vector3(0, 1, 0));
        /// <summary> The path the AI is following through the dungeon. </summary>
        protected Path path = null;
        #endregion

        #region Methods        
        /// <summary>
        /// Update tick of the statemachine.
        /// </summary>
        public virtual void Statemachine()
        {
            // do not move if paused or dead
            if (IsPaused || character.isDead)
            {
                movement.input_direction = lastInput = Vector2.zero;
                return;
            }

            Senses();

            switch (state)
            {
                case State.Patrol:
                    OnPatrol();
                    break;
                case State.Chase:
                    OnChase();
                    break;
                default:
                    state = State.Patrol;
                    break;
            }
        }
        public bool isDebug = false;
        /// <summary>
        /// Checks visiion to player and changes current state based on environment.
        /// </summary>
        public virtual void Senses()
        {
            // player detection
            if (GameManager.instance.player != null)
            {
                playerLastKnown = GameManager.instance.player.transform.position;
                if (sight.InSight(position, new Vector3(lastInput.x, 0, lastInput.y).normalized, playerLastKnown))
                {
                    Vector3 playerDirection = (GameManager.instance.player.transform.position - transform.position).normalized;
                    if (Physics.Raycast(transform.position, playerDirection, out RaycastHit hit, sight.distance, sight.mask))
                    {
                        if (hit.collider != null && hit.transform.root.gameObject == GameManager.instance.player.gameObject)
                        {
                            state = State.Chase;
                        }
                        else if (state == State.Chase) { state = State.Patrol; }
                    }
                    else if (state == State.Chase) { state = State.Patrol; }
                }
                else if (state == State.Chase) { state = State.Patrol; }
            }
            else if (state == State.Chase) { state = State.Patrol; }

            // roam location
            if (patrolArea.InArea(transform.position))
            {
                if (Vector3.Distance(transform.position, targetLocation) <= targetLocationTolerance)
                {
                    if (Random.Range(0f, 1f) < roamChance)
                    {
                        ///TODO : Finish roam behaviour
                        /*DungeonCell dungeonCell = WorldGenerator.instance.dungeon.branch[Random.Range(0, WorldGenerator.instance.dungeon.branch.Count)];
                        Vector3 roamTarget = WorldGenerator.instance.tilemap.CellToWorld(dungeonCell.cell);
                        path = new Path(transform.position, roamTarget);
                        targetLocation = path.CurrentNode.position;*/
                        targetLocation = patrolArea.GetRandomInArea();
                    }
                    else
                    {
                        targetLocation = patrolArea.GetRandomInArea();
                    }
                }
            }
            else
            {
                if (path == null)
                {
                    path = new Path(transform.position, patrolArea.center);
                    targetLocation = path.path.Count == 0 ? patrolArea.GetRandomInArea() : path.CurrentNode.position;
                }
                else if (path.CheckPath(position, targetLocationTolerance))
                {
                    targetLocation = patrolArea.GetRandomInArea();
                    path = null;
                }
                else
                {
                    targetLocation = path.path.Count == 0 ? patrolArea.GetRandomInArea() : path.CurrentNode.position;
                }
            }
        }
        /// <summary> Patrol state behaviour. </summary>
        public virtual void OnPatrol()
        {
            Vector3 inputDir = (targetLocation - position).normalized;
            movement.input_direction = lastInput = new Vector2(inputDir.x, inputDir.z).normalized;
            movement.walkSpeedMultiplier = .4f;
        }
        /// <summary> Chase state behaviour </summary>
        public virtual void OnChase()
        {
            Vector3 playerDir = (playerLastKnown - position).normalized; 
            if (Vector3.Distance(playerLastKnown, position) <= attackDistance)
            {
                if (Vector3.Distance(playerLastKnown, position) <= fleeDistance)
                {
                    lastInput = new Vector2(playerDir.x, playerDir.z).normalized;
                    movement.input_direction = -lastInput;
                }
                else
                {
                    lastInput = new Vector2(playerDir.x, playerDir.z).normalized;
                    movement.input_direction = lastInput * .0001f;
                }
                character.Attack();
            }
            else
            {
                movement.input_direction = lastInput = new Vector2(playerDir.x, playerDir.z).normalized;
            }
            movement.walkSpeedMultiplier = 1f;
        }
        /// <summary> The AI has taken damage. Assumes damage came from player and looks in their direction. </summary>
        public virtual void OnDamaged()
        {
            state = State.Chase;
            Vector3 playerDir = (playerLastKnown - position).normalized;
            lastInput = new Vector2(playerDir.x, playerDir.z).normalized;
        }
        #endregion

        #region IPauseable
        /// <summary> IPauseable.IsPaused </summary>
        public bool IsPaused { get; set; }
        /// <summary> IPauseable.Pause() </summary>
        public void Pause()
        {
            IsPaused = true;
        }
        /// <summary> IPauseable.Play() </summary>
        public void Play()
        {
            IsPaused = false;
        }
        /// <summary> IPauseable.SubscribeToGameManager() </summary>
        public void SubscribeToGameManager()
        {
            if (!GameManager.instance.pausables.Contains(this))
            {
                GameManager.instance.pausables.Add(this);
            }
        }
        #endregion

        #region UnityCallbacks
        private void Start()
        {
            SubscribeToGameManager(); // subscribe to event: pause
            if (movement == null) { movement = GetComponent<Movement>(); }
            if (character == null) { character = GetComponent<Character>(); }

            targetLocation = patrolArea.GetRandomInArea();
            character.onDamage += (int x) => { OnDamaged(); }; // subscribe to event: when the player gets damaged
        }
        private void Update()
        {
            Statemachine();
        }
        private void OnDrawGizmos()
        {
            // patrol area
            Gizmos.color = Color.green;
            if (patrolArea.useCircle)
            {
                Gizmos.DrawWireSphere(patrolArea.center, patrolArea.radius);
            }
            else
            {
                Gizmos.DrawWireCube(patrolArea.center, new Vector3(patrolArea.rect.x, 1, patrolArea.rect.y));
            }

            // sight
            if (character == null) { Debug.LogWarning($"Character == null"); }
            else
            {
                Gizmos.matrix = character.animatedChild.transform.localToWorldMatrix;
                Gizmos.color = Color.red;
                Gizmos.DrawFrustum(sight.headPosition, sight.fov * .5f, sight.distance, .001f, 2);
            }
        }
        #endregion
    }
}
