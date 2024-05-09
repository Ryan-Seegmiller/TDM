using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> The blob's character class. </summary>
public class Blob : Character
{

    /// <summary> The blob's character class. </summary>
    public Blob()
    {
        characterType = CharacterType.blob;
        scoreReward = 5;
        maxHp = 15;
        attackDamage = 5;
        attackRestDuration = 0.2f;
    }

    protected override void Start()
    {
        base.Start();
        if (isPlayer) { HUDManager.instance.playerHUD.ShowNone(); }
    }

    protected override void Update()
    {
        base.Update();

        DetectHit();
    }

    /// <summary> Check for an enemy character in range and cause damage if one is found. </summary>
    void DetectHit()
    {
        if (!isDead)
        {
            //Detect hit
            RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, 0.8f, animatedChild.transform.forward, 0, attackLayer);
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.8f);
    }

}
