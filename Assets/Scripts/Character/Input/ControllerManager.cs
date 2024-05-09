using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    // Singleton instance of the ControllerManager
    public static ControllerManager instance { get; private set; }
    // Flag to indicate whether a controller is enabled
    public bool CONTROLLERENABLED = false;

    // Awake method called when the script instance is being loaded
    private void Awake()
    {
        // Singleton pattern implementation
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        // Setup callbacks for input device changes
        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    ControllerManager.instance.EnableController();
                    break;
                case InputDeviceChange.Disconnected:
                    ControllerManager.instance.DisableController();
                    break;
                case InputDeviceChange.Reconnected:
                    ControllerManager.instance.EnableController();
                    break;
                default:
                    // See InputDeviceChange reference for other event types.
                    break;
            }
        };
    }

    // Start method called on the frame when a script is enabled
    private void Start()
    {
        // Check if there are any gamepads connected and enable the controller if so
        if (Gamepad.all.Count > 0) EnableController();
        else DisableController();
    }

    // Method to enable controller input
    void EnableController()
    {
        CONTROLLERENABLED = true;
        ControlsIconCanvasUpdate();
    }

    // Method to disable controller input
    void DisableController()
    {
        CONTROLLERENABLED = false;
        ControlsIconCanvasUpdate();
    }

    // Method to update controls icon canvas
    void ControlsIconCanvasUpdate()
    {
        // Update controls icon canvas across all canvases
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].BroadcastMessage("OnInputChange", SendMessageOptions.DontRequireReceiver);
        }
    }
}
