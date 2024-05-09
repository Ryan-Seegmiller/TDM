using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    ///Singleton pattern
    public static LoadingScreen instance { get; private set; }
    ///The Loading bar
    [SerializeField] Image progressBar;

    private void Awake()
    {
        //Singleton
        if(instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// Sets progress bar based on a float 0 to 1
    /// </summary>
    /// <param name="percent"></param>
    public void UpdateProgress(float percent)
    {
        progressBar.fillAmount = percent;
    }
    ///Active game object toggle
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
