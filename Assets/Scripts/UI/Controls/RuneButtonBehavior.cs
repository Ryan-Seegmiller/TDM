using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneButtonBehavior : MonoBehaviour
{
    RectTransform myTransform;

    public Vector2 pos1;
    public Vector2 pos2;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<RectTransform>();
        Check();
    }

    public void Check()
    {
        if(GameManager.instance.player.characterType == CharacterType.mage)
        {
            gameObject.SetActive(true);
            if (RuneManager.instance.modRuneMenuOpen)
            {
                myTransform.anchoredPosition = pos2;
            }
            else { myTransform.anchoredPosition = pos1; }
        }
        else { gameObject.SetActive(false); }
    }
}
