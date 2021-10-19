using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimator : StateHolder
{
    public MovingDirectionType directionType;
    public float rotationSpeed = 90f;
    public float speed = 20f;
    public float lifetime = 0;

    private Vector3 movingDir;

    // Start is called before the first frame update
    void Awake()
    {
        if(Random.Range(0,2) == 1)
        {
            rotationSpeed *= -1;
        }
        if (directionType == MovingDirectionType.FromParent)
        {
            movingDir = transform.localPosition;
        }
        else
        {
            movingDir = Random.onUnitSphere;
            movingDir.z = 0;
        }
    }

    protected override void onActiveChange(bool value)
    {
        base.onActiveChange(value);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
            transform.position += movingDir * speed * Time.deltaTime;
        }
    }

    public enum MovingDirectionType
    {
        FromParent,
        Random,
    }
}
