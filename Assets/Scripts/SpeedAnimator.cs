using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAnimator : MonoBehaviour
{
    public float speed = 10f;
    

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += transform.up * speed * Time.deltaTime;
    }
}
