using UnityEngine;
using UnityEngine.SceneManagement;

public static class ParticleManager
{
    //Instead of this obejct and all its emitters persisting between scenes, it will resetup and respawn all the emitters on scene change.
    // Array to store references to particle emitters
    private static ParticleSystem[] emitters;
    // Flag to indicate if the ParticleManager is set up
    public static bool isSetup = false;

    // Method to set up the ParticleManager
    public static void Setup()
    {
        // Subscribe to the activeSceneChanged event to perform cleanup
        SceneManager.activeSceneChanged += UnSetup;

        // Mark setup as complete
        isSetup = true;

        // Load all particle prefabs from the "Particles/" folder
        GameObject[] allParticles = Resources.LoadAll<GameObject>("Particles/");
        emitters = new ParticleSystem[allParticles.Length];

        // Instantiate particle systems
        for (int i = 0; i < allParticles.Length; i++)
        {
            emitters[i] = GameObject.Instantiate(allParticles[i], Vector3.zero, Quaternion.identity, null).GetComponent<ParticleSystem>();
            emitters[i].transform.localEulerAngles = Vector3.zero;
            emitters[i].transform.localPosition = Vector3.zero;

            // Stop the particle system if it's already playing
            if (emitters[i].isPlaying) { emitters[i].Stop(); }
        }
    }

    // Method to play a particle effect at a given index and world position
    public static void PlayParticle(int index, Vector3 worldPos)
    {
        // Ensure setup is complete
        if (!isSetup) { Setup(); }

        // Set the position and play the particle system
        emitters[index].transform.position = worldPos;
        emitters[index].Play();
    }

    // Method called when the active scene changes to reset setup flag
    static void UnSetup(Scene current, Scene next)
    {
        isSetup = false;
    }
}
