using TMPro;
using UnityEngine;

/// <summary>
/// Display of the players hud
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    #region Declerations
    /// <summary>Reference for Canvas Group</summary>
    private CanvasGroup cvGroup;
    /// <summary>Is the activated </summary>
    private bool activated = true;
    /// <summary>The health bar</summary>
    public Bar healthBar;
    /// <summary>The ammo bar</summary>
    private Bar ammoBar;
    /// <summary>The mana bar</summary>
    public Bar manaBar;
    /// <summary>The sword bar</summary>
    public Bar sword;
    /// <summary>The reference quiver manager</summary>
    public QuiverManager quiver;
    /// <summary>The display or the score</summary>
    public TextMeshProUGUI score;
    #endregion

    #region Monobehaveiours
    private void Awake()
    {
        ShowNone(); //default
    }

    private void Start()
    {
        cvGroup = GetComponent<CanvasGroup>();
    }
    #endregion

    #region Toggles
    /// <summary>
    /// Toggles the the ui on and off
    /// </summary>
    public void ToggleUI()
    {
        activated = !activated;
        if (activated)
        {
            cvGroup.alpha = 1;
        }
        else
        {
            cvGroup.alpha = 0;
        }
    }
    #endregion

    #region Update Values
    /// <summary>
    /// Updates the mana bar diplay
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="flash"></param>
    public void ChangeMana(int currentValue, int maxValue, bool flash = true)
    {
        if(ammoBar != null) { ammoBar.ChangeValue(currentValue, maxValue, flash); }
    }
    /// <summary>
    /// Updates the health bar display
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    public void ChangeHealth(int currentValue, int maxValue)
    {
        healthBar.ChangeValue(currentValue, maxValue);
    }
    #endregion

    #region Ammo Bar
    /// <summary>
    /// Show the mana
    /// </summary>
    public void ShowMana()
    {
        manaBar.gameObject.SetActive(true);
        sword.gameObject.SetActive(false);
        quiver.gameObject.SetActive(false);
       
        ammoBar = manaBar;
    }
    /// <summary>
    /// Show sthe sword
    /// </summary>
    public void ShowSword()
    {
        manaBar.gameObject.SetActive(false);
        sword.gameObject.SetActive(true);
        quiver.gameObject.SetActive(false);

        ammoBar = sword;
    }
    /// <summary>
    /// Shows the quiver 
    /// </summary>
    public void ShowQuiver()
    {
        manaBar.gameObject.SetActive(false);
        sword.gameObject.SetActive(false);
        quiver.gameObject.SetActive(true);

        ammoBar = quiver;
    }
    /// <summary>
    /// Shows uhhhhhhh.. none...??
    /// </summary>
    public void ShowNone()
    {
        manaBar.gameObject.SetActive(false);
        sword.gameObject.SetActive(false);
        quiver.gameObject.SetActive(false);

        ammoBar = null;
    }
    #endregion

}
