using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A Class that determines the sprite that will be on a object depnding on the platform
/// </summary>
public class AccessibilitySpritePicker : MonoBehaviour
{
    #region Declerations
    /// <summary>The text that will display </summary>
    public string prompt;
    /// <summary>The button sprite for mouse and keyboard </summary>
    public Sprite mouseAndKeyboardSprite;
    /// <summary>The button sprite for controller </summary>
    public Sprite contollerSprite;

    /// <summary>The image that holds the platform dependant sprite </summary>
    public Image promptImage;
    /// <summary>The text that will display the prompt </summary>
    public TextMeshProUGUI textMeshProUGUI;
    #endregion

    #region MopnoBehaveiour
    private void Start()
    {  
        textMeshProUGUI.text = prompt;
        StartCoroutine(LateStart());
    }
    #endregion

    #region OnInputChange
    /// <summary>
    /// Called whenver a input device is changed
    /// </summary>
    public void OnInputChange()
    {
        if (Application.isMobilePlatform) { gameObject.SetActive(false); return; }
        
        if (ControllerManager.instance.CONTROLLERENABLED)
        {
            promptImage.sprite = contollerSprite;
        }
        else
        {
            promptImage.sprite = mouseAndKeyboardSprite;
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Late start plays after 1 frame has concluded
    /// </summary>
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        OnInputChange();
    }
    #endregion
}
