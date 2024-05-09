using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    Slider slider;
    CameraController camController;
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = 100;
        slider.minValue = 1;
        slider.wholeNumbers = true;
        StartCoroutine(LateStart());
    }

    public void ValueChanged(float value)
    {
        if (camController == null) { camController = Camera.main.GetComponentInParent<CameraController>(); }
        camController.SENSITIVITY = value;
        slider.value = value; //Set slider value manually incase this is not called by the slider
        text.text = value.ToString();
        Debug.Log($"Sens changed to {value}");
    }

    IEnumerator LateStart() 
    { 
        yield return new WaitUntil(() => { if (camController == null) { camController = Camera.main.GetComponentInParent<CameraController>(); } return camController != null; });
        camController = Camera.main.GetComponentInParent<CameraController>();
        slider.value = camController.SENSITIVITY;
    }
}
