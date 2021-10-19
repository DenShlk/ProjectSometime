using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapBorderSetter : MonoBehaviour
{
    public Vector2 borderDeltas;

    private void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            transform.position = new Vector3(MapParams.mapWidth * borderDeltas.x, MapParams.mapHeight * borderDeltas.y, 0);
            var scale = Vector3.one;

            // set x in both cases because of rotation, +1 to make square instersection
            if(Mathf.Abs(borderDeltas.x) > Mathf.Abs(borderDeltas.y))
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
                scale.x = MapParams.mapHeight*2 + 1;
            }
            else
            {
                scale.x = MapParams.mapWidth*2 + 1;
            }
            transform.localScale = scale;
        }
    }
}
