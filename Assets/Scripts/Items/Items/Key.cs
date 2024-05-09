using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    ///Distance for door accessiblity
    public float keyUseDistance = 2f;

    ///Opens nearby doors
    public override bool Use()
    {
        bool doorInArea = false;

        if (GameManager.instance.player.doorInArea)
        {
            doorInArea = true;
            GameManager.instance.player.DoorInRange().OpenDoor();
        }


        return doorInArea;
    }
}
