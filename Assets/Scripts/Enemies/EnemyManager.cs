using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Spawn Position Helpers
    public float minSpawnDistance = 70f;
    public float maxSpawnDistance = 150f;
    public GameObject playerObject;
    private Vector3 playerPos;
    
    // Define size of Waves
    public int enemyGroupCount = 3;
    public int enemyGroupSize = 2;
    public int enemyGroupRadius = 10;

    // Counters
    public int desiredEnemyCount = 6; // should be set by difficulty
    public int desiredWaveCounter = 6; // should be set by difficulty
    int enemyCount = 0;
    int waveCounter = 6; // After you kill x amount of enemies, a big wave will spawn

    public static EnemyManager instance;

    public GameObject[] enemyPrefabs; // assigned in editor
    public GameObject cargoshipPrefab; // assigned in editor

    void Awake() {
        instance = this;
    }

    void Start() {

        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerPos = playerObject.transform.position;
        // Spawn initial Wave
        SpawnEnemiesInRandomCircle(playerPos, enemyPrefabs[0], minSpawnDistance, maxSpawnDistance, enemyGroupCount, enemyGroupSize, enemyGroupRadius);
        //SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(250, 250), enemyGroupCount, enemyGroupSize, enemyGroupRadius);
        
        //THIS BLOCKCOMMENT DISABLES THE CARGO SHIP
        /*
        SpawnSingleEnemyAround(playerPos, cargoshipPrefab, 100f, 100f);
        GameObject cargoShip = FindObjectsOfType<CargoshipBehaviour>()[0].gameObject;
        SpawnEnemiesInRandomCircle(cargoshipPrefab.transform.position, enemyPrefabs[1], 10, 20, 1, 5, 0);
        //SpawnSingleCargoshipAt(cargoSpawn);
        */
    }

    void Update()
    {
        // Spawn enemies one by one outside of Waves
        if(enemyCount < desiredEnemyCount)
        {
            Debug.Log("Checking works");
            SpawnSingleEnemyAround(playerObject.transform.position, enemyPrefabs[0], minSpawnDistance, maxSpawnDistance);
        }

        // When waveCounter hits 0, spawn Wave
        if (waveCounter == 0)
        {
            waveCounter = desiredWaveCounter;
            SpawnEnemiesInRandomCircle(playerObject.transform.position, enemyPrefabs[0], 50f, 70f, enemyGroupCount, enemyGroupSize, enemyGroupRadius);
        }

    }

    public Vector3 GetRandomOnNavmesh(Vector3 centerPosition, float minDistance = 50f, float maxDistance = 150f) {
        //generate initial position
        Vector2 centerPosition2d = new Vector2(centerPosition.x, centerPosition.z);
        Vector2 initPosition = centerPosition2d + Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);

        //move onto navmesh
        Vector3 navPosition = new Vector3(initPosition.x, 0, initPosition.y);
        if (UnityEngine.AI.NavMesh.SamplePosition(new Vector3(initPosition.x, 0, initPosition.y), out UnityEngine.AI.NavMeshHit navHit, 100, UnityEngine.AI.NavMesh.AllAreas))
        {
            navPosition = new Vector3(navHit.position.x, 0, navHit.position.z);
        }
        return navPosition;
    }
    public GameObject SpawnSingleEnemyAround(Vector3 centerPosition, GameObject usedPrefab, float minDistance = 50f, float maxDistance = 150f) {
        Debug.Log("Called Spawn");
        Vector3 enemyPosition = GetRandomOnNavmesh(centerPosition, minDistance, maxDistance);
        Debug.Log("got Position");
        //Instantiate
        GameObject enemyObject = Instantiate(usedPrefab, enemyPosition, Quaternion.identity);
        Debug.Log("Called Instantiate");
        //Check if actually on navmesh
        if (!enemyObject.GetComponent<EnemyBehaviour>().agent.isOnNavMesh)
        {
            Destroy(enemyObject);
            return null;
        }
        else
        {
            enemyObject.GetComponent<EnemyBehaviour>().OnDeath += TestMessage;
            enemyObject.GetComponent<EnemyBehaviour>().OnDeath += ReduceEnemyCount;
            enemyCount += 1;
            return enemyObject;
        }
    }

    public void TestMessage(){
        Debug.Log("u workin?");
    }
    public void SpawnEnemiesInRandomCircle(Vector3 originalCenterPosition, GameObject usedPrefab, float originalMinDistance = 50f, float originalMaxDistance = 150f, int groupAmount = 10, int enemyPerGroup = 1, float groupRadius = 10)
    {
        for (int i = 0; i < groupAmount; i++) {
            Vector3 navPosition = GetRandomOnNavmesh(originalCenterPosition, originalMinDistance, originalMaxDistance);

            for (int j = 0; j < enemyPerGroup; j++)
            {
                SpawnSingleEnemyAround(navPosition, usedPrefab, minDistance:0, maxDistance:groupRadius);
            }
        }
    }
    void ReduceEnemyCount()
    {
        Debug.Log("reduced");
        enemyCount -= 1;
        waveCounter -= 1;

    }
}