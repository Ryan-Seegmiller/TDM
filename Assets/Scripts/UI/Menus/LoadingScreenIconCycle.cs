using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoadingScreenIconCycle : MonoBehaviour
{
    public Sprite[] cycle = new Sprite[2];
    public float frameDuration = 0.2f;

    Image img;
    Coroutine currentRoutine;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    private void Start()
    {
        //Double check it's running
        if(currentRoutine == null) { currentRoutine = StartCoroutine(Cycle()); }
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
        if(img == null) { img = GetComponent<Image>(); }
        int index = 0;
        while (true)
        {
            img.sprite = cycle[index];
            yield return new WaitForSeconds(frameDuration);
            index++;
            if(index >= cycle.Length) { index = 0; }
        }
    }
}
