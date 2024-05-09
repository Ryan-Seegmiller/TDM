using TMPro;
using UnityEngine;
using System.Collections.Generic;
using AI;
using FloatingText;
using LevelGeneration;

//Enums
/// <summary> The character class enum. </summary>
public enum CharacterType { fighter, archer, mage, blob }
/// <summary> The possible status effects that a character can contract. </summary>
public enum StatusEffect { frozen, onFire,  cursed }

//Status effect
/// <summary> An active status effect. </summary>
public class Status
{
    /// <summary> The status effect type. </summary>
    public StatusEffect effect;
    /// <summary> Multiplies the effects of the status effect. </summary>
    public float multiplier;
    /// <summary> The total duration before the status effect disappears. </summary>
    public float duration;
    /// <summary> How long the status effect has left. </summary>
    public float timer = 0;
    
    /// <summary>
    /// An active status effect.
    /// </summary>
    /// <param name="newEffect"></param>
    /// <param name="newDuration"></param>
    /// <param name="newMultiplier"></param>
    public Status(StatusEffect newEffect, float newDuration, float newMultiplier = 1)
    {
        this.effect = newEffect;
        this.duration = newDuration;
        this.multiplier = newMultiplier;
    }
}

/// <summary> The character base class. </summary>
[RequireComponent(typeof(Movement))]
public abstract class Character : MonoBehaviour, IPauseable
{
    //Components
    /// <summary> The Movement of this component's game object. </summary>
    [HideInInspector] public Movement movement;
    /// <summary> This character's ID label. </summary>
    private GameObject label;
    /// <summary> The text of this character's ID label. </summary>
    private TextMeshProUGUI labelText;
    /// <summary> The transform of this character's ID label. </summary>
    private RectTransform labelTransform;
    /// <summary> This character's animation game object. </summary>
    [HideInInspector] public CharacterAnimationBind animatedChild = null;

    //Character values
    /// <summary> This character is the player. </summary>
    public bool isPlayer = false;
    /// <summary> This character's type/class. </summary>
    public CharacterType characterType;
    /// <summary> The collision layer that this character can damage with their attacks. </summary>
    [HideInInspector] public LayerMask attackLayer;

    //Status effects
    /// <summary> This character's active status effects. </summary>
    List<Status> activeStatusEffects = new List<Status>();
    /// <summary> How frequently the character takes fire damage while on fire. </summary>
    float fireDamageDuration = 1;
    /// <summary> How much time is left before the character takes fire damage again (if on fire). </summary>
    float fireDamageTimer = 0;

    //Respawn
    /// <summary> The duration after death before the character respawns. </summary>
    float respawnDuration = 3;
    /// <summary> Where the character will spawn when respawning. </summary>
    Vector3 spawnPosition = Vector3.zero;

    //Game control
    /// <summary> Makes this character invincible. </summary>
    [HideInInspector] public bool godMode = false;
    /// <summary> Called when this character takes damage. </summary>
    /// <param name="dmg"></param>
    public delegate void OnDamage(int dmg);
    /// <summary> Called when this character takes damage. </summary>
    public OnDamage onDamage = null;

    //Game pause
    bool paused = false;
    bool IPauseable.IsPaused { get => paused; set => paused = value; }

    //Enemy values
    /// <summary> This character's individual unique ID. </summary>
    [HideInInspector] public string characterID;
    /// <summary> The score that the player is rewarded when this character is killed. </summary>
    [HideInInspector] public int scoreReward = 0;

    //Health
    /// <summary> This character's max HP. </summary>
    [HideInInspector] public int maxHp = 10;
    /// <summary> This character's current HP. </summary>
    protected int hp = 10;
    /// <summary> The bonus health multiplier that the character gets if it is a player. </summary>
    const float playerHealthMultiplier = 2.5f;
    /// <summary> This character is dead. </summary>
    [HideInInspector] public bool isDead
    { get { return hp <= 0; } }
    /// <summary> The duration of damage immunity after this player is hurt. </summary>
    float immunityDuration = 0.5f;
    /// <summary> This character is currently invulnerable/immune to damage. </summary>
    bool invulnerable = false;

