using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DisplayBuildNumber : MonoBehaviour
{
    GUIStyle style = new GUIStyle();
    private void Awake()
    {
        style.normal.textColor = Color.cyan;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 26;
    }

    private void OnGUI()
    {
       
   
        string s = $"{Application.platform} version: {Application.version} build: {Application.buildGUID}";
        GUI.Label(new Rect(10f, 5f, Screen.width - 20f, 20f), s, style);
        
    }

}
