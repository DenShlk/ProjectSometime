using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDelayedDestroyer : StateHolder
{
    public float lifetime = 2f;

    public float lifetimeTimer;
    private void Start()
    {
        lifetimeTimer = lifetime;
    }

    public override void CopyTo(GameObject other)
    {
        var otherState = CreateIfNotExist<VFXDelayedDestroyer>(other);

        otherState.lifetimeTimer = lifetimeTimer;

        base.CopyTo(other);
    }

    private void Update()
    {
        if (IsActive)
        {
            lifetimeTimer -= Time.deltaTime;
            if(lifetimeTimer < 0)
            {
                IsDestroyed = true;
            }
        }
    }
}
