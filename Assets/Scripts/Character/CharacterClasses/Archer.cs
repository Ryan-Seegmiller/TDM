using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> The archer's character class. </summary>
public class Archer : Character
{
    /// <summary> The prefab object of the arrow projectile. </summary>
    GameObject arrowPrefab;
    /// <summary> The start position of the player's aim line. </summary>
    private Vector3 attackStartPoint
    {
        get { return transform.position + Vector3.up * 1f + animatedChild.transform.forward * 1; }
    }
    /// <summary> The end position of the player's aim line. </summary>
    private Vector3 attackEndPoint
    {
        get { return animatedChild.gameObject.transform.position + Vector3.up * 1f + animatedChild.gameObject.transform.forward * 25; }
    }
    /// <summary> An array containing the start and end position of the player's aim line. </summary>
    private Vector3[] attackLine
    {
        get { return new Vector3[2] { attackStartPoint, attackEndPoint }; }
    }
    /// <summary> The archer's character class. </summary>
    public Archer()
    {
        characterType = CharacterType.archer;
        scoreReward = 10;
        maxHp = 30;
        attackDamage = 30;
        attackRestDuration = 1f;
        ammo = 10;
        ammoReplenishDuration = 1;
    }

    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Start()
    {
        base.Start();
        arrowPrefab = Resources.Load<GameObject>("AttackPrefabs/Arrow");

        //Enable class specific HUD
        if (isPlayer)
        {
            HUDManager.instance.playerHUD.ShowQuiver();
            HUDManager.instance.ChangeMana(ammo, maxAmmo);
        }
    }

    protected override void Update()
    {
        base.Update();
        HandleReticleLR(attackLine);
    }

    /// <summary> Attacks and returns true if the attack was successful </summary>
    public override bool Attack()
    {
        if (ammo <= 0 || !base.Attack())
        { return false; }

        Arrow3D newArrow = Instantiate(arrowPrefab, attackStartPoint, Quaternion.identity).GetComponent<Arrow3D>();

        newArrow.target = attackEndPoint;
        newArrow.attackLayer = attackLayer;
        newArrow.damage = attackDamage;
        newArrow.owner = this;

        ammo -= 1;

        //Update ammo UI
        if (isPlayer)
        { HUDManager.instance.ChangeMana(ammo, maxAmmo); }

        AudioManager.PlaySound(5, 0);

        return true;
    }
}
