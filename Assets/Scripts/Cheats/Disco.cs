using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disco : MonoBehaviour
{
    Character player = null;

    private void Start()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }
}
