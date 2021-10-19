using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : StateHolder
{
    [SerializeField] private int maxLifes = 5;
    [SerializeField] private GameObject damagedVFX;

    public int Score;
    public int lifes { get; private set; }

    private void Awake()
    {
        lifes = maxLifes;
    }

    public void OnDamaged(EnemyBehaviour enemy)
    {
        if (GlobalClock.TimeDirection == 0)
            return;//immortal when time stops

        enemy.OnDamaged(1000);
        lifes--;

        Instantiate(damagedVFX, transform.position, transform.rotation, transform.parent);

        if(lifes <= 0)
        {
            for (int i = 0; i < 10; i++)
            {
                Instantiate(damagedVFX, transform.position, transform.rotation, transform.parent);
            }
            IsActive = false;

            ScoreManager.AddScore(Score);
            StartCoroutine(WaitAndLoadMenu());
        }
    }

    private IEnumerator WaitAndLoadMenu()
    {
        yield return new WaitForSeconds(5f);
        if (lifes <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    public override void CopyTo(GameObject other)
    {
        PlayerBehaviour otherState = CreateIfNotExist<PlayerBehaviour>(other);

        otherState.maxLifes = maxLifes;
        otherState.lifes = lifes;
        otherState.Score = Score;

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
}
