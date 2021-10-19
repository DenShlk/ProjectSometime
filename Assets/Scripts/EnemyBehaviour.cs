using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : StateHolder
{
    public int PointsForDefeat = 100;
    [SerializeField] private float health = 5f;
    [SerializeField] private GameObject deathVFXPrefab;

    public override void CopyTo(GameObject other)
    {
        EnemyBehaviour otherState = CreateIfNotExist<EnemyBehaviour>(other);

        otherState.health = health;

        base.CopyTo(other);
    }

    protected override void onActiveChange(bool value)
    {
        base.onActiveChange(value);

        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = value;
        }
    }


    public void OnDamaged(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathVFXPrefab, transform.position, Quaternion.identity, transform.parent);

        IsDestroyed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerBehaviour>();
        if(player != null)
        {
            player.OnDamaged(this);
        }
    }
}
