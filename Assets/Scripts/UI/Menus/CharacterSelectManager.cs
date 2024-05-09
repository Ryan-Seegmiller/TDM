using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager instance;
    public CharacterType selectedCharacter = CharacterType.fighter;//default fighter character
    int targetLevel = 2;//just 1 level for now
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    public void PickCharacter(int c)
    {
        selectedCharacter = (CharacterType)c;
        BeginGame();//called right after picking character, but can be seperate button
    }
    public void BeginGame()
    {
        SceneManager.LoadScene(targetLevel);
    }
    public void LoadCharacterSelectScreen()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
