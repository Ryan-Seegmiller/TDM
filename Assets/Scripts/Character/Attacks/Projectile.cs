using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour, IPauseable
{
    // Element of the projectile
    private int element = 0; // 0 represents no element
    // Base stats of the projectile
    private float damage = 0;
    private float range = 0;
    private float speed = 5;

    // Array of elemental materials for visual effects
    Material[] elementalMaterials;

    // Total movement of the projectile
    private Vector3 totalMovement = Vector3.zero;

    // Flag to check if the game is paused
    public bool paused;

    // Layer mask for detecting attacks
    [HideInInspector] public LayerMask attackLayer;

    // Flag to determine if the projectile is owned by the player
    [HideInInspector] public bool OwnedByPlayer = false;

    // Components of the projectile
    MeshRenderer mR;
    ParticleSystemRenderer psR;

    // Target position of the projectile
    public Vector3 target;

    // Implementation of the IPauseable interface
    bool IPauseable.IsPaused { get => paused; set => paused = value; }

    private void Awake()
    {
        // Get components
        elementalMaterials = Resources.LoadAll<Material>("Runes/ElementalMaterials/");
        attackLayer = LayerMask.GetMask("Default") | attackLayer;
        psR = GetComponent<ParticleSystemRenderer>();
        mR = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        // Update only if the game is not paused
        if (!paused)
        {
            ApplyMovement();
            DetectCollision();
            CheckRange();
        }
    }

    private void Start()
    {
        // Set the stats of the projectile
        SetStats();
        SubscribeToGameManager();
        // Rotate the projectile to face the target
        gameObject.transform.LookAt(target);
    }

    // Set the stats of the projectile based on runes
    public void SetStats()
    {
        if (RuneManager.instance == null) { return; }
        int[] runes = (OwnedByPlayer) ? RuneManager.instance.selectedModRunes : new int[3] { 0, 0, 0 };
        for (int i = 0; i < runes.Length; i++)
        {
            damage += RuneDataSheet.runeStats[runes[i]].damage;
            range += RuneDataSheet.runeStats[runes[i]].range;
            speed += RuneDataSheet.runeStats[runes[i]].speed;
        }
        element = (OwnedByPlayer) ? RuneManager.instance.selectedElementRune : Random.Range(0, RuneManager.ELEMENTALRUNECOUNT);
        psR.sharedMaterial = elementalMaterials[element];
        mR.sharedMaterial = elementalMaterials[element];
    }

    // Pause the projectile
    void IPauseable.Pause()
    {
        paused = true;
    }

    // Resume the projectile
    void IPauseable.Play()
    {
        paused = false;
    }

    // Detect collisions with other objects
    void DetectCollision()
    {
        int layerMask = attackLayer | LayerMask.GetMask("Default");
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f, layerMask);
        if (hits.Length > 0)
        {
            foreach (Collider hit in hits)
            {
                // Try to get the Character component from the hit object
                Character hitCharacter;
                hit.gameObject.TryGetComponent(out hitCharacter);
                if (hitCharacter != null)
                {
                    // Inflict damage and status effect on the hit character
                    hitCharacter.Hurt((int)damage);
                    hitCharacter.SetStatusEffect((StatusEffect)element, 3, 1);
                }
                // Destroy the projectile after hitting a target
                Destroy(gameObject);
            }
        }
    }

    // Check if the projectile has exceeded its range
    void CheckRange()
    {
        if (totalMovement.magnitude >= range) { Destroy(this.gameObject); }
    }

    // Apply movement to the projectile
    void ApplyMovement()
    {
        Vector3 toMove = transform.forward * speed * Time.deltaTime;
        totalMovement += toMove;
        transform.position += toMove;
    }

    // Subscribe the projectile to the GameManager's pauseable list
    public void SubscribeToGameManager()
    {
        if (!GameManager.instance.pausables.Contains(this))
        {
            GameManager.instance.pausables.Add(this);
        }
    }

    // Draw a wire sphere around the projectile for visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
