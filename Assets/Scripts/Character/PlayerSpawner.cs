
using LevelGeneration;
using UnityEngine;


/// <summary> Spawns the player character in this object's position. </summary>
public class PlayerSpawner : MonoBehaviour
{
    /// <summary> The character type to spawn the player as. </summary>
    [SerializeField] CharacterType characterType;


    void Start()
    {
        //Set character type
        if (CharacterSelectManager.instance != null)
        { characterType = (CharacterType)CharacterSelectManager.instance.selectedCharacter; }
        //Spawn player and destroy self
        GameManager.instance.SpawnCharacter(WorldGenerator.instance.spawnPosition, characterType, true);
        AudioManager.ManualSetup();
        Destroy(gameObject);
    }
}
