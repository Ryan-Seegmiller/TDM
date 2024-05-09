using UnityEngine;

/// <summary> Used solely for passing the score to the end scene </summary>
public class ScoreStore : MonoBehaviour
{
    ///Singleton pattern
    public static ScoreStore instance { get; private set; }

    ///Store score to pass through scenes
    public static int score = 0;

    void Awake()
    {
       if(instance == null) { instance = this; }
       else { Destroy(gameObject); }
       DontDestroyOnLoad(gameObject);
    }
}
