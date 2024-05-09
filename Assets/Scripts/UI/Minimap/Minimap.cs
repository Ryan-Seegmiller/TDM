using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public static Minimap instance;
    Vector2 LEVELSIZE = new Vector2(45, 35);
    Vector2 MINIMAPSIZE = new Vector2(450, 350);
    Vector2 OFFSET = new Vector2(0, 2);

    public Transform player;
    public RectTransform playerIcon;
    public RectTransform marker;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    private void Update()
    {
        if(player != null)
        {
            playerIcon.anchoredPosition = MinimapPositionFromWorld(player.position, OFFSET);
        }
    }

    private static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    public void SetMarker(Vector3 worldPosition)
    {
        marker.anchoredPosition = MinimapPositionFromWorld(worldPosition, new Vector2(0,-1));
        if (!marker.gameObject.activeInHierarchy) { marker.gameObject.SetActive(true); }
    }
    public void ClearMarker()
    {
        if (marker.gameObject.activeInHierarchy) { marker.gameObject.SetActive(true); }
    }
    Vector3 MinimapPositionFromWorld(Vector3 worldPosition, Vector2 offset)
    {
        return new Vector3(map(worldPosition.x - offset.x, 0, LEVELSIZE.x, 0, MINIMAPSIZE.x),
            map(worldPosition.y - offset.y, 0, LEVELSIZE.y, 0, MINIMAPSIZE.y),
            0);
    }
}
