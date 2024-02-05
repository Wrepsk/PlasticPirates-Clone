using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Spawn Position Helpersor
    public float minSpawnDistance = 80f;
    public float maxSpawnDistance = 150f;
    public GameObject playerObject;
    private Vector3 playerPos;

    // Define size of Waves
    public int enemyGroupCount = 6;
    public int enemyGroupSize = 1;
    public int enemyGroupRadius = 10;

    // Counters
    public int desiredEnemyCount = 6; // should be set by difficulty
    public int desiredWaveCounter = 6; // should be set by difficulty
    int enemyCount = 0;
    int waveCounter = 6; // After you kill x amount of enemies, a big wave will spawn

    public static EnemyManager instance;

    public GameObject[] enemyPrefabs; // assigned in editor
    public GameObject cargoshipPrefab; // assigned in editor

    //Difficulty Modifier starts at 1 and increases over time.
    //This modifies desiredwave and desiredEnemy counts and also wave counter to avoid constant waves
    public float difficultyMod = 1.0f;
    private int nextDifficultyInt = 2;
    public static float difficultyIncrement = 0.0005f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {

        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerPos = playerObject.transform.position;
        // Spawn initial Wave
        SpawnEnemiesInRandomCircle(playerPos, enemyPrefabs[0], minSpawnDistance, maxSpawnDistance, enemyGroupCount, enemyGroupSize, enemyGroupRadius);
        //SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(250, 250), enemyGroupCount, enemyGroupSize, enemyGroupRadius);

        //THIS BLOCKCOMMENT DISABLES THE CARGO SHIP

        SpawnSingleEnemyAround(playerPos, cargoshipPrefab, 150f, 150f);
        //GameObject cargoShip = FindObjectsOfType<CargoshipBehaviour>()[0].gameObject;
        //SpawnEnemiesInRandomCircle(cargoshipPrefab.transform.position, enemyPrefabs[1], 10, 20, 1, 5, 0);
        //SpawnSingleCargoshipAt(cargoSpawn);

    }

    void Update()
    {
        //increment difficulty and check for increases by whole number
        if ((difficultyMod + difficultyIncrement) >= nextDifficultyInt)
        {
            nextDifficultyInt++;
            desiredEnemyCount++;
            desiredWaveCounter++;
            waveCounter++;
        }
        difficultyMod += difficultyIncrement;

        // Spawn enemies one by one outside of Waves
        if (enemyCount < desiredEnemyCount)
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

    public Vector3 GetRandomOnNavmesh(Vector3 centerPosition, float minDistance = 30f, float maxDistance = 80f)
    {
        //generate initial position
        Vector2 centerPosition2d = new Vector2(centerPosition.x, centerPosition.z);
        bool goodSpot = false;
        int searchradius = 100;
        Vector3 navPosition = centerPosition;
        while (!goodSpot)
        {
            Vector2 initPosition = centerPosition2d + UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minDistance, maxDistance);
            //move onto navmesh
            navPosition = new Vector3(initPosition.x, 0, initPosition.y);
            if (UnityEngine.AI.NavMesh.SamplePosition(navPosition, out UnityEngine.AI.NavMeshHit navHit, searchradius, UnityEngine.AI.NavMesh.AllAreas))
            {
                if (-1f <= navHit.position.y && 1f >= navHit.position.y)
                {
                    goodSpot = true;
                    navPosition = new Vector3(navHit.position.x, 0, navHit.position.z);
                }
            }
            minDistance += 2;
            maxDistance += 2;
            searchradius += 2;
            if (searchradius >= 200) goodSpot = true;
        }
        return navPosition;
    }
    public GameObject SpawnSingleEnemyAround(Vector3 centerPosition, GameObject usedPrefab, float minDistance = 50f, float maxDistance = 150f)
    {
        Vector3 enemyPosition = GetRandomOnNavmesh(centerPosition, minDistance, maxDistance);
        //Instantiate
        GameObject enemyObject = Instantiate(usedPrefab, enemyPosition, Quaternion.identity);
        //Check if actually on navmesh
        //DOESN'T WORK WITH LIVE-BAKED NAVMESH
        /*
        if (!enemyObject.GetComponent<UnityEngine.AI.NavMeshAgent>().isOnNavMesh)
        {
            Destroy(enemyObject);
            return null;
        }
        else
        {
            enemyObject.GetComponent<EnemyBehaviour>().OnDeath += ReduceEnemyCount;
            enemyCount += 1;
            return enemyObject;
        }
        */
        enemyObject.GetComponent<EnemyBehaviour>().OnDeath += ReduceEnemyCount;
        enemyCount += 1;
        return enemyObject;
    }

    public void SpawnEnemiesInRandomCircle(Vector3 originalCenterPosition, GameObject usedPrefab, float originalMinDistance = 50f, float originalMaxDistance = 150f, int groupAmount = 10, int enemyPerGroup = 1, float groupRadius = 10)
    {
        for (int i = 0; i < groupAmount; i++)
        {
            Vector3 navPosition = GetRandomOnNavmesh(originalCenterPosition, originalMinDistance, originalMaxDistance);

            for (int j = 0; j < enemyPerGroup; j++)
            {
                SpawnSingleEnemyAround(navPosition, usedPrefab, minDistance: 0, maxDistance: groupRadius);
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