    //Ammo
    /// <summary> This character's maximum ammo. </summary>
    protected int maxAmmo = 10;
    /// <summary> This character's current ammo. </summary>
    protected int ammo = 10;
    /// <summary> The frequency that this character replenishes ammo. </summary>
    protected float ammoReplenishDuration = 1;
    /// <summary> How much longer until this character replenishes some ammo. </summary>
    protected float ammoReplenishTimer = 0;

    //Attack
    /// <summary> This character is currently playing its attack animation. </summary>
    [HideInInspector] public bool animationAttack = false;
    /// <summary> This character is in a point of its attack that's preventing movement. </summary>
    [HideInInspector] public bool attackFreeze = false;
    /// <summary> The damage that this character deals when attacking another character. </summary>
    protected int attackDamage = 0;
    /// <summary> How long the character has to rest between attacks. </summary>;
    public float attackRestDuration = 0.5f;
    /// <summary> The time left before the character is available to attack again. </summary>
    protected float attackRestTimer = 0;


    //Reticle
    /// <summary> The player's reticle. </summary>
    LineRenderer reticleLR;

    //Door
    /// <summary> Returns true if there is a door in range of the player. </summary>
    [HideInInspector] public bool doorInArea {
        get { return _doorInArea; }
        set { 
            if (value != _doorInArea) 
            {
                _doorInArea = value;
                if (value)
                {
                    doorText = FloatingTextManager.instance.SetStationaryFloatingText(FloatingTextManager.instance.doorText ,DoorInRange().transform.position);
                    ControlPromptsManager.instance.ActivateControl("UseItem");
                }
                else
                {
                    if (!GameManager.instance.inventory.HasItemEquiped())
                    {
                        ControlPromptsManager.instance.DeactivateControl("UseItem");
                    }
                    doorText.RemoveText();
                }
            } 
        } 
    }
    private bool _doorInArea;
    /// <summary> The floating text that appears when near a door. </summary>
    FloatingText.FloatingText doorText;

    //Shop
    /// <summary> Returns true if there is a shop in range of the player. </summary>
    [HideInInspector]
    public bool shopInArea
    {
        get { return _shopInArea; }
        set
        {
            if (value != _shopInArea)
            {
                _shopInArea = value;
                if (value)
                {
                    shopText = FloatingTextManager.instance.SetStationaryFloatingText(FloatingTextManager.instance.shopText, ShopInRange().transform.position);
                    ControlPromptsManager.instance.ActivateControl("UseItem");
                    MobileControlManager.instance.shopButton?.gameObject.SetActive(true);
                }
                else
                {
                    shopText.RemoveText();
                    if (!GameManager.instance.inventory.HasItemEquiped())
                    {
                        ControlPromptsManager.instance.DeactivateControl("UseItem");
                    }
                    MobileControlManager.instance.shopButton?.gameObject.SetActive(false);
                }
            }
        }
    }
    private bool _shopInArea;
    /// <summary> The floating text that appears when near a door. </summary>
    FloatingText.FloatingText shopText;

    protected virtual void Awake()
    {
        //Get components
        movement = GetComponent<Movement>();
        //Animator component
        string animatedChildName = "";
        switch (characterType)
        {
            case (CharacterType.fighter):
                animatedChildName = "animated_knight";
                break;
            case (CharacterType.archer):
                animatedChildName = "animated_archer";
                if (isPlayer) { SetupReticleLR(); }
                break;
            case (CharacterType.mage):
                animatedChildName = "animated_mage";
                if (isPlayer) { SetupReticleLR(); }
                break;
            case (CharacterType.blob):
                animatedChildName = "animated_blob";
                break;
        }
        animatedChild = transform.Find(animatedChildName).gameObject.GetComponent<CharacterAnimationBind>();

        //Cache spawn position
        spawnPosition = transform.position;

        //Set HP
        if (isPlayer)
        {
            maxHp = (int)(maxHp * playerHealthMultiplier);
        }
        hp = maxHp;
    }

