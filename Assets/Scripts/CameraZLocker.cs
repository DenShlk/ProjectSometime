using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZLocker : MonoBehaviour
{
    public Transform player;
    public float zOffset = -10f;

    private void LateUpdate()
    {
        if (player != null)
        {
            var pos = transform.position;
            pos.z = player.position.z + zOffset;
            transform.position = pos;
        }
    }
}
