using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowItem : Item 
{
    public int arrowAmount = 1;
    public override bool Use()
    {
        int totalAmount = arrowAmount = amount;
        return true;
        //TODO: Add functionality
    }
}
