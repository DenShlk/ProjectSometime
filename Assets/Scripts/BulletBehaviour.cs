using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : StateHolder
{
    public float damage = 10f;
    public PlayerBehaviour Player;

    public override void CopyTo(GameObject other)
    {
        BulletBehaviour otherState = CreateIfNotExist<BulletBehaviour>(other);

        base.CopyTo(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.gameObject.GetComponent<EnemyBehaviour>();
        if (enemy != null)
        {
            enemy.OnDamaged(damage);

            //todo !!!
            if(GlobalClock.TimeDirection == 0)
            {
                Player.Score += 3 * enemy.PointsForDefeat; //frozing bonus
            }
            else
            {
                Player.Score += enemy.PointsForDefeat;
            }

            IsDestroyed = true;
        }
        else
        {
            var player = collision.gameObject.GetComponent<PlayerBehaviour>();
            if (player == null)
            {
                IsDestroyed = true;
            }
        }
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

    private void Update()
    {
        if (IsActive)
        {
            if(Mathf.Abs(transform.position.x) > MapParams.mapWidth ||
                Mathf.Abs(transform.position.y) > MapParams.mapHeight)
            {
                IsDestroyed = true;
            }
        }
    }
}
