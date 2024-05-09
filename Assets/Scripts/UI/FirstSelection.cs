using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Will select this object upon being enabled for console
/// </summary>
public class FirstSelection : MonoBehaviour
{
    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == null) { Select(); }
    }
    private void OnEnable()
    {
        Select();
    }
    public void Select()
    {
        if(ControllerManager.instance != null && ControllerManager.instance.CONTROLLERENABLED)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        else if (Application.isConsolePlatform)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
