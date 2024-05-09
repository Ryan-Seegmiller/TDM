using UnityEngine;

public static class AudioManager
{
    // Arrays to store different types of audio clips
    private static AudioClip[] UI_SFX = new AudioClip[0]; // Group index 0
    private static AudioClip[] melee_SFX = new AudioClip[0]; // Group index 1
    private static AudioClip[] magic_SFX = new AudioClip[0];
    private static AudioClip[] interaction_SFX = new AudioClip[0];
    private static AudioClip[] music = new AudioClip[0];
    private static AudioClip[] explosions = new AudioClip[0];
    private static AudioClip[] ranged_SFX = new AudioClip[0];
    // Audio sources for different purposes
    private static AudioSource UISFX_Emitter;
    private static AudioSource musicEmitter;
    private static AudioSource[] emitters = new AudioSource[3];
    // 2D array to store all audio clip groups
    private static AudioClip[][] allAudioClipGroups = new AudioClip[6][];
    private static bool isSetup = false;
    // Play a UI sound effect at a specified index
    public static void PlaySound_UI_SFX(int index)
    {
        CheckEmitters();
        UISFX_Emitter.PlayOneShot(UI_SFX[index]);
    }

    // Play a sound from a specific audio group and clip index
    public static void PlaySound(int groupIndex, int clipIndex)
    {
        CheckEmitters();
        GetFreeEmitter().PlayOneShot(allAudioClipGroups[groupIndex][clipIndex]);
    }

    // Play a sound from a specific audio group and clip index at a specified world position
    public static void PlaySound(int groupIndex, int clipIndex, Vector3 worldPos)
    {
        CheckEmitters();
        AudioSource aS = GetFreeEmitter();
        aS.transform.position = worldPos;
        aS.PlayOneShot(allAudioClipGroups[groupIndex][clipIndex]);
    }


    private static AudioSource GetFreeEmitter()
    {
        CheckEmitters();
        AudioSource selected = emitters[0];
        for (int i = 0; i < emitters.Length; i++)
        {
            if (!emitters[i].isPlaying) { selected = emitters[i]; break; }
        }
        return selected;
    }

    public static void ChangeMusicVolume(float c) // Scale of 0-1
    {
        CheckEmitters();
        musicEmitter.volume = Mathf.Clamp01(c);
    }

    public static void ChangeUISFX_Volume(float c) // Scale of 0-1
    {
        CheckEmitters();
        UISFX_Emitter.volume = Mathf.Clamp01(c);
    }

    // Setup audio by loading audio clips and setting up arrays
    public static void Setup()
    {
        if (isSetup) { Debug.Log("already setup"); return; }//Ensure has not been setup prior
        //Make sure all references are clear - this is a doublecheck
    
        if (musicEmitter != null) { GameObject.Destroy(musicEmitter.gameObject); }
        if (UISFX_Emitter != null) { GameObject.Destroy(UISFX_Emitter.gameObject); }
        for (int i = 0; i < emitters.Length; i++)
        {
            if (emitters[i] != null) { GameObject.Destroy(emitters[i].gameObject); }
        }
       
        UI_SFX = Resources.LoadAll<AudioClip>("Audio/UI_SFX");
        melee_SFX = Resources.LoadAll<AudioClip>("Audio/Melee_SFX");
        magic_SFX = Resources.LoadAll<AudioClip>("Audio/Magic_SFX");
        interaction_SFX = Resources.LoadAll<AudioClip>("Audio/Interaction_SFX");
        music = Resources.LoadAll<AudioClip>("Audio/Music");
        explosions = Resources.LoadAll<AudioClip>("Audio/Explosions");
        ranged_SFX = Resources.LoadAll<AudioClip>("Audio/Ranged_SFX");
        GameObject uiSFX_emitterReference = Resources.Load<GameObject>("AudioManager/UISFX_Emitter");
        GameObject emitterReference = Resources.Load<GameObject>("AudioManager/Emitter");
        GameObject musicEmitterReference = Resources.Load<GameObject>("AudioManager/MusicEmitter");
        // Set up audio clip indices
        allAudioClipGroups[0] = UI_SFX;
        allAudioClipGroups[1] = melee_SFX;
        allAudioClipGroups[2] = magic_SFX;
        allAudioClipGroups[3] = interaction_SFX;
        allAudioClipGroups[4] = explosions;
        allAudioClipGroups[5] = ranged_SFX;

        //Initialize music emitter
        GameObject musicEmitterGO = Object.Instantiate(musicEmitterReference, Vector3.zero, Quaternion.identity) as GameObject;
        musicEmitter = musicEmitterGO.GetComponent<AudioSource>();
        musicEmitter.gameObject.name = "MusicEmitter";
        //Initialize UISFX emitter
        GameObject uiSFX_emitterInstance = Object.Instantiate(uiSFX_emitterReference, Vector3.zero, Quaternion.identity) as GameObject;
        UISFX_Emitter = uiSFX_emitterInstance.GetComponent<AudioSource>();
        UISFX_Emitter.gameObject.name = "UISFX_Emitter";
        // Initialize pooled audio emitters
        emitters = new AudioSource[3];
        for (int i = 0; i < emitters.Length; i++)
        {
            GameObject tempGO = Object.Instantiate(emitterReference, Vector3.zero, Quaternion.identity) as GameObject;
            emitters[i] = tempGO.GetComponent<AudioSource>();
            emitters[i].gameObject.name = "AudioEmitter";
        }

        // Set initial music clip and play
        musicEmitter.clip = music[Random.Range(0, music.Length)];
        musicEmitter.Play();
        musicEmitter.loop = true;
        isSetup = true;
        Debug.Log(isSetup);
    }

    // Set emitters to 3D or 2D mode
    public static void SetEmitters3D(bool is3D)
    {
        CheckEmitters();
        for (int i = 0; i < emitters.Length; i++)
        {
            emitters[i].spatialBlend = is3D ? 1 : 0;
        }
    }
    private static void CheckEmitters()
    {
        if (!isSetup) { Setup(); return; }//If it hasnt been setup yet, do that
        //If its been setup before, but emitters were lost via scene change, check for that here, and resetup 
        bool emitterDead=false;
        for (int i = 0; i < emitters.Length; i++)
        {
            if (emitters[i] == null) { emitterDead = true; }
        }
        if (musicEmitter == null || UISFX_Emitter == null || emitterDead) { isSetup = false; Setup(); }
    }
    public static void ManualSetup()
    {
        isSetup = false;
        Setup();
    }
}
