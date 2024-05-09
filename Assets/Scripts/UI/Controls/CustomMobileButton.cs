using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary> A mock unity button made for multi-touch </summary>
public class CustomMobileButton : MoblieControl
{
    //Held values
    [Header("Held values")]
    public bool isHoldable = false;
    bool held;
    //Timer
    public float heldTimerDuration;
    private float startTime;
    ///Radial effect, showing effect 
    public Image fillImage;

    public UnityEvent OnClick;
    public UnityEvent OnReleased;

     protected override void Start()
    {
        base.Start();
        if (isHoldable)
        {
            fillImage.fillAmount = 0;
        }
    }

    private void Update()
    {
        Held();  
    }
    /// <summary> Behaviors for being clicked </summary>
    public override void Clicked()
    {

        if (isHoldable)
        {
            held = true;
            startTime = Time.realtimeSinceStartup;
        }
        else
        {
            OnClick?.Invoke();
        }

    }
    /// <summary> Behaviors for being released </summary>
    public override void Released()
    {
        if (isHoldable && MathFunc.Timeout(heldTimerDuration, startTime))
        {
            OnReleased?.Invoke();
        }
        else if (isHoldable)
        {
            OnClick?.Invoke();
        }
        else
        {
            OnReleased?.Invoke();
        }
        if (isHoldable)
        {
            fillImage.fillAmount = 0;
        }
        held = false;
    }
    /// <summary> Behaviors for being held down </summary>
    public void Held()
    {
        if (!held) { return; }
        fillImage.fillAmount =  (heldTimerDuration - MathFunc.TimeLeft(heldTimerDuration, startTime)) / heldTimerDuration;
        if(MathFunc.Timeout(heldTimerDuration, startTime)) { MobileControlManager.instance.ForceQuitInput(index); }
    }

    /// <summary> Mobile Attack() override </summary>
    public void Attack()
    {
        playerInput?.OnAttackMobile();
    }
    /// <summary> Mobile PickUpItem() override </summary>
    public void PickUpItem()
    {
        playerInput?.OnPickup();
    }
    /// <summary> Mobile UseItem() override </summary>
    public void UseItem()
    {
        if (!GameManager.instance.inventory.HasItemEquiped()) { return; }
        playerInput?.OnUseItem(new UnityEngine.InputSystem.InputValue());
    }
    /// <summary> Mobile DropItem() override </summary>
    public void DropItem()
    {
        if (!GameManager.instance.inventory.HasItemEquiped()) { return; }
        playerInput?.OnDropItem(new UnityEngine.InputSystem.InputValue());
    }
    /// <summary> Mobile SelectItem() override </summary>
    public void SelectItem(InventorySlot slot)
    {
        if (MobileControlManager.instance.inventoryUI.inventoryActive)
        {
            MobileControlManager.instance.inventoryUI.SetSelected(slot);
        }
    }

    public void ShopOpen()
    {
        GameManager.instance.player.ShopInRange().OpenShop();
    }

    //Runes
    /// <summary> Mobile ToggleRunes() override </summary>
    public void ToggleRunes()
    {
        RuneManager.instance.OnOpenRuneMenu();
    }
    public void RuneLeft()
    {
        RuneManager.instance.OnLeft();
    }
    public void RuneRight()
    {
        RuneManager.instance.OnRight();
    }
}
