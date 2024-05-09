using FloatingText;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AI;
using LevelGeneration;

/// <summary>
/// Game dimensions
/// </summary>
public enum GameDimension { game2D, game25D, game3D }

/// <summary>
/// Controls all the fun things in the game
/// </summary>
public class GameManager
{
    #region Decleartioins
    #region Events
    /// <summary>Game Start Delegate</summary>
    public delegate void GameStart();
    /// <summary>Game Start Delegate </summary>
    public GameStart OnGameStart;
    /// <summary>Game end delegate</summary>
    public delegate void GameEnd(); // TODO : parse needed game data
    /// <summary>Game end delegate</summary>
    public GameEnd OnGameEnd;
    /// <summary>Game pause delegate</summary>
    public delegate void GamePause();
    /// <summary>Game play delegate</summary>
    public GamePause OnPlay;
    /// <summary>Game pause delegate</summary>
    public GamePause OnPause;
    #endregion

    #region Singleton
    /// <summary>Singleton</summary>
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    #endregion

    /// <summary>Inventory reference</summary>
    public Inventory inventory;
    /// <summary>Cheats reference</summary>
    public DevController cheats;
    /// <summary>Player reference</summary>
    public Character player;

    /// <summary>The players score count</summary>
    public GameObject disco = null;
    
    
    public int score;
    /// <summary>The players total gold</summary>
    public int goldAmount;
    /// <summary>The multiplier of the score for the player</summary>
    public float scoreMultiplier = 1.2f;

    /// <summary>Unviersal id counter</summary>
    public static int IDCounter = 0;
    /// <summary>Can show labels</summary>
    public static bool showLabels = false;
    /// <summary>Determines the game dimeansion of the game</summary>
    public GameDimension gameDimension
    {
        get { return _gameDimension; }
        set { _gameDimension = value;
            AudioManager.SetEmitters3D((_gameDimension == GameDimension.game3D)?(true) : (false)); }
    } 
    private GameDimension _gameDimension = GameDimension.game3D;

    /// <summary>The Hud reference</summary>
    public HUDManager hud => HUDManager.instance;
    /// <summary>Stores the main camera</summary>
    public Camera mainCamera => Camera.main;
    /// <summary>Holds the list of all the enemies</summary>
    List<Character> enemies = new List<Character>();
    /// <summary>The entity</summary>
    const string entityID = "Entity";

    #region Prefab paths

    #region Player
    /// <summary>Player prefab path in rescoruces for fighter</summary>
    GameObject prefab_player3D_fighter = Resources.Load<GameObject>("CharacterPrefabs/3D/Player3D-Fighter");
    /// <summary>Player prefab path in rescoruces for archer</summary>
    GameObject prefab_player3D_archer = Resources.Load<GameObject>("CharacterPrefabs/3D/Player3D-Archer");
    /// <summary>Player prefab path in rescoruces for mage</summary>
    GameObject prefab_player3D_mage = Resources.Load<GameObject>("CharacterPrefabs/3D/Player3D-Mage");
    /// <summary>Player prefab path in rescoruces for blob</summary>
    GameObject prefab_player3D_blob = Resources.Load<GameObject>("CharacterPrefabs/3D/Player3D-Blob");
    #endregion

    #region Enemy
    /// <summary>Enemey prefab path in rescoruces for fighter</summary>
    GameObject prefab_enemy3D_fighter = Resources.Load<GameObject>("CharacterPrefabs/3D/AI3D-Fighter");
    /// <summary>Enemey prefab path in rescoruces for archer</summary>
    GameObject prefab_enemy3D_archer = Resources.Load<GameObject>("CharacterPrefabs/3D/AI3D-Archer");
    /// <summary>Enemey prefab path in rescoruces for mage</summary>
    GameObject prefab_enemy3D_mage = Resources.Load<GameObject>("CharacterPrefabs/3D/AI3D-Mage");
    /// <summary>Enemey prefab path in rescoruces for blob</summary>
    GameObject prefab_enemy3D_blob = Resources.Load<GameObject>("CharacterPrefabs/3D/AI3D-Blob");
    #endregion

    GameObject prefab_disco = Resources.Load<GameObject>("Disco/DiscoFloor");

    #endregion

