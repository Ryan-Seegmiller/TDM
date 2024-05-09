using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManagement : MonoBehaviour
{
    [SerializeField] GameObject StartNQuit;
    [SerializeField] GameObject FirstSelectOnMain;

    [SerializeField] GameObject Credits;
    [SerializeField] GameObject FirstSelectOnCredit;


    void Start()
    {
        DisplayMain();
        ParticleManager.isSetup = false;
    }

    public void DisplayMain()
    {
        StartNQuit.SetActive(true);
        Credits.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(FirstSelectOnMain);
    }

    public void DisplayCredits()
    {
        Credits.SetActive(true);
        StartNQuit.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(FirstSelectOnCredit);
    }

    public void NewGame()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SpriteArtistLink()
    {
        Application.OpenURL("https://opengameart.org/content/twelve-16x18-rpg-sprites-plus-base");
    }
    public void BgArtistLink()
    {
        Application.OpenURL("https://opengameart.org/content/pixel-art-backgrounds-0");
    }
    public void KennyLink()
    {
        Application.OpenURL("https://kenney.nl/assets/tiny-dungeon");
    }
    public void AudioLink()
    {
        Application.OpenURL("https://freesound.org/people/levelplane/sounds/412080/");
    }
}
