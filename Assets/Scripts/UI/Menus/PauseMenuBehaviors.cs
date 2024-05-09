using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehaviors : MonoBehaviour
{
    bool pauseActive = false;
    private void Start()
    {
        Resume();    
    }

    public void MainMenu()
    {
        GameManager.instance.gameState = GameState.Play;
        CameraController.instance.LockMouse(false);
        Destroy(CharacterSelectManager.instance?.gameObject);
        SceneManager.LoadScene(0);
    }
    public void Resume()
    {
        gameObject.SetActive(false);
    }
    public void TogglePause()
    {
        pauseActive = !pauseActive;
        gameObject.SetActive(pauseActive);
    }
}
