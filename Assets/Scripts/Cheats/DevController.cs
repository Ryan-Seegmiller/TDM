
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A way to add cheats to the game
/// </summary>
public class DevController : MonoBehaviour
{
    #region Singleton
    public static DevController instace;
    public void Start()
    {
        if (instace == null)
        {
            instace = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetupCommands();

    }
    #endregion

    #region Decleartions
    /// <summary>Can command console show</summary>
    [HideInInspector] public bool showConsole;
    /// <summary>Can help console show</summary>
    [HideInInspector] public bool showHelp;
    /// <summary>Can the items show</summary>
    [HideInInspector] public bool showItems;
    /// <summary>Input from the player</summary>
    [HideInInspector] public string input = string.Empty;
    /// <summary>A list of command data that stores the cheat data</summary>
    public List<CommandData> commands = new List<CommandData>();
    /// <summary>A list of pervious inputs</summary>
    private List<string> previousInput = new List<string>();
    /// <summary>List of dev commands that stores the cheats</summary>
    [NonSerialized] List<DevCommandBase> devCommands = new List<DevCommandBase>();
    ///<summary>The position of the scroll</summary>
    private Vector2 scroll;
    #endregion

    #region Setup
    /// <summary>
    /// Initializes all the commands
    /// </summary>
    public void SetupCommands()
    {
        foreach (CommandData data in commands)
        {
            devCommands.Add(new DevCommand(data.commndID, data.commandDecription, data.commandFormat, () => { data.command.Invoke(); }));
        }
        DevCommand<Vector3> movePlayerLocation = new DevCommand<Vector3>
        (
            "MovePlayer",
            "Teleports the player to the location specified",
            "MovePlayer (#,#,#)",
            (Vector3) => { GameManager.instance.player.transform.position = Vector3; }
        );
        devCommands.Add(movePlayerLocation);
        DevCommand<int, Vector3> moveEnemy = new DevCommand<int, Vector3>
        (
            "MoveEnemy",
            "Telports the enmey using the enmey id to a specific location",
            "MoveEnemy # (#,#,#)",
            (ID, position) => { GameManager.instance.MoveEnemey(ID, position); }
        );
        devCommands.Add(moveEnemy);
        DevCommand<int> killEnemy = new DevCommand<int>
        (
            "KillEnemy",
            "Kills an enemy based on ID",
            "KillEnemy #",
            (ID) => { GameManager.instance.KillEnemy(ID); }
        );
        devCommands.Add(killEnemy);
        DevCommand findIDs = new DevCommand
        (
            "FindNearbyEnemies",
            "Finds all nearby IDs",
            "FindNearbyEnemies",
            () => {/*TODO: Add method that finds all nearby enemies and displays their ids*/ }

        );
        devCommands.Add(findIDs);
        DevCommand killALl = new DevCommand
        (
            "KillAll",
            "Kills all enemies",
            "KillAll",
            () => { GameManager.instance.KillAllEnemies(); }
        );
        devCommands.Add(killALl);
        DevCommand toggleIDS = new DevCommand
        (
            "ToggleID",
            "Toggles IDs above the charecter",
            "ToggleID",
            () => { GameManager.instance.ToggleID(); }
        );
        devCommands.Add(toggleIDS);
        DevCommand<string, int> addItem = new DevCommand<string, int>
        (
            "AddItem",
            "Add an item with an amount",
            "AddItem name #",
            (name, amount) => { GameManager.instance.AddItem(name, amount); }
        );
        devCommands.Add(addItem);
        DevCommand<string> removeItem = new DevCommand<string>
        (
            "RemoveItem",
            "Removes all items of that item",
            "RemoveItem name",
            (name) => { GameManager.instance.RemoveItem(name); }
        );
        devCommands.Add(removeItem);
        DevCommand showItems = new DevCommand
        (
            "DisplayItemNames",
            "Displays all the item names",
            "DisplayItemNames",
            () => { ToggleItemsDisplay(); }
        );
        devCommands.Add(showItems);
        DevCommand godMode = new DevCommand
        (
            "GodMode",
            "Makes you invincible",
            "GodMode",
            () => { GameManager.instance.GodMode(); }
        );
        DevCommand nextLevel = new DevCommand
        (
            "NextLevel",
            "Triggers a door to load the next level",
            "NextLevel",
            () => { FindObjectOfType<Door>().OpenDoor(); }
        );
        devCommands.Add(nextLevel);
        DevCommand disco = new DevCommand
        (
            "Disco",
            "Woo! Party!!!",
            "Disco",
            () => { GameManager.instance.Disco(); }
        );
        devCommands.Add(disco);
        for (int i = 0; i < devCommands.Count; i++)
        {
            devCommands[i].commandID = devCommands[i].commandID.ToLower();
        }
    }
    #endregion

    #region MonoBehaveiour
    private void OnGUI()
    {
        if (!showConsole) { return; }

        float y = 0f;

        if (showHelp)
        {

            ShowHelp(ref y);
        }
        if (showItems)
        {
            ShowItems(ref y);
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);

        GUI.SetNextControlName("ConsoleCommandMenu");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
        GUI.FocusControl("ConsoleCommandMenu");
    }
    #endregion

    #region Input
    /// <summary>
    /// Takes in the input from the keyboard and checks against the commands
    /// </summary>
    public void HandleInput()
    {
        input = input.ToLower();
        string[] properties = input.Split(" ");

        for (int i = 0; i < devCommands.Count; i++)
        {
            DevCommandBase command = devCommands[i];
            
            if (!input.Contains(command.commandID)) { continue; }
            if(i < commands.Count)
            {
                if (HasType(commands[i].command))
                {
                    Type type = FindType(commands[i].command);
                    
                    var input = Convert.ChangeType(properties[1], type);

                    ChangeListener(commands[i].command, input);
                    if(command is DevCommand)
                    {
                        (command as DevCommand).Invoke();
                    }
                    
                }
            }
            if(properties.Length > 1)
            {
                int ID = 0;
                Vector3? locationToMoveTo = ConvertToVector3(properties[1]);
                if(locationToMoveTo != null)
                {
                    print(locationToMoveTo);
                    (command as DevCommand<Vector3>).Invoke((Vector3)locationToMoveTo);
                }
                
                else if(properties.Length > 2)
                {
                    locationToMoveTo = ConvertToVector3(properties[2]);
                    //Behaviour of a int and a vector
                    if(locationToMoveTo != null && int.TryParse(properties[1], out ID))
                    {
                        (command as DevCommand<int, Vector3>).Invoke(ID,(Vector3)(locationToMoveTo));
                    }
                    //Behavious of a string and a int
                    else if (int.TryParse(properties[2], out ID))
                    {
                        (command as DevCommand<string, int>).Invoke(properties[1], ID);
                    }
                }
                //Behaviou of an int
                else if (int.TryParse(properties[1], out ID))
                {
                    (command as DevCommand<int>).Invoke(ID);
                }
                //behaviour of a string
                else if(command is DevCommand<string>)
                {
                    (command as DevCommand<string>).Invoke(properties[1]);
                }
            }
            //Bahaviour of nothing
            else
            {
                (command as DevCommand).Invoke();
            }

            
        }
        previousInput.Add(input);
        input = "";
    }
    /// <summary>
    /// Converts a string to a vector 2
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public Vector3? ConvertToVector3(string property)
    {
        float[] vector3 = new float[3];
        string[] vector = property.Split(new string[] { "(", ",", ")"}, StringSplitOptions.RemoveEmptyEntries);
        if(vector.Length != 3) { return null; }
        for (int i = 0; i < vector.Length; i++)
        {
            float number;
            if (float.TryParse(vector[i], out number))
            {
                vector3[i] = number;
            }
            else
            {
                return null;
            }
        }
        Vector3 vectorToReturn = new Vector3(vector3[0], vector3[1], vector3[2]);

        return vectorToReturn;
    }
    #endregion

    #region Reflection
    /// <summary>
    /// Changes the listener on the unity event to use the inputed value
    /// </summary>
    /// <param name="eventToChange"></param>
    /// <param name="paramater"></param>
    private void ChangeListener(UnityEvent eventToChange, object paramater)
    {
        for (int i = 0; i < eventToChange.GetPersistentEventCount(); i++)
        {
            string methodName = eventToChange.GetPersistentMethodName(0);
            Type objectType = eventToChange.GetPersistentTarget(0).GetType();
            MethodInfo[] methods = objectType.GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.Name == methodName)
                {
                    var newInstance = Convert.ChangeType(eventToChange.GetPersistentTarget(i), objectType);
                    eventToChange.SetPersistentListenerState(0,UnityEventCallState.Off);
                    eventToChange.AddListener(delegate { 
                        objectType.GetMethod(methodName).Invoke(newInstance, new[] { paramater });
                        eventToChange.RemoveAllListeners();
                    });
                }
            }
        }
    }
    /// <summary>
    /// Find the type of the class inserted into the unity event
    /// </summary>
    /// <param name="eventToPullType"></param>
    /// <returns></returns>
    public Type FindType(UnityEvent eventToPullType)
    {
        Type typeFound = null;

        for (int i = 0; i < eventToPullType.GetPersistentEventCount(); i++)
        {
            string methodName = eventToPullType.GetPersistentMethodName(i);
            Type objectType = eventToPullType.GetPersistentTarget(i).GetType();
            MethodInfo[] methods = objectType.GetMethods();
            foreach(MethodInfo method in methods)
            {
                if(method.Name == methodName)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    typeFound = parameters[0].ParameterType;
                    return typeFound;
                }
            }
        }
        return typeFound;
    }
    /// <summary>
    /// checks if the unity event has a method
    /// </summary>
    /// <param name="eventToPullType"></param>
    /// <returns></returns>
    public bool HasType(UnityEvent eventToPullType)
    {
        string methodName = eventToPullType.GetPersistentMethodName(0);
        Type objectType = eventToPullType.GetPersistentTarget(0).GetType();
        MethodInfo[] methods = objectType.GetMethods();
        foreach (MethodInfo method in methods)
        {
            if (method.Name == methodName)
            {
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length > 0;
            }
        }
        
        return false;
    }
    #endregion

    #region Toggles
    /// <summary>
    /// Turns on and off the help display
    /// </summary>
    public void ToggleHelpDisplay()
    {
        if (showItems) { showItems = !showItems; }
        showHelp = !showHelp;
    }

    /// <summary>
    /// Turns on and off the items display
    /// </summary>
    public void ToggleItemsDisplay()
    {
        if (showHelp) { showHelp = !showHelp; }
        showItems = !showItems;
    }
    #endregion

    #region Display
    /// <summary>
    /// Displays the item names as they are shown
    /// </summary>
    /// <param name="y">the height to display</param>
    void ShowItems(ref float y)
    {
        GUI.Box(new Rect(0, y, Screen.width, 100), "");
        Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * (HUDManager.instance.inventoryUI.itemDataReference.itemsData.Length));

        scroll = GUI.BeginScrollView(new Rect(0, y, Screen.width, 90), scroll, viewport);

        for(int i = 0; i < HUDManager.instance.inventoryUI.itemDataReference.itemsData.Length; i++)
        {
            string label = $"{HUDManager.instance.inventoryUI.itemDataReference.itemsData[i].itemName}";
            Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
            GUI.Label(labelRect, label);
        }
       
        GUI.EndScrollView();
        y += 100;
    }
    /// <summary>
    /// Shows the dev commands as they are strped
    /// </summary>
    /// <param name="y">the height to display</param>
    void ShowHelp(ref float y)
    {
        //draw a box
        GUI.Box(new Rect(0, y, Screen.width, 100), "");
        Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * (devCommands.Count));

        scroll = GUI.BeginScrollView(new Rect(0, y, Screen.width, 90), scroll, viewport);

        for (int i = 0; i < devCommands.Count; i++)
        {
            string label = $"{devCommands[i].commandFormat} - {devCommands[i].commandDescription}";
            Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
            GUI.Label(labelRect, label);
        }
        GUI.EndScrollView();
        y += 100;
    }
    /// <summary>
    /// Sets the input to the pervious input
    /// </summary>
    public void SetInputPrevious()
    {
        if(previousInput.Count > 0)
        {
            input = previousInput[previousInput.Count - 1];
            previousInput.Remove(previousInput[previousInput.Count - 1]);
        }
    }
    #endregion
}
#region Command Data
/// <summary>
/// A collcetion of data that is used when storing specific cheats in the inspector
/// </summary>
[System.Serializable]
public struct CommandData
{
    /// <summary>Id of the command that is cross referenced</summary>
    public string commndID;
    /// <summary>Description of the command that is displayed when help is typed</summary>
    public string commandDecription;
    /// <summary>Format for the command that is also displayed</summary>
    public string commandFormat;
    /// <summary>UNity event that stores the behaviour of the cheat</summary>
    public UnityEvent command;
}
#endregion