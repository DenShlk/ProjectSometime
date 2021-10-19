using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Transform objectToAvoid;
    public float avoidingDistance = 2f;
    public float spawnDelay = 5f;
    public bool spawnNearby = false;
    public bool attachAsChild = true;
    public float spawnCircleRadius = 1f;

    private float spawnTimer;

    // Start is called before the first frame update
    private void Awake()
    {
        spawnTimer = spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0)
        {
            Spawn();
        }
    }

    private void Spawn()
    {

        spawnTimer = spawnDelay;
        GameObject enemy = Instantiate(chooseEnemyPrefab());
        if (attachAsChild)
        {
            enemy.transform.parent = transform;
        }
        else
        {
            enemy.transform.parent = transform.parent;
        }
        enemy.transform.localPosition = generateEnemyPos();
    }

    private Vector3 generateEnemyPos()
    {
        if (spawnNearby)
        {
            var vec2 = Random.insideUnitCircle * spawnCircleRadius;
            return transform.position + new Vector3(vec2.x, vec2.y);
        }

        float randX = Random.Range(-MapParams.mapWidth, MapParams.mapWidth);
        float randY = Random.Range(-MapParams.mapHeight, MapParams.mapHeight);

        var result = new Vector3(randX, randY, 0);

        if (objectToAvoid)
        {
            if (Vector3.Distance(result, objectToAvoid.position) < avoidingDistance)
                return generateEnemyPos();
        }

        return result;
    }

    private GameObject chooseEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }
}
