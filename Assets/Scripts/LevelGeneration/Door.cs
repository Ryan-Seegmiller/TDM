using LevelGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Door interaction logic
/// </summary>
public class Door : MonoBehaviour
{
    public WorldGenerator WorldGenerator => WorldGenerator.instance;

    /// <summary>
    /// Load the next level.
    /// </summary>
    [ContextMenu("OpenDoor")]
    public void OpenDoor() 
    {
        GameManager.instance.AddScore(500, transform.position);
        WorldGenerator.LoadDungeon(WorldGenerator.instance.CurrentLevel + 1, GameManager.instance.OnLevelLoaded);
    }

}
