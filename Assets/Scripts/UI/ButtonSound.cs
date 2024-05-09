using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { DoSound(); });
    }

    void DoSound()
    {
        AudioManager.PlaySound_UI_SFX(0);
    }
}
