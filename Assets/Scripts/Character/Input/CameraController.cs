using UnityEngine;

public class CameraController : MonoBehaviour, IPauseable
{
    // Singleton instance of the CameraController
    public static CameraController instance { get; private set; }

    // Reference to the camera mount
    public GameObject cameraMount;

    // Camera component
    Camera cam;

    // Reference to CharacterInput script
    CharacterInput cInputs;

    // Layer mask for wall checking
    public LayerMask wallCheckMask;

    // Sensitivity properties for X and Y axes
    public float sensitivity_x
    {
        get { return _sensitivity_x; }
        set { _sensitivity_x = value; if (lockSensXY && sensitivity_y != value) { sensitivity_y = value; } }
    }
    public float sensitivity_y
    {
        get { return _sensitivity_y; }
        set { _sensitivity_y = value; if (lockSensXY && sensitivity_x != value) { sensitivity_x = value; } }
    }

    // Combined sensitivity property
    public float SENSITIVITY
    {
        set
        {
            sensitivity_x = value;
            sensitivity_y = value;
        }
        get { return (sensitivity_x + sensitivity_y) / 2; }//PEMDAS!
    }

    // Sensitivity variables
    private float _sensitivity_x;
    private float _sensitivity_y;

    // Flag to lock sensitivity for X and Y axes
    private bool lockSensXY = true;

    // Invert flags for X and Y axes
    public bool invertX = false;
    public bool invertY = false;

    // Base field of view (FOV)
    public int BASEFOV = 60;

    // Smooth FOV change rate
    private float FOV_ChangeRate = 0.2f;

    // Target FOV
    public float targetFOV;

    // Flag to enable/disable mouse look
    public bool lookEnabled = true;

    // Maximum angle for X-axis clamping
    private const int XCLAMPANGLE = 90;

    // Camera offsets
    private Vector3 camOffset_3D = new Vector3(1f, 0f, -3f);
    private Vector3 camMountOffset_3D = new Vector3(0, 1.2f, 0);

    // Current X and Y rotation angles
    private float xAxis = 0;
    private float yAxis = 0;

    private void Awake()
    {
        // Unlock sensitivity
        lockSensXY = false;

        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        // Get the main camera
        cam = Camera.main;
        // Reset camera position and rotation
        ResetCamera();
        // Subscribe to GameManager for pausing functionality
        SubscribeToGameManager();
        // Get CharacterInput component
        cInputs = GetComponent<CharacterInput>();
        // Set default sensitivity
        SENSITIVITY = 30;
        if(ControllerManager.instance.CONTROLLERENABLED || Application.isMobilePlatform) { SENSITIVITY = 20; }
        // Set camera clip planes
        cam.farClipPlane = 500;
        cam.nearClipPlane = 0.2f;
        // Set initial target FOV for smooth transition
        targetFOV = BASEFOV;
        cam.fieldOfView = 40;
        //Subscribe LockMouse(false) to the end game delegate
        GameManager.instance.OnGameEnd += UnlockMouse;
    }

    // Reset camera position and rotation
    public void ResetCamera()
    {
        cam.transform.parent = cameraMount.transform;
        cam.transform.localPosition = camOffset_3D;
        cam.transform.localEulerAngles = Vector3.zero;
        cameraMount.transform.localPosition = camMountOffset_3D;
        cameraMount.transform.localEulerAngles = Vector3.zero;
    }

    void Update()
    {
        // Mouse look
        if (lookEnabled && !RuneManager.instance.modRuneMenuOpen)
        {
            if (cInputs.aimVector.x != 0)
            {
                // Player rotates globally on Y-axis based on mouse X
                float scaledInput = (sensitivity_x * 5) * Time.deltaTime * cInputs.aimVector.x;
                if (invertX) { scaledInput *= -1; }
                yAxis += scaledInput;
                if (yAxis >= 360) { yAxis -= 360; }
                if (yAxis <= 0) { yAxis += 360; }
                transform.rotation = Quaternion.Euler(0, yAxis, 0);
            }
            if (cInputs.aimVector.y != 0)
            {
                // Camera mount rotates locally on X-axis based on mouse Y
                float scaledInput = -((sensitivity_y * 5) * Time.deltaTime * cInputs.aimVector.y);
                if (invertY) { scaledInput *= -1; }
                xAxis = Mathf.Clamp(xAxis + scaledInput, -XCLAMPANGLE, XCLAMPANGLE);
                cameraMount.transform.localRotation = Quaternion.Euler(xAxis, 0, 0);
            }
        }

        // FOV smoothing
        if (cam.fieldOfView != targetFOV)
        {
            float newFov = Mathf.Lerp(cam.fieldOfView, targetFOV, FOV_ChangeRate);
            if (Mathf.Abs(cam.fieldOfView - targetFOV) < 0.5f) { newFov = targetFOV; }
            cam.fieldOfView = newFov;
        }

        //Manual cursor override
        //if (Input.GetKeyDown(KeyCode.LeftBracket)) { LockMouse(false); }
        //if (Input.GetKeyDown(KeyCode.RightBracket)) { LockMouse(true); }
    }

    private void FixedUpdate()
    {
        WallCheck();
    }

    // Pause method from IPauseable interface
    public void Pause()
    {
        lookEnabled = false;
    }

    // Play method from IPauseable interface
    public void Play()
    {
        lookEnabled = true;
    }

    // Subscribe to GameManager for pausing functionality
    public void SubscribeToGameManager()
    {
        if (!GameManager.instance.pausables.Contains(this))
        {
            GameManager.instance.pausables.Add(this);
        }
    }

    // Set target FOV
    public void SetFOV(float newFOV)
    {
        targetFOV = newFOV;
    }

    // Check if camera would collide with walls
    void WallCheck()
    {
        RaycastHit hit;
        if (Physics.Linecast(cameraMount.transform.position, (cameraMount.transform.position + cameraMount.transform.rotation * camOffset_3D), out hit, wallCheckMask))
        {
            cam.transform.position = hit.point;
        }
        else
        {
            cam.transform.localPosition = camOffset_3D;
        }
    }

    // Toggle mouse confinement
    public void LockMouse(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    //Unlocks the mouse (this method is subscribed to the OnGameEnd delegate)
    public void UnlockMouse()
    {
        LockMouse(false);
    }

    // Property from IPauseable interface
    public bool IsPaused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
