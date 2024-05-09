using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    private Sprite[] backgrounds;
    private Image background;
    public float parralaxSpeed = 1.5f;
    private Vector2 camPosLastFrame;
    void Awake()
    {
        backgrounds = Resources.LoadAll<Sprite>("Backgrounds");
        background = GetComponentInChildren<Image>();
    }
    private void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
    public void SetBackground(int index)
    {
        background.sprite = backgrounds[index];
    }
    private void FixedUpdate()
    {
        if (camPosLastFrame != Vector2.zero)
        {
            background.transform.position -= ((Vector3)((Vector2)Camera.main.transform.position - camPosLastFrame)* parralaxSpeed);
        }
        camPosLastFrame = Camera.main.transform.position;
    }
}
