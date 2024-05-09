using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary> A character's input as provided by the player. </summary>
[RequireComponent(typeof(Movement))]
public class CharacterInput : MonoBehaviour, IPauseable
{
    //Components
    /// <summary> The Movement of this component's game object. </summary>
    Movement movement;
    /// <summary> The Character of this component's game object. </summary>
    Character character;
    /// <summary> The ControllerManager of this component's game object. </summary>
    ControllerManager controllerManager;

    //Pause
    bool paused = false;
    public bool IsPaused { get => paused; set => paused = value; }

    //Attack
    /// <summary> The direction that this character is aiming. </summary>
    [HideInInspector] public Vector2 aimVector = Vector2.one; //The direction of the aim reticle


    private void Start()
    {
        SubscribeToGameManager(); //Adds to the GameManager's list of pausable objects

        //Get components
        controllerManager = GetComponent<ControllerManager>();
        character = gameObject.GetComponent<Character>();
        movement = gameObject.GetComponent<Movement>();

        //Setup for manager components
        GameManager.instance.player = character;
    }

    #region Inputs

    /// <summary> Calls the player's HUD to pause. </summary>
    void OnPause(InputValue inputValue)
    {
        HUDManager.instance.PauseGame();
    }

    #region Walk
    /// <summary> Applies player's input to this character's movement. </summary>
    void OnWalk(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();

        if (!paused)
        {
            movement.input_direction = input;
        }
    }

    /// <summary> Applies player's keyboard input to this character's movement. </summary>
    void OnWalkKeyboard(InputValue inputValue) {
        movement.input_direction = (!paused && !ControllerManager.instance.CONTROLLERENABLED) ? inputValue.Get<Vector2>() : Vector2.zero;
    }
    /// <summary> Applies player's gamepad input to this character's movement. </summary>
    void OnWalkStick(InputValue inputValue) {
        movement.input_direction = (!paused && ControllerManager.instance.CONTROLLERENABLED) ? inputValue.Get<Vector2>() : Vector2.zero;
    }
    /// <summary> Applies player's mobile input to this character's movement. </summary>
    public void OnWalkMobile(Vector2 input)  {
        movement.input_direction = !paused ? input : Vector2.zero;
    }
    #endregion

    #region Attack

    /// <summary> Calls this character to attack using mouse. </summary>
    public void OnAttackMouse()
    {
        if (!paused && !ControllerManager.instance.CONTROLLERENABLED) {  
            character.Attack();
        }
    }
    /// <summary> Calls this character to attack using gamepad. </summary>
    public void OnAttackTrigger()
    {
        if (!paused && ControllerManager.instance.CONTROLLERENABLED) {
            character.Attack();
        }
    }
    /// <summary> Calls this character to attack using mobile. </summary>
    public void OnAttackMobile()
    {
        if (!paused)
        {
            character.Attack();
        }
    }
    #endregion

    #region Aim
    /// <summary> Aims using mouse. </summary>
    public void OnAimMouse(InputValue inputValue)
    {
        if (!paused)
        {
            aimVector = (paused || ControllerManager.instance.CONTROLLERENABLED) ? Vector2.zero : inputValue.Get<Vector2>();
        }
    }
    /// <summary> Aims using gamepad. </summary>
    public void OnAimStick(InputValue inputValue)
    {
        if (!paused)
        {
            aimVector = (paused || !ControllerManager.instance.CONTROLLERENABLED) ? Vector2.zero : inputValue.Get<Vector2>().normalized;
        }
    }
    /// <summary> Aims using mobile. </summary>
    public void OnAimMobile(Vector2 inputValue)//for mobile, note not a callback, gets called manually
    {
        if (!paused)
        {
            aimVector = (paused) ? Vector2.zero : inputValue.normalized;
        }
    }
    #endregion

    #region Items
    /// <summary> Calls character to pick up an item in range. </summary>
    public void OnPickup()
    {
        if (!paused)
        { character.Pickup(); }
    }
    /// <summary> Drops an item out of the player's inventory. </summary>
    public void OnDropItem(InputValue value)
    {
        if (!GameManager.instance.inventory.HasItemEquiped() || paused) { return; }
        KeyValuePair<string, int> itemData = GameManager.instance.inventory.RemoveAtIndex(GameManager.instance.inventory.selcetedIndex);
        HUDManager.instance.inventoryUI.RemoveImage(GameManager.instance.inventory.selcetedIndex);
        ItemPool.instance.SpawnItem(transform.position + new Vector3(0,1,0), itemData.Key, itemData.Value);
    }
    /// <summary> Uses an item from the player's inventory. </summary>
    public void OnUseItem(InputValue value)
    {
        if (!paused)
        {
            //Open shop
            if (character.ShopInRange() != null)
            {
                Shop shop = character.ShopInRange();
                shop.OpenShop();
                Pause();
                movement.Pause();
            }
            //Use item
            else
            {
                GameManager.instance.inventory.UseSelectedItem();
            }
        }
    }
    /// <summary> Scrolls through your items, assigning the player's equipped item to an adjascent item in their inventory. </summary>
    public void OnShiftItem(InputValue value)
    {

        Vector2 direction = value.Get<Vector2>();

        if (direction.x != 0)
        {
            if (!GameManager.instance.cheats.showConsole)
            {
                HUDManager.instance.inventoryUI.ScrollSelectedIndex((int)direction.x);
            }
        }
        else if (direction.y != 0)
        {
            if (GameManager.instance.cheats.showConsole)
            {
                GameManager.instance.cheats.SetInputPrevious();
            }
            else
            {
                HUDManager.instance.inventoryUI.ScrollSelectedIndex((int)direction.y * -5);
            }
        }

        HUDManager.instance.selectedUI.RefreshDisplay();
    }
    #endregion

    public void Pause()
    {
        paused = true;
    }

    public void Play()
    {
        paused = false;
    }

    public void SubscribeToGameManager()
    {
        if (!GameManager.instance.pausables.Contains(this))
        { GameManager.instance.pausables.Add(this); }
    }

    /// <summary> Opens/closes the player's inventory. </summary>
    public void OnInventory()
    {
        if (!GameManager.instance.cheats.showConsole && !paused)
        { GameManager.instance.ToggleInvetory(); }
    }

    /// <summary> Opens/closes the cheats menu. </summary>
    void OnCheats()
    { GameManager.instance.cheats.showConsole = !GameManager.instance.cheats.showConsole; }
    /// <summary> Calls the cheat command that was type into the cheat line. </summary>
    void OnReturn()
    { GameManager.instance.cheats.HandleInput(); }


    #region Inventory
    /// <summary> Selects an item in the inventory for drag-and-drop. </summary>
    public void OnInventoryItemGrab(InputValue value)
    {
        HUDManager.instance.inventoryUI?.OnLeftClick();
    }
    /// <summary> Equips the selected item. </summary>
    public void OnInventoryItemSelect(InputValue value)
    {
        HUDManager.instance.inventoryUI?.OnRightCLick();
    }
    #endregion

    #endregion
}
