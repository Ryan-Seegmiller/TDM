using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWithRuneSystem : MonoBehaviour
{
    void Start()
    {
        if (RuneManager.instance.runesEnabled) { gameObject.SetActive(true); }
        else { gameObject.SetActive(false); }
    }
}
