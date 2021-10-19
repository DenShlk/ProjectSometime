using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerBehaviour))]
public class InputGrabber : MonoBehaviour
{
    public float speed = 10f;
    public GameObject bulletPrefab;
    public float reloadDelay = 0.2f;

    [Header("Skills")]
    public float timeReverseReloadDelay = 2f;
    public float timeStopReloadDelay = 10f;
    public float timeStopMaxDuration = 0.75f;


    float reloadTimer = 0;
    float timeStopReloadTimer = 0;
    float timeReverseReloadTimer = 0;

    private PlayerBehaviour playerState;
    private void Start()
    {
        playerState = GetComponent<PlayerBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState.IsDestroyed || playerState.lifes <= 0)
            return;


        TimeReverseSkill();
        TimeStopSkill();

        Moving();
        Shooting();
    }

    private bool isShooting = false;
    public void OnShooting(InputAction.CallbackContext value)
    {
        isShooting = value.ReadValueAsButton();
    }

    private void Shooting()
    {
        reloadTimer -= Time.deltaTime;
        if (isShooting && reloadTimer < 0)
        {
            reloadTimer = reloadDelay;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation, transform.parent);

            bullet.GetComponent<BulletBehaviour>().Player = playerState;
        }
    }

    private Vector3 movementDirection;
    public void OnMovementFromMouse(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        
        Ray castPoint = Camera.main.ScreenPointToRay(new Vector3(inputMovement.x, inputMovement.y));

        float zPlane = transform.position.z;

        float koefficient = Mathf.Abs(-castPoint.origin.z + zPlane) / castPoint.direction.z;
        float targetX = castPoint.origin.x + castPoint.direction.x * koefficient;
        float targetY = castPoint.origin.y + castPoint.direction.y * koefficient;

        movementDirection = new Vector3(targetX - transform.position.x, targetY - transform.position.y, 0);
    }

    public void OnMovementFromStick(InputAction.CallbackContext value)
    {
        var input = value.ReadValue<Vector2>();
        movementDirection = new Vector3(input.x, input.y);

        if (movementDirection.magnitude > 0.1f)
            isShooting = true;
    }

    private void Moving()
    {
        if (movementDirection.sqrMagnitude > 1f)
        {
            movementDirection = movementDirection.normalized;
        }
        transform.rotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
        transform.position += movementDirection * speed * Time.deltaTime;
        transform.position = MapParams.ClampPosition(transform.position);
    }

    private bool isTimeStopUsing = false;
    public void OnTimeStopSkill(InputAction.CallbackContext value)
    {
        isTimeStopUsing |= value.started;
    }

    private int timeDirectionBeforeStop = 1;
    private float timeStopDuration = 0;
    private void TimeStopSkill()
    {
        timeStopReloadTimer -= Time.deltaTime;
        if (isTimeStopUsing)
        {
            if (GlobalClock.TimeDirection == 0)
            {
                GlobalClock.TimeDirection = timeDirectionBeforeStop;
            }
            else
            {
                if (timeStopReloadTimer < 0)
                {
                    timeStopReloadTimer = timeStopReloadDelay;
                    timeStopDuration = 0;

                    timeDirectionBeforeStop = GlobalClock.TimeDirection;
                    GlobalClock.TimeDirection = 0;
                }
            }
        }
        isTimeStopUsing = false;

        if (GlobalClock.TimeDirection == 0)
        {
            timeStopDuration += Time.deltaTime;
            if (timeStopDuration > timeStopMaxDuration)
            {
                GlobalClock.TimeDirection = timeDirectionBeforeStop;
            }
        }
    }

    private bool isTimeReverseUsing = false;
    public void OnTimeReverseSkill(InputAction.CallbackContext value)
    {
        isTimeReverseUsing = value.ReadValueAsButton();
    }
    private void TimeReverseSkill()
    {
        timeReverseReloadTimer -= Time.deltaTime;
        if (isTimeReverseUsing)
        {
            if (GlobalClock.TimeDirection == 1)
            {
                if (timeReverseReloadTimer < 0)
                {
                    timeReverseReloadTimer = timeReverseReloadDelay;
                    GlobalClock.TimeDirection = -1;
                }
            }
        }
        else
        {
            if (GlobalClock.TimeDirection == -1)
            {
                GlobalClock.TimeDirection = 1;
            }
        }
    }

    public float GetTimeStopReloadProgress()
    {
        return Mathf.Clamp(1 - timeStopReloadTimer / timeStopReloadDelay, 0, 1);
    }

    public float GetTimeReverseReloadProgress()
    {
        return Mathf.Clamp(1 - timeReverseReloadTimer / timeReverseReloadDelay, 0, 1);
    }
}
