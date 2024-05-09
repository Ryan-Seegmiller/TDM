using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary> The character's movement behavior. </summary>
public class Movement : MonoBehaviour, IPauseable
{
    /// <summary> The Character belonging to this component's game object. </summary>
    Character character;
    /// <summary> The Rigidbody belonging to this component's game object. </summary>
    Rigidbody rb3D;

    bool paused = false;
    public bool IsPaused { get => paused; set => paused = value; }

    /// <summary> How fast this character can move. </summary>
    public float walkSpeed = 5f;
    /// <summary> Adjusts this character's movement speed. </summary>
    internal float walkSpeedMultiplier = 1.3f;
    /// <summary> Applied to this character's movement speed when the character has the 'frozen' status effect. </summary>
    float frozenWalkMultiplier = 0.2f;

    /// <summary> This character's current velocity. </summary>
    [HideInInspector] public Vector2 velocity = Vector2.zero;
    /// <summary> The Vector2 direction that this character is trying to move in. </summary>
    [HideInInspector] public Vector2 input_direction = Vector2.zero;
    /// <summary> The last input direction, used to determine if input_direction has changed. </summary>
    Vector2 lastInput = Vector2.zero;


    private void Start()
    {
        SubscribeToGameManager();

        rb3D = GetComponent<Rigidbody>();

        character = gameObject.GetComponent<Character>();
    }

    private void Update()
    {
        if (!paused && !character.isDead)
        {
            //SetDirection();
            ApplyVelocity();
        }
        else
        {
            input_direction = Vector2.zero;
        }
    }

    /// <summary> Adds the character's input direction to its velocity and applies the velocity. </summary>
    void ApplyVelocity()
    {
        //Set velocity
        if (!character.attackFreeze && !character.isDead)
        {
            //Set the frozen status walk multiplier
            float frozenMultiplier = 1;
            Status frozenStatus = character.GetStatusEffect(StatusEffect.frozen);
            if (frozenStatus != null)
            {
                frozenMultiplier = (frozenStatus.multiplier != 0) ? (frozenWalkMultiplier / frozenStatus.multiplier) : (frozenWalkMultiplier); //Using a ternary to prevent division by 0
            }
            //Set velocity
            velocity = input_direction * walkSpeed * walkSpeedMultiplier * frozenMultiplier;
        }
        else
        {
            velocity = Vector2.zero;
        }
        //Move character
        rb3D.velocity = transform.TransformVector(new Vector3(velocity.x, 0, velocity.y));
    }

    /// <summary> Stops the character's current movement. </summary>
    public void StopMovement()
    {
        if (rb3D == null)
        {
            rb3D = GetComponent<Rigidbody>();
        }
        velocity = Vector2.zero;
        rb3D.velocity = Vector3.zero;
    }

    public void Pause()
    {
        paused = true;
        if(rb3D == null)
        {
            rb3D = GetComponent<Rigidbody>();
        }
        input_direction = Vector2.zero;
        rb3D.velocity = Vector2.zero;
    }

    public void Play()
    {
        paused = false;
    }

    public void SubscribeToGameManager()
    {
        if (!GameManager.instance.pausables.Contains(this))
        {
            GameManager.instance.pausables.Add(this);
        }
    }
    private void OnDestroy()
    {
        GameManager.instance.RemoveFromPausables(this);
    }

}