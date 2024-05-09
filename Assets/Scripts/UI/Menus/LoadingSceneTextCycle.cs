using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LoadingSceneTextCycle : MonoBehaviour
{
    public string[] cycle = new string[2];
    public float frameDuration = 0.5f;

    TextMeshProUGUI textDisplay;
    Coroutine currentRoutine;

    void Awake()
    {
        textDisplay = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        //Double check it's running
        if (currentRoutine == null) { currentRoutine = StartCoroutine(Cycle()); }
    }

    private void OnEnable()
    {
        currentRoutine = StartCoroutine(Cycle());
    }

    private void OnDisable()
    {
        StopCoroutine(currentRoutine);
    }

    IEnumerator Cycle()
    {
        if (textDisplay == null) { textDisplay = GetComponent<TextMeshProUGUI>(); }
        int index = 0;
        while (true)
        {
            textDisplay.text = cycle[index];
            yield return new WaitForSeconds(frameDuration);
            index++;
            if (index >= cycle.Length) { index = 0; }
        }
    }
}
