using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyBehaviour))]
public class EnemyAnimator : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float maxAccelerationModule = 1f;
    public float changeAccelerationDelay = 0.5f;

    private Vector2 velocity;
    private Vector2 acceleration;

    private float changeAccelerationTimer = 0f;


    private void Awake()
    {
        velocity = Random.insideUnitCircle * maxSpeed;
        acceleration = Random.insideUnitCircle * maxAccelerationModule;
    }

    private EnemyBehaviour stateHolder;
    private void Start()
    {
        stateHolder = GetComponent<EnemyBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stateHolder.IsActive)
            return;

        changeAccelerationTimer -= Time.deltaTime;
        if(changeAccelerationTimer < 0)
        {
            changeAccelerationTimer = changeAccelerationDelay;
            acceleration = Random.insideUnitCircle * maxAccelerationModule;
        }

        velocity += acceleration * Time.deltaTime;
        if(velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
            acceleration = Random.insideUnitCircle * maxAccelerationModule;
        }

        Vector3 pos = transform.position;
        pos += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;

        if (Mathf.Abs(pos.x) > MapParams.mapWidth)
        {
            pos.x = Mathf.Sign(pos.x) * MapParams.mapWidth;
            velocity.x *= -1;
        }
        if (Mathf.Abs(pos.y) > MapParams.mapHeight)
        {
            pos.y = Mathf.Sign(pos.y) * MapParams.mapHeight;
            velocity.y *= -1;
        }

        transform.position = pos;

        transform.rotation.SetLookRotation(Vector3.forward, new Vector3(velocity.x, velocity.y, 0));
    }
}
