using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#region SciptableObject
/// <summary>
/// Scriptable object that stores all the controls diplay
/// </summary>
[CreateAssetMenu(fileName = "ControlsDisplay", menuName = "GameData/Controls/Display")]
public class ControlsDisplayScriptableObject : ScriptableObject
{
    /// <summary>Base prefab for the sprite picker</summary>
    public AccessibilitySpritePicker basePrefab;
    /// <summary>Control propmt display array that stores all the data</summary>
    public ControlPromptDisplay[] display;
    /// <summary>Control propmt display array that stores all the data groupings</summary>
    public ControlPromptIDGroup[] groupings;

}
#endregion

#region Control Prompt Display
/// <summary>
/// Data storgae for the control prompt display
/// </summary>
[Serializable]
public struct ControlPromptDisplay
{
    /// <summary>ID for the control prompt</summary>
    public string ID;
    /// <summary>Text to be displayed</summary>
    public string prompt;
    /// <summary>Control sprite for mouse and keybaord</summary>
    public Sprite mouseAndKeyboardSprite;
    /// <summary>Control sprite for controller</summary>
    public Sprite controllerSprite;
    /// <summary>Wether or not it will be active off the bat</summary>
    public bool activeOnStart;

}
#endregion

#region Groupings
/// <summary>
/// Control Group holder
/// </summary>
[Serializable]
public struct ControlPromptIDGroup
{
    /// <summary>Id to reference the group by</summary>
    public string ID;
    /// <summary>A group of ids from the control prompt display to be used in the group</summary>
    public string[] IDs;
}
#endregion

#region Control ID
/// <summary>
/// A way to easily accses a control from the inspector
/// </summary>
[Serializable]
public class ControlID
{
    /// <summary>Boolo that selected wether to use grouping or not</summary>
    public bool useControls;
    /// <summary>Groupings id display</summary>
    public string groupingsID;
    /// <summary>Control id </summary>
    public string controlID;
    /// <summary>Gets the id from whichever is active</summary>
    public string ID
    {
        get { return (useControls) ? groupingsID : controlID; }
    }

    /// <summary>
    /// Activates the control
    /// </summary>
    public void Activate()
    {
        if (useControls)
        {
            ControlPromptsManager.instance.ActivateControl(controlID);
        }
        else
        {
            ControlPromptsManager.instance.ActivateGroup(groupingsID);
        }
    }
    /// <summary>
    /// Deactivates the group the control
    /// </summary>
    public void Deactivate()
    {
        if (useControls)
        {
            ControlPromptsManager.instance.DeactivateControl(controlID);
        }
        else
        {
            ControlPromptsManager.instance.DeactivateGroup(groupingsID);
        }
    }
}
#endregion

#region Editor
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ControlID))]
public class ControlIDEditor : PropertyDrawer
{
    #region Decleartions
    private readonly string[] popupOptions = { "Use All IDS", "Use Groupings" };

    public GUIStyle popupStyle;

    ControlsDisplayScriptableObject controlsDisplayRefernce;

    string[] IDs = null;
    string[] groupings = null;

    bool created = false;
    #endregion

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Setup();

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        SerializedProperty useControls = property.FindPropertyRelative("useControls");//Bool
        SerializedProperty groupings = property.FindPropertyRelative("groupingsID");//string
        SerializedProperty control = property.FindPropertyRelative("controlID");//string

        //Toggle rect
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        int popupResult = EditorGUI.Popup(buttonRect, useControls.boolValue ? 0 : 1, popupOptions, popupStyle);//popup using the toggle button
        useControls.boolValue = popupResult == 0;

        ///Shows the methods if they actually can and they are not null
        if(IDs != null || this.groupings != null)
        {   //Popup for the ids
            int idResult = EditorGUI.Popup(position, useControls.boolValue? GetID(control.stringValue) : GetGrouping(groupings.stringValue), useControls.boolValue ? IDs : this.groupings);
        
            if (useControls.boolValue)
            {   //Controls Set
                control.stringValue = IDs[idResult];
            }
            else
            {   //Groupings set
                if (idResult < this.groupings.Length)
                {
                    groupings.stringValue = this.groupings[idResult];
                }
            }
        }
        else
        {
            EditorGUI.PropertyField(position, useControls.boolValue ? control : groupings, GUIContent.none);
        }


        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

    }
    #region Setup
    /// <summary>
    /// Things that only need to be called once
    /// </summary>
    void Setup()
    {
        if (created) { return; }//Only calls once

        //Stes the popup style
        popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
        popupStyle.imagePosition = ImagePosition.ImageOnly;

        //Sets teh refernces 
        controlsDisplayRefernce = GetReferneces();

        //Set string values
        IDs = FindIDs();
        groupings = FindGroupings();

        created = true;
    }

    /// <summary>
    /// Gets the refernces from the scene
    /// </summary>
    /// <returns></returns>
    public ControlsDisplayScriptableObject GetReferneces()
    {
        return Component.FindObjectOfType<ControlPromptsManager>()?.controlsRefernces;
    }
    #endregion

    #region Controls
    /// <summary>
    /// Finds all the control IDs
    /// </summary>
    /// <returns></returns>
    public string[] FindIDs()
    {
        if(controlsDisplayRefernce == null) { return new string[0]; }
        List<string> IDs = new List<string>();

        for (int i = 0; i < controlsDisplayRefernce.display.Length; i++)
        {
            IDs.Add(controlsDisplayRefernce.display[i].ID);
        }
        return IDs.ToArray();
    }
    /// <summary>
    /// Gets the ID
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public int GetID(string ID)
    {
        if (IDs == null) { return 0; }
        for (int i = 0; i < IDs.Length; i++)
        {
            if (IDs[i] == ID)
            {
                return i;
            }
        }
        return 0;
    }
    #endregion

    #region Groupings
    public string[] FindGroupings()
    {
        if (controlsDisplayRefernce == null) { return new string[0]; }
        List<string> IDs = new List<string>();

        for (int i = 0; i < controlsDisplayRefernce.groupings.Length; i++)
        {
            IDs.Add(controlsDisplayRefernce.groupings[i].ID);
        }
        return IDs.ToArray();
    }
    public int GetGrouping(string ID)
    {
        if(groupings == null) { return 0; }
        for (int i = 0; i < groupings.Length; i++)
        {
            if (groupings[i] == ID)
            {
                return i;
            }
        }
        return 0;
    }
    #endregion
}
#endif
#endregion
