using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control prompt manager that stores all the controls
/// </summary>
public class ControlPromptsManager : MonoBehaviour
{
    #region Singleton
    /// <summary>Singleton </summary>
    public static ControlPromptsManager _instance;
    public static ControlPromptsManager instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj = new GameObject("TEMP_ControlManger");
                obj.AddComponent<ControlPromptsManager>();
               
            }
            return _instance;
        }
        private set
        {
            if(_instance != null)
            {
                _instance = value;
            }
            else
            {
                Destroy(value.gameObject);
            }
        }
    }
    #endregion

    #region Declerations
    /// <summary>The refernce to the scriptable object that stores all the controlprompt data</summary>
    public ControlsDisplayScriptableObject controlsRefernces;
    /// <summary>A strogae container for the controls display</summary>
    public Dictionary<string, GameObject> controlDisplays = new Dictionary<string, GameObject>();
    /// <summary>Storage for all the data</summary>
    public Dictionary<string, ControlPromptIDGroup> groupings = new Dictionary<string, ControlPromptIDGroup>();
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetUp();
    }
    #endregion

    #region Setup
    /// <summary>
    /// setups all that is needed with the object
    /// </summary>
    private void SetUp()
    {
        if(controlsRefernces == null) { return; }
        RectTransform rectTR = GetComponent<RectTransform>();
        for (int i = 0; i < controlsRefernces.display.Length; i++)
        {   //Setup for each individual control prompt
            ControlPromptDisplay displayValues = controlsRefernces.display[i];
            AccessibilitySpritePicker controlDisplay = Instantiate(controlsRefernces.basePrefab, rectTR);
            controlDisplay.contollerSprite = displayValues.controllerSprite;
            controlDisplay.mouseAndKeyboardSprite = displayValues.mouseAndKeyboardSprite;
            controlDisplay.prompt = displayValues.prompt;
            controlDisplays.Add(displayValues.ID, controlDisplay.gameObject);//Adds to a dictionary for ease of use later
            if (!displayValues.activeOnStart)
            {
                DeactivateControl(displayValues.ID);
            }
        }

        for (int i = 0; i < controlsRefernces.groupings.Length; i++)
        {
            groupings.Add(controlsRefernces.groupings[i].ID, controlsRefernces.groupings[i]);
        }
    }
    #endregion

    #region Toggle controls
    /// <summary>
    /// Activates a control display based off of ID
    /// </summary>
    /// <param name="ID"></param>
    public void ActivateControl(string ID)
    {
        if (controlsRefernces == null) { return; }
        if (!controlDisplays.ContainsKey(ID))
        {
            Debug.LogWarning(ID + " does not exist in controls display");
            return;
        }
        controlDisplays[ID].SetActive(true);
    }
    /// <summary>
    /// Deactivates a control display based off of ID
    /// </summary>
    /// <param name="ID"></param>
    public void DeactivateControl(string ID)
    {
        if (controlsRefernces == null) { return; }
        if (!controlDisplays.ContainsKey(ID))
        {
            Debug.LogWarning(ID + " does not exist in controls display");
            return;
        }
        controlDisplays[ID].SetActive(false);
    }
    /// <summary>
    /// Activates a group using the group ID
    /// </summary>
    /// <param name="ID"></param>
    public void ActivateGroup(string ID)
    {
        if (controlsRefernces == null) { return; }
        if (!groupings.ContainsKey(ID))
        {
            Debug.LogWarning(ID + " does not exist in groupings");
            return;
        }
        ControlPromptIDGroup group = groupings[ID];
        for (int i = 0; i < group.IDs.Length; i++)
        {
            ActivateControl(group.IDs[i]);
        }
    }
    /// <summary>
    /// Decativates a group using the groupsd ID
    /// </summary>
    /// <param name="ID"></param>
    public void DeactivateGroup(string ID)
    {
        if(controlsRefernces == null) { return; }
        if (!groupings.ContainsKey(ID))
        {
            Debug.LogWarning(ID + " does not exist in groupings");
            return;
        }
        ControlPromptIDGroup group = groupings[ID];
        for (int i = 0; i < group.IDs.Length; i++)
        {
            DeactivateControl(group.IDs[i]);
        }
    }
    #endregion
}
