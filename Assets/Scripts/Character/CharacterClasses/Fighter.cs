
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> The fighter's character class. </summary>
public class Fighter : Character
{
    //Attack
    /// <summary> The duration that the fighter's hitbox is active during the attack. </summary>
    float attackDuration = 0.2f;
    /// <summary> The duration between the time that the fighter starts their attack, and the time that the attack hitbox becomes active. </summary>
    float attackWindup = 0.36f;
    ///<summary>Value that is set on attack start</summary>
    float attackStartTime;

    /// <summary> The fighter's character class. </summary>
    public Fighter()
    {
        characterType = CharacterType.fighter;
        scoreReward = 15;
        maxHp = 50;
        attackDamage = 20;
        attackRestDuration = 0.6f;
    }
    protected override void Awake()
    {
        base.Awake();
       
    }
    protected override void Start()
    {
        base.Start();
        //Enable class specific HUD
        if (isPlayer)
        { HUDManager.instance.playerHUD.ShowSword(); }
    }


    protected override void Update()
    {
        base.Update();

        if (isPlayer) { CheckCoolDown(); }

        if (attackFreeze && !isDead)
        {
            //Detect hit
            float attackDist = (isPlayer) ? (0.4f) : (0f);
            RaycastHit[] hitArray = Physics.SphereCastAll(transform.position + animatedChild.transform.forward, 0.8f, animatedChild.transform.forward, attackDist, attackLayer);
            foreach (RaycastHit hit in hitArray)
            {
                Character hitCharacter = hit.collider.gameObject.GetComponent<Character>();
                if (hitCharacter)
                {
                    hitCharacter.Hurt(attackDamage);
                }
            }
        }
    }

    #region Attack

    /// <summary> Attacks and returns true if the attack was successful. </summary>
    public override bool Attack()
    {
        if (!base.Attack())
        { return false; }
        attackStartTime = Time.realtimeSinceStartup;
        animationAttack = true;
        StartCoroutine(MathFunc.Timer(attackWindup, "Attack_StartHit", gameObject));

        AudioManager.PlaySound(1, Random.Range(0,3));

        return true;
    }
    /// <summary> Activates the attack hitbox. </summary>
    void Attack_StartHit()
    {
        attackFreeze = true;
        StartCoroutine(MathFunc.Timer(attackDuration, "Attack_EndHit", gameObject));
    }
    /// <summary> Deactivates the attack hitbox. </summary>
    void Attack_EndHit()
    {
        animationAttack = false;
        attackFreeze = false;
    }

    #endregion

    /// <summary> Updates the sword UI. </summary>
    void CheckCoolDown()
    {
        HUDManager.instance.ChangeMana((int)(attackRestDuration - MathFunc.TimeLeft(attackRestDuration, attackStartTime) * 100), (int)(attackRestDuration * 100), true);
    }

    private void OnDrawGizmos()
    {
        if (attackFreeze)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + animatedChild.transform.forward, 0.8f);
        }
    }

}
