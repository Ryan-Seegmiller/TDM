using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A custom mobile control that acts like a console joystick
/// </summary>
public class VirtualJoyStick : MoblieControl
{
    /// <summary> The collected value from being pressed down </summary>
    public Vector2 direction;

    ///Center of joystick
    Vector2 pointA;
    ///Stick
    Vector2 pointB;

    [Header("Joystick")]
    ///Fixed or based on first touch point
    public bool dynamic = true;
    ///Visual background to joystick transform
    public RectTransform background;
    ///Visual stick transform
    public RectTransform stick;
    [Tooltip("Adjust how far over the edge the stick gets")]
    ///Stick offset
    [Range(2f, 10f)] public float visualOffsetAmount = 10;
    ///Distance form the edge of the background
    float offsetFromEdge = 10;
    ///Maxium stick distance from center
    float distanceFromCenter = 50;

    ///When the stick updates
    public UnityEvent moveEvent;

    protected override void Start()
    {
        base.Start();
        //Assumes both are square
        offsetFromEdge = (stick.rect.width / visualOffsetAmount);
        distanceFromCenter = (background.rect.width / 2) - offsetFromEdge;

        Released();
    }

    void Update()
    {
        UpdateStick();
    }

    /// <summary> Visual stick update and invoke events </summary>
    void UpdateStick()
    {
        if (touched)
        {
            pointB = touchPostion;
            Vector2 offset = pointB - pointA;
            Vector2 stickLocation = Vector2.ClampMagnitude(offset, distanceFromCenter * zone.lossyScale.magnitude * 0.5f);
            direction = stickLocation.normalized; //for taking the input

            stick.position = new Vector2(background.position.x + stickLocation.x, background.position.y + stickLocation.y);
            moveEvent?.Invoke();
        }
    }
    /// <summary> Reset visuals and invoke events </summary>
    public override void Released()
    {
        if (!dynamic) { stick.position = background.position; }
        //disable
        if (stick.gameObject.activeInHierarchy && dynamic) { stick.gameObject.SetActive(false); }
        if (background.gameObject.activeInHierarchy && dynamic) { background.gameObject.SetActive(false); }
        //reset position
        direction = Vector2.zero;
        //trigger behaviors
        moveEvent?.Invoke();
    }
    /// <summary> Set up finger tracking with stick </summary>
    public override void Clicked()
    {
        if (dynamic) { pointA = initialTouchPostion; }
        else { pointA = background.position; }
        //enable
        if (!stick.gameObject.activeInHierarchy)
        {
            stick.gameObject.SetActive(true);
            if (dynamic) { stick.position = pointA; } //reposition
        }
        if (!background.gameObject.activeInHierarchy)
        {
            background.gameObject.SetActive(true);
            if (dynamic) { background.position = pointA; } //reposition
        }
    }

    //Specific binding overrides
    /// <summary> Mobile Aim() override </summary>
    public void Aim()
    {
        if (RuneManager.instance.modRuneMenuOpen) { RuneManager.instance.OnMobileStick(direction); }
        else { playerInput?.OnAimMobile(direction); }
    }
    /// <summary> Mobile Walk() override </summary>
    public void Walk()
    {
        playerInput?.OnWalkMobile(direction);
    }
}
