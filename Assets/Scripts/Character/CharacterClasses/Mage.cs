using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> The mage's character class. </summary>
public class Mage : Character
{
    /// <summary> The prefab object of the mage's projectile. </summary>
    GameObject projectilePrefab;

    /// <summary> The start position of the player's aim line. </summary>
    private Vector3 attackStartPoint
    {
        get { return transform.position + Vector3.up * 1f + animatedChild.transform.forward * 1; }
    }
    /// <summary> The end position of the player's aim line. </summary>
    private Vector3 attackEndPoint {
        get { return animatedChild.gameObject.transform.position + Vector3.up * 1f + animatedChild.gameObject.transform.forward * 25;}
    }
    /// <summary> An array containing the start and end position of the player's aim line. </summary>
    private Vector3[] attackLine {
        get { return new Vector3[2] { attackStartPoint, attackEndPoint }; }
    }
    /// <summary> The mage's character class. </summary>
    public Mage()
    {
        characterType = CharacterType.mage;
        scoreReward = 10;
        maxHp = 40;
        attackDamage = 0; //Attack damage is dependent on rune selection
        attackRestDuration = 0.6f;
        maxAmmo = 100;
        ammo = maxAmmo;
        ammoReplenishDuration = 0.3f;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        projectilePrefab = Resources.Load<GameObject>("AttackPrefabs/Projectile");

        //Enable class specific HUD
        if (isPlayer)
        {
            HUDManager.instance.playerHUD.ShowMana();
            HUDManager.instance.ChangeMana(ammo, maxAmmo);
        }
    }

    protected override void Update()
    {
        HandleReticleLR(attackLine);

        base.Update();
    }
    /// <summary> Attacks and returns true if the attack was successful. </summary>
    public override bool Attack()
    {
        if ((isPlayer && ammo < RuneManager.instance.GetManaCost()) || !base.Attack())
        { return false; }
        AudioManager.PlaySound(2, RuneManager.instance.GetElement());
        //Attack
        Projectile newProjectile = Instantiate(projectilePrefab, attackStartPoint, Quaternion.identity).GetComponent<Projectile>();
        newProjectile.target = attackEndPoint;
        newProjectile.attackLayer = attackLayer;
        newProjectile.OwnedByPlayer = isPlayer;

        ammo -= RuneManager.instance.GetManaCost();

        //Update ammo UI
        if (isPlayer)
        { HUDManager.instance.ChangeMana(ammo, maxAmmo); }

        return true;
    }
}