    #region GameState
    private GameState _gameState;
    /// <summary>Holds the previous state the game was in</summary>
    private GameState previousState;
    /// <summary>Holds which state the game is in</summary>
    public GameState gameState
    {
        get { return _gameState; }
        set => SetGameState(value);
    }
    /// <summary>
    /// Sets the game state through a switch statemant that calls differnet behaviours
    /// </summary>
    /// <param name="state"></param>
    void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Play:
                PlayGame();
                break;
            case GameState.Pause:
                PauseGame();
                break;
            case GameState.End:
                break;
            case GameState.Pre:
                break;
            default:
                break;
        }
    }
    /// <summary>Holds a reference to all the pausables</summary>
    public List<IPauseable> pausables = new List<IPauseable>();

    #endregion

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    private GameManager()
    {
        //Gets the objectpool manager in the scene
        //NOTE: This must be in the scene
        cheats = MonoBehaviour.FindObjectOfType<DevController>();            

        inventory = new Inventory(10);
        gameState = GameState.Play;

        enemies = new List<Character>();
    }
    #endregion
    #endregion

    #region Level
    /// <summary>
    /// Method that is called on level load
    /// </summary>
    /// <param name="dungeon"></param>
    public void OnLevelLoaded(Dungeon dungeon)
    {
        HUDManager.instance.progressionValues.SetLevelText(dungeon.level);
        player.Respawn();
        if (dungeon.level != 1)
        { 
            ChangeMultiplier(scoreMultiplier + .2f);
        }
        else
        {
            ChangeMultiplier(1);
        }

    }
    #endregion

    #region Game
    /// <summary>
    /// Called on game start
    /// </summary>
    private void StartGame()
    {
        OnGameStart?.Invoke();
    }
    /// <summary>
    /// Called on game end
    /// </summary>
    public void EndGame()
    {
        OnGameEnd?.Invoke();
        ScoreStore.score = score;
        SceneManager.LoadScene(3);
    }
    #endregion

    #region Pause
    /// <summary>
    /// Pauses all the iPausable game objects
    /// </summary>
    private void PauseGame()
    {
        foreach (IPauseable go in pausables)
        {
            if(go == null) { continue; }
            go.Pause();
        }
        _gameState = GameState.Pause;
        OnPause?.Invoke();
    }
    /// <summary>
    /// Remove an object from iPausables
    /// </summary>
    /// <param name="pausable"></param>
    public void RemoveFromPausables(IPauseable pausable)
    {
        pausables.Remove(pausable);
    }
    /// <summary>
    /// Resumes the game
    /// </summary>
    private void PlayGame()
    {
        foreach (IPauseable go in pausables)
        {
            if (go == null) { continue; }
            go.Play();
        }
        _gameState = GameState.Play;
        OnPlay?.Invoke();
       
    }
    /// <summary>
    /// Toggle pause
    /// </summary>
    public void TogglePause()
    {
        if (gameState != GameState.Pause)
        {
            previousState = gameState;
            gameState = GameState.Pause;
        }
        else
        {
            gameState = previousState;
        }
        CameraController.instance.LockMouse(gameState != GameState.Pause);
    }
    #endregion

    #region Inventory
    /// <summary>
    /// Toggle the inventory 
    /// </summary>
    public void ToggleInvetory()
    {
        hud.inventoryUI.ToggleInvetory();
    }
    #endregion

    #region Progression values
    /// <summary>
    /// Adds score to the player score
    /// </summary>
    /// <param name="scoreToAdd"></param>
    /// <param name="positionToStart"></param>
    public void AddScore(int scoreToAdd, Vector3 positionToStart)
    {
        score += (int)(scoreToAdd);
        FloatingTextManager.instance.SetStaticMovementFloatingText(GetScoreValues(scoreToAdd), HUDManager.instance.progressionValues.scoreText.rectTransform, positionToStart, () => 
        {
            HUDManager.instance.progressionValues.SetScoreText(score);
        } );
    }
   /// <summary>
   /// Subtract score 
   /// </summary>
   /// <param name="scoreToTakeAway"></param>
    public void SubtractScore(int scoreToTakeAway)
    {
        score -= (int)(scoreToTakeAway);
    }
    /// <summary>
    /// Adds gold to the players gold
    /// </summary>
    /// <param name="goldToAdd"></param>
    public void AddGold(int goldToAdd)
    {
        goldAmount += goldToAdd;
        HUDManager.instance.progressionValues.SetGoldText(goldAmount);
    }
    /// <summary>
    /// Removes gold from the player gold
    /// </summary>
    /// <param name="goldToRemove"></param>
    public void RemoveGold(int goldToRemove)
    {
        goldAmount -= goldToRemove;
        HUDManager.instance.progressionValues.SetGoldText(goldAmount);
    }
    /// <summary>
    /// Changes the score mulitplier
    /// </summary>
    /// <param name="multiplier"></param>
    public void ChangeMultiplier(float multiplier)
    {
        scoreMultiplier = multiplier;
        HUDManager.instance.progressionValues.SetMultplierText(scoreMultiplier);
    }
    #endregion

    #region Characters (Player & Enemies)
    /// <summary>
    /// Returns the charcter ID
    /// </summary>
    /// <returns></returns>
    public static string SetID()
    {
        IDCounter++;
        return $"{entityID}{IDCounter}";
    }
    /// <summary>
    /// Gets the enemey
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Character GetEnemy(int id)
    {
        foreach(Character enemy in enemies)
        {
            if (enemy.characterID == entityID + id)
            {
                return enemy;
            }
        }
        return null;
    }
    /// <summary>
    /// Called when enemey is dead
    /// </summary>
    /// <param name="enemy"></param>
    public void OnEnemyDied(Character enemy)
    {
        bool oneIsAlive = false;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].isDead) { oneIsAlive = true; break; }
        }
        if (!oneIsAlive)
        {
            //EndGame();
        }
    }

    /// <summary>
    /// Spawns a charcter in the world
    /// </summary>
    /// <param name="position"></param>
    /// <param name="characterType"></param>
    /// <param name="isPlayer"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    public GameObject SpawnCharacter(Vector3 position, CharacterType characterType, bool isPlayer, AIBrain.PatrolArea area = new AIBrain.PatrolArea())
    {
        if (gameDimension == GameDimension.game2D)
        { position.z = 0; }
        else
        { position.y = 0; }
        GameObject newCharacter = null;
        //Create the appropriate prefab
        //Player
        if (isPlayer)
        {
            //3D
            switch (characterType)
            {
                case CharacterType.fighter:
                    newCharacter = Object.Instantiate(prefab_player3D_fighter, position, Quaternion.identity);
                    break;
                case CharacterType.archer:
                    newCharacter = Object.Instantiate(prefab_player3D_archer, position, Quaternion.identity);
                    break;
                case CharacterType.mage:
                    newCharacter = Object.Instantiate(prefab_player3D_mage, position, Quaternion.identity);
                    ControlPromptsManager.instance.ActivateGroup("Mage");
                    break;
                case CharacterType.blob:
                    newCharacter = Object.Instantiate(prefab_player3D_blob, position, Quaternion.identity);
                    break;
            }
        }
        //AI
        else
        {
            switch (characterType)
            {
                case CharacterType.fighter:
                    newCharacter = Object.Instantiate(prefab_enemy3D_fighter, position, Quaternion.identity);
                    break;
                case CharacterType.archer:
                    newCharacter = Object.Instantiate(prefab_enemy3D_archer, position, Quaternion.identity);
                    break;
                case CharacterType.mage:
                    newCharacter = Object.Instantiate(prefab_enemy3D_mage, position, Quaternion.identity);
                    break;
                case CharacterType.blob:
                    newCharacter = Object.Instantiate(prefab_enemy3D_blob, position, Quaternion.identity);
                    break;
            }
        }

        Character character = newCharacter.GetComponent<Character>();
        //Assign to delegate
        character.onDamage += (int dmg) =>
        {
            FloatingTextValues damagedTextValues = GetDamageText(dmg);
            FloatingTextManager.instance.SetStationaryFloatingText(damagedTextValues, character.transform.position);
        };

        //Assign as player
        if (isPlayer)
        {
            character.characterID = "Player";
            character.isPlayer = true;

            if(Minimap.instance!=null)
                Minimap.instance.player = character.transform;
        }
        //Assign as enemy
        else
        {
            // register
            enemies.Add(character);
            // set ID
            character.characterID = SetID();
            // AI config
            AIBrain brain = newCharacter.GetComponent<AIBrain>();
            Movement movement = newCharacter.GetComponent<Movement>();
            brain.patrolArea = area;
        }

        return newCharacter;
    }
    /// <summary>
    /// Spawns a charcter in the world
    /// </summary>
    /// <param name="position"></param>
    /// <param name="characterType"></param>
    /// <param name="isPlayer"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    public GameObject SpawnCharacter(Vector3 position, int characterType, bool isPlayer, AIBrain.PatrolArea area) => SpawnCharacter(position, (CharacterType)characterType, isPlayer, area);
    #endregion
    /// <summary>
    /// Get damage the text for damage depending on the damage
    /// </summary>
    /// <param name="dmg"></param>
    /// <returns></returns>
    private FloatingTextValues GetDamageText(int dmg)
    {
        string damagedText = FloatingTextManager.instance.damagedText.text + $" {dmg}";
        FloatingTextValues damagedTextValues = new FloatingTextValues();
        damagedTextValues.text = damagedText;
        damagedTextValues.textColor = FloatingTextManager.instance.damagedText.textColor;
        damagedTextValues.duration = FloatingTextManager.instance.damagedText.duration;
        damagedTextValues.speed = FloatingTextManager.instance.damagedText.speed;
        damagedTextValues.offset = FloatingTextManager.instance.damagedText.offset;
        damagedTextValues.direction = FloatingTextManager.instance.damagedText.direction;
        return damagedTextValues;
    }
    /// <summary>
    /// Gets the text for the score depending on the score
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    private FloatingTextValues GetScoreValues(int score)
    {
        string damagedText = FloatingTextManager.instance.scoreText.text + $" {score}";
        FloatingTextValues scoreTextValues = new FloatingTextValues();
        scoreTextValues.text = damagedText;
        scoreTextValues.textColor = FloatingTextManager.instance.scoreText.textColor;
        scoreTextValues.duration = FloatingTextManager.instance.scoreText.duration;
        scoreTextValues.speed = FloatingTextManager.instance.scoreText.speed;
        scoreTextValues.offset = FloatingTextManager.instance.scoreText.offset;
        scoreTextValues.direction = FloatingTextManager.instance.scoreText.direction;
        scoreTextValues.hasStaticMovement = FloatingTextManager.instance.scoreText.hasStaticMovement;
        scoreTextValues.size = FloatingTextManager.instance.scoreText.size;
        scoreTextValues.momentaryHoverFloat = FloatingTextManager.instance.scoreText.momentaryHoverFloat;
        return scoreTextValues;
    }

    #region Cheats

    /// <summary>
    /// FInd the ids in the scene
    /// </summary>
    public void FindID()
    {

    }
    /// <summary>
    /// Toggles the ids
    /// </summary>
    public void ToggleID()
    {
        Debug.Log("TESTESTEST");
        showLabels = !showLabels;
    }
    /// <summary>
    /// Kills an enemey based on an ID
    /// </summary>
    /// <param name="id"></param>
    public void KillEnemy(int id)
    {
        Character enemy = GetEnemy(id);
        enemy?.Die();
    }
    /// <summary>
    /// Kills all the enemies
    /// </summary>
    public void KillAllEnemies()
    {
        foreach(Character character in enemies)
        {
            character.Die();
        }
    }
    /// <summary>
    /// Turns the charcter into god mode
    /// </summary>
    /// <param name="id"></param>
    public void GodMode(int id = -1)
    {
        if (id == -1 && player != null)
        {
            player.godMode = !player.godMode;
            return;
        }
        else if (id != -1)
        {
            Character enemy = GetEnemy(id);
            if (enemy != null)
            {
                enemy.godMode = !enemy.godMode;
            }
        }
    }
    /// <summary>
    /// Moves the enemey base on id to the specified location
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    public void MoveEnemey(int id, Vector3 position)
    {
        Character enemey = GetEnemy(id);
        if(enemey != null)
        {
            enemey.transform.position = position;
        }

    }
    /// <summary>
    /// Adds an item based on the item name and amount
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="amount"></param>
    public void AddItem(string itemName, int amount)
    {
        ItemData itemData = ItemPool.instance.itemReferences.GetItemData(itemName);
        if (itemData.itemObject.isInventory)
        {
            inventory.AddToInventory(itemName, amount);
        }
        else
        {
            itemData.itemObject.Use();
        }
    }
    /// <summary>
    /// Remove items an item using the name
    /// </summary>
    /// <param name="itemName"></param>
    public void RemoveItem(string itemName)
    {
        inventory.RemoveFromInvetory(itemName);
    }

    public void Disco()
    {
        if (player != null)
        {
             //Create a new disco if needed
            if (disco == null)
            {
                disco = Object.Instantiate(prefab_disco, player.transform.position, Quaternion.identity);
            }
            //Activate and set the position of the disco
            else
            {
                Object.Destroy(disco);
                disco = null;
            }
        }
    }


    #endregion

}
/// <summary>
/// The state of the game
/// </summary>
public enum GameState
{
    Play,
    Pause,
    End,
    Pre
}

