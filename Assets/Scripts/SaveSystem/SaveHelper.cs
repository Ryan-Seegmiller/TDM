using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveHelper : MonoBehaviour
{
    [SerializeField] bool showdeaths;
    DeathData[] hotspots;


    [ContextMenu("Death")]
    void TestLogDeath()
    {
        Vector2 point = Random.insideUnitCircle * Random.Range(0, 10f);
        SaveSystem.LogGameFail(new DeathData( Random.Range(4,34), new Vector3(point.x,point.y,0)));

    }

    [ContextMenu("ReadDeathLog")]
    void ReadLog()
    {
        DeathData[] deaths = SaveSystem.LoadDeathData();
        if (deaths.Length == 0 || deaths == null)
        {
            Debug.Log("no log found");
        }
        else
        {
            for(int i = 0; i < deaths.Length; i++)
            {
                Debug.Log($"Log {i}: PlayerType {deaths[i].playertype} died at location  X:{deaths[i].X} Y:{deaths[i].Y}" );
            }
        }
        hotspots = deaths;

    }

    private void OnDrawGizmos()
    {
        if (showdeaths)
        {
            Gizmos.color = new Color(1, 0, 0, .2f);
            foreach (DeathData death in hotspots)
            {
                Gizmos.DrawSphere(new Vector3(death.X, death.Y, 0), 1f);
            }
        }
    }

}
