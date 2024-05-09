using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowImage : MonoBehaviour
{
    public Color[] colors = new Color[] { Color.red, new Color(1f, 0.5f, 0), Color.yellow, Color.green, Color.cyan, Color.blue, new Color(0.5f, 0, 1f), Color.magenta };

    Coroutine currentRoutine;
    float duration = 1f;
    Image targetImage;

    void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        currentRoutine = StartCoroutine(ColorCycle());
    }

    private void OnDisable()
    {
        StopCoroutine(currentRoutine);
    }

    IEnumerator ColorCycle()
    {
        int index = 0;
        float startTime;
        while (true)
        {
            startTime = Time.time;
            while ((Time.time - startTime) <= duration)
            {
                yield return null;
                if (index == colors.Length - 1) //check for the end of the array
                {
                    targetImage.color = Color.Lerp(colors[index], colors[0], (Time.time - startTime) / duration);
                }
                else { targetImage.color = Color.Lerp(colors[index], colors[index + 1], (Time.time - startTime) / duration); }
            }
            index++;
            if(index >= colors.Length) { index = 0; }
        }

    }
}
