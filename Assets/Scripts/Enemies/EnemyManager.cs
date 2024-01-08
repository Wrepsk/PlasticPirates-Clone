using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Define size of Waves
    public int enemyGroupCount = 3;
    public int enemyGroupSize = 2;
    public int enemyGroupRadius = 10;

    // Counters
    public int desiredEnemyCount = 3; // should be set by difficulty
    public int desiredWaveCounter = 6; // should be set by difficulty
    int enemyCount = 0;
    int waveCounter = 6; // After you kill x amount of enemies, a big wave will spawn

    public static EnemyManager instance;

    public GameObject enemyPrefab; // assigned in editor

    void Awake() {
        instance = this;
    }

    void Start() {
        // Spawn initial Wave
        SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(250, 250), enemyGroupCount, enemyGroupSize, enemyGroupRadius);
    }

    void Update()
    {
        // Spawn enemies one by one outside of Waves
        if(enemyCount < desiredEnemyCount)
        {
            SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(512, 512), 1, 1, enemyGroupRadius);
        }

        // When waveCounter hits 0, spawn Wave
        if (waveCounter == 0)
        {
            waveCounter = desiredWaveCounter;
            SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(250, 250), enemyGroupCount, enemyGroupSize, enemyGroupRadius);
        }

    }

    public void SpawnRandomEnemyWithinArea(Vector2 center, Vector2 size, int groupAmount = 10, int enemyPerGroup = 1, float groupRadius = 10) {
        for (int i = 0; i < groupAmount; i ++) {
            float centerX = UnityEngine.Random.Range(0f, size.x);
            float centerY = UnityEngine.Random.Range(0f, size.y);

            for (int j = 0; j < enemyPerGroup; j++) {
                float offsetX = UnityEngine.Random.Range(-groupRadius / 2, groupRadius / 2);
                float offsetY = UnityEngine.Random.Range(-groupRadius / 2, groupRadius / 2);

                float enemyX = centerX + offsetX;
                float enemyY = centerY + offsetY;
                
                
                enemyX = Mathf.Clamp(enemyX, 0, size.x);
                enemyY = Mathf.Clamp(enemyY, 0, size.y);

                Vector2 spawnPosition = center - size / 2 + new Vector2(enemyX, enemyY);

                //  -- TODO: Optimise this --
                Terrain currentTerrain = TerrainManager.instance
                    .GetClosestCurrentTerrain(new Vector3(spawnPosition.x, 0, spawnPosition.y));

                float terrainHeightAtLocation = currentTerrain
                    .SampleHeight(new Vector3(spawnPosition.x, 0, spawnPosition.y));

                Debug.Log(spawnPosition + " " + terrainHeightAtLocation + " " + currentTerrain.name);

                if (terrainHeightAtLocation > 10)
                {
                    Debug.Log("Enemy within island, skipping: " + terrainHeightAtLocation);
                    j--;
                    continue;
                }
                // --------------------------

                SpawnSingleEnemyAt(spawnPosition);
            }

        }
    }

/*
    public void SpawnBulkEnemyWithinRadius(Vector2 location, float radius, float gap) {
        for (float radi = 0; radi <= radius; radi += gap) {
            float perimeter = 2 * Mathf.PI * radi;

            for (float i = 0; i < perimeter; i += gap) {
                float angle = ( i / perimeter ) * 360; // angle in degrees

                Vector2 displacement = Quaternion.Euler(0, 0, angle) * new Vector2(radi, 0);

                GameObject enemyObject = SpawnSingleEnemyAt(location + displacement);
            } 
        }
    }
*/
    void ReduceEnemyCount()
    {
        enemyCount -= 1;
        waveCounter -= 1;

    }

    public GameObject SpawnSingleEnemyAt(Vector2 location) {
        Debug.Log(location);

        GameObject enemyObject = Instantiate(enemyPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);
        enemyObject.GetComponent<EnemyBehaviour>().OnDeath += ReduceEnemyCount;
        enemyCount += 1;

        return enemyObject;
    }

    /*
    public void RemoveAllEnemy() {
        Enemy[] allEnemy = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in allEnemy)
        {
            Destroy(enemy.gameObject);
        }

    }
    */
}