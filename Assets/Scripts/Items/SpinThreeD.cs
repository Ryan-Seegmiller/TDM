using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinThreeD : MonoBehaviour
{
    ///Self reference
    Transform target;
    ///Tracking to stop when disabled
    Coroutine currentRoutine;
    ///Rotation applied every frame
    Vector3 rotationAmount = new Vector3(0, 1, 0);

    void Awake()
    {
        target = transform;
    }
    ///Begin spinning
    private void OnEnable()
    {
        if(target != null)
        {
            currentRoutine = StartCoroutine(SpinInPlace());
        }
    }
    ///Stop spinning
    private void OnDisable()
    {
        StopCoroutine(currentRoutine);
    }
    ///Spin every frame
    IEnumerator SpinInPlace()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            transform.Rotate(rotationAmount);
        }
    }
}
