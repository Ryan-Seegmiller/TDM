using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class EventSystemTest : MonoBehaviour
{
   public InputActionAsset actions;
    public InputSystemUIInputModule input;


    private void Start()
    {
        input.actionsAsset = actions;
    }
    private void Update()
    {
        if(input.actionsAsset != actions)
        {
            input.actionsAsset = actions;
        }
    }
}