    protected virtual void Start()
    {
        SubscribeToGameManager();

        //Create and set label
        if (HUDManager.instance.characterLabels)
        {
            label = Instantiate(Resources.Load<GameObject>("CharacterPrefabs/CharacterLabel"), HUDManager.instance.characterLabels);
            labelText = label.GetComponent<TextMeshProUGUI>();
            labelTransform = label.GetComponent<RectTransform>();
            labelText.text = characterID.ToString();
            label.SetActive(false);
        }

        //Set player variables
        if (isPlayer)
        {
            RuneManager.instance?.ToggleRuneUI(characterType == CharacterType.mage);

            attackLayer = LayerMask.GetMask("Enemy");
            //Set player HUD
            HUDManager.instance?.ChangeHealth(hp, maxHp);
            HUDManager.instance?.ChangeMana(ammo, maxAmmo);

        }
        //Set enemy variables
        else
        {
            attackLayer = LayerMask.GetMask("Player");
            respawnDuration = 0;
        }
    }

    protected virtual void Update()
    {
        if (!paused)
        {
            SetLabelPosition();
            if (!isDead)
            {
                AttackRestTimer();
                AmmoTimer();
                StatusTimer();
                OnFireTimer();
                if (isPlayer)
                { ManageIcons(); DoorInRange(); ShopInRange(); }
            }
        }
    }

    /// <summary> Activate/deactivate control prompt icons as needed. </summary>
    void ManageIcons()
    {
        //Item pickup
        if (ItemInRange())
        { ControlPromptsManager.instance.ActivateControl("PickUp"); }
        else
        { ControlPromptsManager.instance.DeactivateControl("PickUp"); }

        //Item selected
        if (GameManager.instance.inventory.HasItemEquiped())
        { ControlPromptsManager.instance.ActivateGroup("Item"); }
        else
        { if(!shopInArea)ControlPromptsManager.instance.DeactivateGroup("Item"); }
    }

    /// <summary> Manages the attack timer decrement and timeout functionality. </summary>
    void AttackRestTimer()
    {
        if (attackRestTimer > 0)
        {
            attackRestTimer -= Time.deltaTime;
            //Timeout
            if (attackRestTimer <= 0)
            {
                attackRestTimer = 0;
            }
        }
    }


    /// <summary> Manages the ammo replenish timer decrement and timeout functionality. </summary>
    void AmmoTimer()
    {
        if (ammoReplenishTimer > 0)
        {
            ammoReplenishTimer -= Time.deltaTime;
            //Timeout
            if (ammoReplenishTimer <= 0)
            {
                if (ammo < maxAmmo)
                {
                    ammo++;
                    if (isPlayer)
                    {
                        HUDManager.instance.ChangeMana(ammo, maxAmmo);
                    }
                    if (ammo < maxAmmo)
                    { ammoReplenishTimer = ammoReplenishDuration; }
                }
            }
        }
    }

