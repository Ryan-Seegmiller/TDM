using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Takes in mobile input and converets it to world space
/// </summary>
public class MobileControlManager : MonoBehaviour
{
    #region Declerations
    /// <summary>Singleton</summary>
    public static MobileControlManager instance;
    /// <summary>List of moibile controls on the screen</summary>
    public List<MoblieControl> mobileControls = new List<MoblieControl>();
    /// <summary>Stores the inventpry UI</summary>
    public InventoryUI inventoryUI;
    /// <summary>array of controls to diable on pause</summary>
    public MoblieControl[] controlsToDisable;

    public MoblieControl shopButton;
    
    /// <summary>
    /// Stores all the touch points depending on when they were touched and where the touch is
    /// </summary>
    Dictionary<int, KeyValuePair<bool, Vector2>> touchPoints = new Dictionary<int, KeyValuePair<bool, Vector2>>();
    #endregion

    #region MonoBehaviour
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
    private void Start()
    {
        SetupInventorySlots();
    }
    #endregion

    #region Setup
    /// <summary>
    /// Set the inventory buttons to the mobile controls
    /// </summary>
    private void SetupInventorySlots()
    {
        InventorySlot[] slots = inventoryUI?.inventorySlots;
        for (int i = 0; i < slots.Length; i++)
        {
            mobileControls.Add(slots[i].GetComponent<MoblieControl>());
        }
    }
    /// <summary>
    /// toggles the contros on and off
    /// </summary>
    public void ToggleControls()
    {
        foreach(MoblieControl control in controlsToDisable)
        {
            control.Toggle();
        }
    }
    #endregion

    #region TapBehaviours
    /// <summary>
    /// Cycles through which input is pressed when
    /// </summary>
    void CycleThrough()
    {
        foreach(MoblieControl mobileControl in mobileControls)
        {
            if (mobileControl.touched)
            {   //Checks if the touch is no longer being touched
                if (!touchPoints[mobileControl.index].Key)
                {
                    mobileControl.touched = false;
                    mobileControl.index = -1;
                    mobileControl.Released();
                }
                //If its still active dont worry about it
                else
                {
                    continue;
                }
            }
            if (!mobileControl.active) { continue; }
            foreach (KeyValuePair<int, KeyValuePair<bool, Vector2>> touchPoint in touchPoints)
            {
                if (touchPoint.Value.Key)
                {
                    if (!mobileControl.OutOfBounds(touchPoint.Value.Value) && mobileControl.gameObject.activeInHierarchy)
                    {
                        mobileControl.touched = true;
                        mobileControl.index = touchPoint.Key;
                        mobileControl.initialTouchPostion = touchPoint.Value.Value;
                        mobileControl.Clicked();
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Sets the position of where thg input was pressed
    /// </summary>
    /// <param name="index"></param>
    void SetPostion(int index)
    {
        foreach(MoblieControl mobileControl in mobileControls)
        {
            if (!mobileControl.touched) { continue; }
           
            if(mobileControl.index == index)
            {
                if (mobileControl is CustomMobileButton)
                {
                    CustomMobileButton mobileButton = ((CustomMobileButton)mobileControl);
                    if (mobileButton.OutOfBounds(touchPoints[index].Value))
                    {
                        ForceQuitInput(index);
                        return;
                    }
                }
                mobileControl.touchPostion = touchPoints[index].Value;
            }
        }
    }
    
    /// <summary>
    /// Force quit input
    /// </summary>
    /// <param name="index"></param>
    public void ForceQuitInput(int index)
    {
        touchPoints[index] = new KeyValuePair<bool, Vector2>(false, touchPoints[index].Value);
        CycleThrough();
    }
    #endregion

    #region Position
    public void OnTouchPoint(InputValue value)
    {
        Held(0, value.Get<Vector2>());
    }
    public void OnTouchPoint2(InputValue value)
    {
        Held(1, value.Get<Vector2>());
    }
    public void OnTouchPoint3(InputValue value)
    {
        Held(2, value.Get<Vector2>());
    } 
    public void OnTouchPoint4(InputValue value)
    {
        Held(3, value.Get<Vector2>());
    } 
    public void OnTouchPoint5(InputValue value)
    {
        Held(4, value.Get<Vector2>());
    }
    #endregion

    #region Tap
    public void OnTap1(InputValue value)
    {
        Tapped(0, value.isPressed);
    }
    public void OnTap2(InputValue value)
    {
        Tapped(1, value.isPressed);
    }
    public void OnTap3(InputValue value)
    {
        Tapped(2, value.isPressed);
    }
    public void OnTap4(InputValue value)
    {
        Tapped(3, value.isPressed);
    }
    public void OnTap5(InputValue value)
    {
        Tapped(4, value.isPressed);
    }
    #endregion

    #region Tap position
    /// <summary>
    /// Sets the inital press
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pressed"></param>
    void Tapped(int index, bool pressed)
    {
        if (touchPoints.ContainsKey(index))
        {
            touchPoints[index] = new KeyValuePair<bool, Vector2>(pressed, touchPoints[index].Value);
        }
        else
        {
            touchPoints[index] = new KeyValuePair<bool, Vector2>(pressed, new Vector2(10000,100000));
        }
        CycleThrough();
    }
    /// <summary>
    /// Sets the position to the index of touch points
    /// </summary>
    /// <param name="index"></param>
    /// <param name="position"></param>
    void Held(int index, Vector2 position)
    {
        if (touchPoints.ContainsKey(index))
        {
            bool isDown = touchPoints[index].Key;
            touchPoints[index] = new KeyValuePair<bool, Vector2>(isDown,position);
        }
        SetPostion(index);
    }
    #endregion
}
