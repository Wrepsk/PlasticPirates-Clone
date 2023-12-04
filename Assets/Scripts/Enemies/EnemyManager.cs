using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int nEnemyGroups = 10;
    public int sizeEnemyGroups = 1;
    public int radiusEnemyGroups = 10;

    public static EnemyManager instance;

    public GameObject enemyPrefab; // assigned in editor

    void Awake() {
        instance = this;
    }

    void Start() {
        // uhhhh ... comment this for now, since NavMesh is created by TerrainManager atm
        // and if this function runs before TerrainManager.Start() we're doomed
        // we can spawn in Update because it is called later
        // i promise i will fix it with a new GameManager object
        // v v v
        // SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(250, 250), nEnemyGroups, sizeEnemyGroups, radiusEnemyGroups);
    }

    void Update()
    {
        EnemyBehaviour[] enemies = FindObjectsOfType<EnemyBehaviour>();
        if(enemies.Length == 0)
        {
            SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(512, 512), nEnemyGroups, sizeEnemyGroups, radiusEnemyGroups);
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

                // float terrainHeightAtLocation = Terrain.activeTerrain.SampleHeight(new Vector3(spawnPosition.x, 0, spawnPosition.y));
                // if (terrainHeightAtLocation > 18) {
                //     Debug.Log("Enemy within island, skipping");
                // } else {
                //     Debug.Log(terrainHeightAtLocation);
                // }

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
    public GameObject SpawnSingleEnemyAt(Vector2 location) {
        Debug.Log(location);


        GameObject enemyObject = Instantiate(enemyPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);

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