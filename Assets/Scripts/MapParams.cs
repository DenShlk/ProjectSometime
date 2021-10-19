using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapParams
{
    public static float mapWidth = 25f;
    public static float mapHeight = 10f;

    public static Vector3 ClampPosition(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, -mapWidth + 1, mapWidth - 1);
        pos.y = Mathf.Clamp(pos.y, -mapHeight + 1, mapHeight - 1);
        return pos;
    }
}
