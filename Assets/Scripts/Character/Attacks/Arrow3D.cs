using UnityEngine;

public class Arrow3D : MonoBehaviour, IPauseable
{
    // Movement speeds for the arrow
    float moveSpeedForward = 45;
    float moveSpeedDown = 0.05f;

    // Layer mask for detecting attacks
    [HideInInspector] public LayerMask attackLayer;
    // Damage inflicted by the arrow
    [HideInInspector] public int damage;

    // Flag to check if the game is paused
    bool paused = false;
    bool IPauseable.IsPaused { get => paused; set => paused = value; }

    // Initial position of the arrow
    Vector3 startPos = Vector3.zero;
    // Owner of the arrow
    public Character owner;
    // Target position of the arrow
    public Vector3 target;

    private void Start()
    {
        // Rotate the arrow to face the target
        gameObject.transform.LookAt(target);

        // Subscribe to the GameManager
        SubscribeToGameManager();

        // Combine the attack layer with the default layer
        attackLayer = LayerMask.GetMask("Default") | attackLayer;

        // Set the start position of the arrow
        startPos = transform.position;

        // Destroy the arrow after 10 seconds
        Destroy(this.gameObject, 10);
    }

    private void Update()
    {
        // Update only if the game is not paused
        if (!paused)
        {
            // Detect collisions with other objects
            DetectCollision();

            // Move the arrow forward
            transform.position += (gameObject.transform.forward * moveSpeedForward * Time.deltaTime);
            // Move the arrow downward
            transform.position += (-Vector3.up * moveSpeedDown * Time.deltaTime);
        }
    }

    // Detect collisions with other objects
    void DetectCollision()
    {
        // Combine attack layer with default layer for collision detection
        int layerMask = attackLayer | LayerMask.GetMask("Default");

        // Check for overlapping colliders within a sphere around the arrow's position
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f, layerMask);

        // If there are hits, handle each one
        if (hits.Length > 0)
        {
            foreach (Collider hit in hits)
            {
                // Try to get the Character component from the hit object
                Character hitCharacter;
                hit.gameObject.TryGetComponent<Character>(out hitCharacter);

                // If the hit object has a Character component
                if (hitCharacter != null)
                {
                    // If the hit character is not the owner of the arrow, apply damage
                    if (hitCharacter == owner) { continue; }
                    hitCharacter.Hurt(damage);
                }

                // Destroy the arrow after hitting a target
                Destroy(gameObject);
            }
        }
    }

    // Pause the arrow
    void IPauseable.Pause()
    {
        paused = true;
    }

    // Resume the arrow
    void IPauseable.Play()
    {
        paused = false;
    }

    // Subscribe the arrow to the GameManager's pauseable list
    public void SubscribeToGameManager()
    {
        if (!GameManager.instance.pausables.Contains(this))
        {
            GameManager.instance.pausables.Add(this);
        }
    }
}