    /// <summary> Manages the status effect timer decrement and timeout functionality. </summary>
    void StatusTimer()
    {
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            activeStatusEffects[i].timer += Time.deltaTime;
            //Status timeout
            if (activeStatusEffects[i].timer >= activeStatusEffects[i].duration)
            {
                //Delete status
                activeStatusEffects.Remove(activeStatusEffects[i]);
                return;
            }
        }
    }

    /// <summary> Manages the fire damage timer decrement and timeout functionality. </summary>
    void OnFireTimer()
    {
        Status status = GetStatusEffect(StatusEffect.onFire);
        if (status != null)
        {
            //Increment timer
            if (fireDamageTimer > 0)
            {
                fireDamageTimer -= Time.deltaTime;
                if (fireDamageTimer <= 0)
                {
                    fireDamageTimer = fireDamageDuration;
                    Hurt(Mathf.FloorToInt(1f * status.multiplier));
                }
            }
        }
    }

    /// <summary> Sets the position of the character's ID label. </summary>
    void SetLabelPosition()
    {
        if (label)
        {
            if (GameManager.showLabels)
            {
                //Display labels
                labelTransform.position = GameManager.instance.mainCamera.WorldToScreenPoint(transform.position + new Vector3(0, 1f));
                if (!label.activeSelf)
                { label.SetActive(true); }
            }
            else if (label.activeSelf)
            {
                //Hide labels
                label.SetActive(false);
            }
        }
    }
    /// <summary> Load and create line renderer for displaying where the player's projectiles will travel. </summary>
    private void SetupReticleLR()
    {
        reticleLR = Instantiate((GameObject)Resources.Load("LineRenderers/ReticleLR"), Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>();
        ResetReticleLR();
    }
    /// <summary> Sets the position of the reticle. </summary>
    protected void HandleReticleLR(Vector3[] vertices)//Called in Update of derived Mage and Archer class. Vertices are calculated at derived class and passed in as an Vector3[]
    {
        if (reticleLR == null) { return; }//Stop if no LR
        if (isDead) { ResetReticleLR(); }//Dont show if dead
        else
        {
            reticleLR.positionCount = vertices.Length;
            reticleLR.SetPositions(vertices);
        }
    }
    /// <summary> Resets the position of the reticle. </summary>
    protected void ResetReticleLR()//Clear LR
    {
        if (reticleLR == null) { return; }
        reticleLR.positionCount = 0;
    }
    #region Actions

    /// <summary> Heals this character by a given amount. </summary>
    /// <param name="amnt"></param>
    public void Heal(int amnt)
    {
        if (!isDead && hp < maxHp)
        {
            hp += amnt;
            hp = Mathf.Min(hp, maxHp);
            HUDManager.instance.ChangeHealth(hp, maxHp);
        }
    }
    /// <summary> Heals this character by a given percent. </summary>
    /// <param name="percantage"></param>
    public void HealPercent(int percantage)
    {
        float amnt = ((float)maxHp * ((float)percantage / 100f));
        if (!isDead && hp < maxHp)
        {
            hp += (int)amnt;
            hp = Mathf.Min(hp, maxHp);
            HUDManager.instance.ChangeHealth(hp, maxHp);
        }
    }
    /// <summary> Returns true if this character is at max health. </summary>
    public bool IsMaxHealth()
    {
        return hp >= maxHp - maxHp * .05f;
    }

    /// <summary> Damages the character by the given amount and checks for death. </summary>
    /// <param name="dmg"></param>
    public void Hurt(int dmg)
    {
        if (!isDead && !invulnerable && !godMode)
        {
            if (characterType == CharacterType.blob) { animatedChild.anim.SetTrigger("hit"); }
            ParticleManager.PlayParticle(0, transform.position + Vector3.up);
            hp -= dmg;
            invulnerable = true;
            onDamage?.Invoke(dmg);

            //Set invulnerability timer
            StartCoroutine(MathFunc.Timer(immunityDuration, "MakeVulnerable", gameObject));

            //Update HP bar
            if (isPlayer)
            {
                HUDManager.instance.ChangeHealth(hp, maxHp);
            }

            //Dead
            if (hp <= 0)
            {
                Die();
            }
        }
    }

    /// <summary> Turns the character's invulnerability off. </summary>
    public void MakeVulnerable()
    {
        invulnerable = false;
    }

    /// <summary> Kills the character. </summary>
    public void Die()
    {
        hp = 0;
        //spawnTimer = 0;
        movement.StopMovement();

        //Log death for heat map
        if (isPlayer)
        {
            //Set respawn timer
            StartCoroutine(MathFunc.Timer(respawnDuration, GameManager.instance.EndGame));
            AudioManager.PlaySound(3, 1);
            SaveSystem.LogGameFail(new DeathData((int)characterType, transform.position));
        }
        //Enemy ded
        else
        {
            //Drop item
            AIBrain brainBase = GetComponent<AIBrain>();
            if (brainBase)
            {
                if (brainBase.lootTable == null) { return; }

                ItemData item = brainBase.lootTable.GetRandom();
                ItemPool.instance.SpawnItem(brainBase.position + new Vector3(0,1,0), item.itemName);

                GameManager.instance.OnEnemyDied(this);
            }

            //Add to score
            GameManager.instance.AddScore((int)(scoreReward * GameManager.instance.scoreMultiplier), GameManager.instance.mainCamera.WorldToScreenPoint(transform.position));
        }
    }

    /// <summary> Respawns this character and resets to full health. </summary>
    public void Respawn()
    {
        //Set HP
        hp = maxHp;
        ammo = maxAmmo;
        if (isPlayer)
        { HUDManager.instance.ChangeHealth(hp, maxHp); }
        //Set position to respawn point
        transform.position = WorldGenerator.instance.spawnPosition;
    }

    /// <summary> Picks up an item near the character. </summary>
    public void Pickup()
    {
        LayerMask itemLayer = LayerMask.GetMask("Item");
        RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, 0.8f, animatedChild.transform.forward, 0.4f, itemLayer);
        if (hitArray.Length > 0)
        {
            Item item = hitArray[0].collider.gameObject.GetComponent<Item>();
            if (item)
            {
                item.Pickup();
            }
        }
    }

    /// <summary> Stops the character's attack animation. </summary>
    public void StopAttackAnimation()
    {
        animationAttack = false;
    }

    #region Status effects

    //Gives this character a new status effect
    public void SetStatusEffect(StatusEffect statusEffect, float statusDuration, float statusMultiplier = 1)
    {
        Status status = GetStatusEffect(statusEffect);
        //Add status effect
        if (status == null)
        {
            activeStatusEffects.Add(new Status(statusEffect, statusDuration, statusMultiplier));
        }
        //Status effect already exists; update stats
        else
        {
            status.duration = statusDuration;
            status.timer = 0;
            status.multiplier = statusMultiplier;
        }

        //Set specific status effect stats
        if (statusEffect == StatusEffect.onFire)
        { fireDamageTimer = fireDamageDuration; }
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        Status status = GetStatusEffect(statusEffect);
        //Return if the status effect already doesn't exist
        if (status == null)
        { return; }
        //Remove status effect
        activeStatusEffects.Remove(status);
    }

    public void ClearStatusEffects()
    {
        activeStatusEffects.Clear();
    }

    //Returns the character's statusinstance  with the given status effect (or null if this character doesn't have the status effect)
    public Status GetStatusEffect(StatusEffect statusEffect)
    {
        foreach (Status status in activeStatusEffects)
        {
            if (status.effect == statusEffect)
            { return status; }
        }
        //Status effect does not exist
        return null;
    }

    #endregion

    #region Pause system

    void IPauseable.Pause()
    {
        paused = true;
    }

    void IPauseable.Play()
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

    #endregion

    /// <summary> Performs an attack. </summary>
    public virtual bool Attack()
    {
        if (attackRestTimer > 0 || isDead || GetStatusEffect(StatusEffect.cursed) != null || characterType == CharacterType.blob)
        {return false; }

        //Start attack timer
        attackRestTimer = attackRestDuration;

        //Start ammo replenish timer
        if (ammo == maxAmmo)
        {
            ammoReplenishTimer = ammoReplenishDuration;
        }
        animationAttack = true;
        return true;
    }

    #endregion

    private void OnDestroy()
    {
        GameManager.instance.RemoveFromPausables(this);
    }

    #region Returns

    /// <summary> Returns true if an item is in range of being picked up. </summary>
    bool ItemInRange()
    {
        LayerMask itemLayer = LayerMask.GetMask("Item");
        RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, 0.8f, animatedChild.transform.forward, 0.4f, itemLayer);
        return (hitArray.Length > 0);
    }
    /// <summary> Returns true if a door is in range of being opened. </summary>
    public Door DoorInRange()
    {
        LayerMask doorLayer = LayerMask.GetMask("Door");
        RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, 0.8f, animatedChild.transform.forward, 2f, doorLayer);
        doorInArea = (hitArray.Length > 0);
        if (doorInArea)
        {
            foreach (RaycastHit hit in hitArray)
            {
                if (hit.collider.TryGetComponent<Door>(out Door door))
                {
                    return door;
                }
            }
        }
        return null;
    }
    /// <summary> Returns true if a shop is in range of interaction. </summary>
    public Shop ShopInRange()
    {
        LayerMask itemLayer = LayerMask.GetMask("Shop");
        RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, 0.8f, animatedChild.transform.forward, 0.4f, itemLayer);
        if (hitArray.Length > 0)
        {
            Shop shop = hitArray[0].collider.GetComponent<Shop>();
            shopInArea = true;
            return shop;
        }
        shopInArea = false;

        return null;
    }

    #endregion
}
