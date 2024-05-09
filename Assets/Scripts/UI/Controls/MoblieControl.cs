using UnityEngine;

/// <summary>
/// Mobile control base class
/// </summary>
public class MoblieControl : MonoBehaviour
{
    #region Declerations
    /// <summary>the zone at which the button occupys</summary>
    protected RectTransform zone;

    /// <summary>Has the button been touched</summary>
    [HideInInspector]public bool touched;
    /// <summary>Index of the touch point</summary>
    [HideInInspector]public int index;
    /// <summary>Touch position</summary>
    [HideInInspector]public Vector2 touchPostion;
    /// <summary>Intila touch position</summary>
    [HideInInspector] public Vector2 initialTouchPostion;
    /// <summary>is the button active</summary>
    [HideInInspector] public bool active;
    #endregion

    #region Properties
    private CharacterInput _playerInput;
    /// <summary>
    /// Gets the player input component
    /// </summary>
    protected CharacterInput playerInput
    {
        get 
        { 
            if(_playerInput == null)
            {
                _playerInput = GameManager.instance.player?.GetComponent<CharacterInput>();
            }
            return _playerInput;
        }
    }
    #endregion

    #region Monobehaviours
    // Start is called before the first frame update
    protected virtual void Start()
    {
        zone = GetComponent<RectTransform>();
        
        active = true;
    }
    #endregion

    #region Virtual method
    /// <summary>
    /// Method that is called on click
    /// </summary>
    public virtual void Clicked()
    {
        //Implementation specific
    }
    /// <summary>
    /// Method that is called on release
    /// </summary>
    public virtual void Released()
    {
        //Implementation specific
    }
    #endregion

    #region Toggle
    /// <summary>
    /// Toggles the button
    /// </summary>
    public void Toggle()
    {
        active = !active;
        gameObject.SetActive(!gameObject.activeSelf);
    }
    #endregion

    #region Bounds
    /// <summary>
    /// Deterimes wether or not something is in bounds
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool OutOfBounds(Vector2 pos)
    {
        if(zone == null)
        {
            zone = GetComponent<RectTransform>();
        }
        Vector2 zonePos = (Vector2)zone.position;
        if (pos.x > zonePos.x + (zone.rect.width / 2) * zone.lossyScale.x) { return true; } //right
        else if (pos.x < zonePos.x - (zone.rect.width / 2) * zone.lossyScale.x) { return true; } //left
        else if (pos.y > zonePos.y + (zone.rect.height / 2) * zone.lossyScale.y) { return true; } //above
        else if (pos.y < zonePos.y - (zone.rect.height / 2) * zone.lossyScale.y) { return true; } //beneath

        return false;
    }
    #endregion
}
