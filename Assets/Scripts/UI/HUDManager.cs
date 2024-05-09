using System;
using UnityEngine;

/// <summary>
/// A refernce point to all things related to the player hud
/// </summary>
public class HUDManager : MonoBehaviour
{
    #region Singleton
    ///<summary>Singleton</summary>
    public static HUDManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Declerations
    ///<summary>Mobile UI reference</summary>
    public UIElements mobileUI;
    ///<summary>PC UI reference</summary>
    public UIElements pcUI;

    ///<summary>Inventory UI reference</summary>
    public InventoryUI inventoryUI => (Application.isMobilePlatform) ? mobileUI.inventoryUI : pcUI.inventoryUI;
    ///<summary>PlayerHUD UI reference</summary>
    public PlayerHUD playerHUD => (Application.isMobilePlatform) ? mobileUI.playerHUD : pcUI.playerHUD;
    ///<summary>Selected Item UI reference</summary>
    public SelectedItemUI selectedUI => (Application.isMobilePlatform) ? mobileUI.selectedUI : pcUI.selectedUI;
    ///<summary>CharcterLabels reference</summary>
    public RectTransform characterLabels => (Application.isMobilePlatform) ? mobileUI.characterLabels : pcUI.characterLabels;
    ///<summary>Pause menu reference</summary>
    public PauseMenuBehaviors pauseMenu => (Application.isMobilePlatform) ? mobileUI.pauseMenu : pcUI.pauseMenu;
    ///<summary>Progression values reference</summary>
    public ProgressionValues progressionValues => (Application.isMobilePlatform) ? mobileUI.progressionValues : pcUI.progressionValues;


    [Header("Platform dependent")]
    ///<summary>Mobile layour reference</summary>
    public LayoutHelper mobileLayout;
    ///<summary>PC layout reference</summary>
    public LayoutHelper pcLayout;
    ///<summary>Control ID for the movement controls</summary>
    public ControlID movementControls;
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        UsePlatformUI();
    }
    #endregion

    #region Progression Values
    /// <summary>
    /// Changes the mana
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="flash"></param>
    public void ChangeMana(int currentValue, int maxValue, bool flash = true)
    {
        if (Application.isMobilePlatform)
        {
            mobileUI.playerHUD?.ChangeMana(currentValue, maxValue, flash);
        }
        else
        {
            pcUI.playerHUD?.ChangeMana(currentValue, maxValue, flash);
        }
    }
    /// <summary>
    /// Changes the health
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    public void ChangeHealth(int currentValue, int maxValue)
    {
        if (Application.isMobilePlatform)
        {
            mobileUI.playerHUD?.ChangeHealth(currentValue, maxValue);
        }
        else
        {
            pcUI.playerHUD?.ChangeHealth(currentValue, maxValue);
        }
    }
    #endregion

    #region Pause
    /// <summary>
    /// Pauses the game
    /// </summary>
    public void PauseGame()
    {
        GameManager.instance.TogglePause();
        ToggleLayoutUI();
        if (Application.isMobilePlatform)
        {
            if (mobileUI.inventoryUI.inventoryActive)
            {
                mobileUI.inventoryUI?.ToggleInvetory();
            }
            mobileUI.pauseMenu?.TogglePause();
            mobileUI.playerHUD?.ToggleUI();
            MobileControlManager.instance.ToggleControls();
        }
        else
        {
            if (pcUI.inventoryUI.inventoryActive)
            {
                pcUI.inventoryUI?.ToggleInvetory();
            }
            pcUI.pauseMenu?.TogglePause();
            pcUI.playerHUD?.ToggleUI();
            Canvas[] canvases = Component.FindObjectsOfType<Canvas>();
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].BroadcastMessage("OnInputChange", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    #endregion

    #region Toggle
    /// <summary>
    /// Toggle the layout on and off
    /// </summary>
    private void ToggleLayoutUI() 
    {
        if (Application.isMobilePlatform)
        {
            mobileLayout.Toggle();
        }
        else
        {
            pcLayout.Toggle();
            
        }
    }
    #endregion

    #region PlatformUI
    /// <summary>
    /// Sets the uI based on platform
    /// </summary>
    public void UsePlatformUI()
    {
        if (Application.isMobilePlatform)
        {
            mobileLayout?.gameObject.SetActive(true);
            pcLayout?.gameObject.SetActive(false);
        }
        else
        {
            pcLayout?.gameObject.SetActive(true);
            mobileLayout?.gameObject.SetActive(false);
        }
    }
    #endregion
}

#region UIELments
/// <summary>
/// A struct that stores refernces to all the data that is differnet on multiple platforms
/// </summary>
[Serializable]
public struct UIElements
{
    ///<summary>Refernce to the PlayerHud</summary>
    public PlayerHUD playerHUD;
    ///<summary>Refernce to the InventoryUI</summary>
    public InventoryUI inventoryUI;
    ///<summary>Refernce to the SelectedUI</summary>
    public SelectedItemUI selectedUI;
    ///<summary>Refernce to the ChartcerLabels</summary>
    public RectTransform characterLabels;
    ///<summary>Refernce to the PauseMenu</summary>
    public PauseMenuBehaviors pauseMenu;
    ///<summary>Refernce to the progression values</summary>
    public ProgressionValues progressionValues;
}
#endregion
