using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{

    public void ReturnToMainMenu()
    {
        Destroy(CharacterSelectManager.instance?.gameObject);
        SceneManager.LoadScene(0);
    }
}
