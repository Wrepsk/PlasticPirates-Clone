using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager instance;


    [Header("Generation Settings")]

    [Tooltip("This seed will be used to generate random terrains. Type 0 for a random seed.")]
    public int seed = 0;

    [Tooltip("Set whether TerrainManager will automatically generate terrains.")]
    public bool autoGenerate = true;


    [Header("Optimization Settings")]

    [Tooltip("Set whether TerrainManager will delete terrains out of range.")]
    public bool optimizeTerrains = true;

    [Tooltip("Set terrain diameter.")]
    public int terrainDiameter = 7;


    [Header("Terrain Settings")]

    [Tooltip("Set the prefab terrains that TerrainManager will use.")]
    public GameObject[] terrainTypes; // This is set in the editor
    
    [Tooltip("The terrains will be created under this object.")]
    public GameObject terrainsParent; // This is set in the editor

    
    [Header("Debug Settings")]

    [Tooltip("Show terrain outline with a bright color")]
    public bool debugTerrainOutline = false;
    public GameObject debugOutlinePrefab; // This is set in the editor


    private Dictionary<int2, GameObject> _createdTerrains;

    void Awake()
    {
        instance = this;

        if (terrainDiameter % 2 == 0) {
            throw new Exception("Terrain diameter can not be even!");
        }

        _createdTerrains = new Dictionary<int2, GameObject>();
    }

    void Start()
    {
        // Below code will later be moved to GameManager
        if (autoGenerate) InvokeRepeating(nameof(GenerateTerrainsWithinPlayerRange), 0, 5);
        if (autoGenerate && optimizeTerrains) InvokeRepeating(nameof(DestroyTerrainsOutsidePlayerRange), 0, 5);
        Invoke(nameof(GenerateNavMesh), 0);
    }

    
    private void GenerateNavMesh()
    {
        // NavMeshData nvData = NavMeshBuilder.BuildNavMeshData(NavMesh.GetSettingsByID(1), _createdTerrains.Values.Select(x => new NavMeshBuildSource { shape = NavMeshBuildSourceShape.Terrain, sourceObject = x }).ToList(), new Bounds(), new Vector3(), Quaternion.identity);
        // GameObject.FindGameObjectWithTag("Environment").GetComponent<NavMeshSurface>().navMeshData = nvData;
        // EnemyManager.instance.SpawnRandomEnemyWithinArea(new Vector2(0, 0), new Vector2(250, 250), 2, 2, 10);


        // ABSOLUTE HACK AHEAD !!! 
        // i don't know what i'm doing but we need to hurry until the Demo so
        // we'll fix this once it's done
        GameObject.FindGameObjectWithTag("Environment").transform.position -= new Vector3(0, 10, 0);
        GameObject.FindGameObjectWithTag("Environment").GetComponent<NavMeshSurface>().BuildNavMesh();
        GameObject.FindGameObjectWithTag("Environment").transform.position += new Vector3(0, 10, 0);
    }

    public int2 GetPlayerGrid()
    {

        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        int playerX = Mathf.RoundToInt(playerPos.x / 128f);
        int playerY = Mathf.RoundToInt(playerPos.z / 128f);

        return new(playerX, playerY);
    }

    void GenerateTerrainsWithinPlayerRange() 
    {
        int2 playerGrid = GetPlayerGrid();

        int halve = (terrainDiameter - 1) / 2;

        for (int x = -halve; x <= halve; x++) {
            for (int y = -halve; y <= halve; y++) {
                int randomTerrainType = UnityEngine.Random.Range(0, terrainTypes.Length);
                GenerateTerrainAt(new int2(x + playerGrid.x, y + playerGrid.y), randomTerrainType);
            }
        }

        // Below code will later be moved to GameManager
        TrashManager.instance.SpawnTrashWithinPlayerRange();
    }

    void DestroyTerrainsOutsidePlayerRange() {
        int2 playerGrid = GetPlayerGrid();

        foreach (int2 terrainGrid in _createdTerrains.Keys.ToList()) {
            int terrainRange = (terrainDiameter - 1) / 2;

            if (Mathf.Abs(terrainGrid.x - playerGrid.x) > terrainRange ||
                Mathf.Abs(terrainGrid.y - playerGrid.y) > terrainRange) 
            {
                DestroyTerrainAt(terrainGrid);
            } 
        }

        // Below code will later be moved to GameManager
        TrashManager.instance.RemoveAllTrashOutsidePlayerRange();
    }

    // OBSOLETE CODE 
    // public bool GenerateTerrainWithTrashAt(int2 gridPosition, int terrainType, int trashGroupAmount)
    // {
    //     bool isNewlyGenerated = GenerateTerrainAt(gridPosition, terrainType);

    //     if (isNewlyGenerated) {
    //         TrashManager.instance.SpawnRandomTrashWithinArea(
    //             new Vector2(gridPosition.x * 128, gridPosition.y * 128), // center of terrain
    //             new Vector2(128, 128), // terrain size
    //             trashGroupAmount
    //         );
    //     }

    //     return isNewlyGenerated;
    // }
    // OBSOLETE CODE

    public bool GenerateTerrainAt(int2 gridPosition, int terrainType) 
    {
        if (gridPosition.x == 0 && gridPosition.y == 0) return false; // Base grid, skipping.

        // Grid (0, 0)  ->  [-64, -64] -- [ 64, 64]
        // Grid (1, 0)  ->  [ 64, -64] -- [192, 64]

        if (_createdTerrains.ContainsKey(gridPosition)) return false;

        int gridX = gridPosition.x;
        int gridY = gridPosition.y;

        float terrainPosX = gridX * 128;
        float terrainPosY = gridY * 128;

        GameObject anchorParent = new GameObject();
        anchorParent.transform.position = new Vector3(terrainPosX, -10, terrainPosY);
        anchorParent.transform.parent = terrainsParent.transform;
        anchorParent.name = string.Format("Terrain {0} ({1}, {2})", terrainType, gridX, gridY);

        GameObject terrainPrefab = terrainTypes[terrainType];

        GameObject liveTerrain = Instantiate(terrainPrefab, new Vector3(), Quaternion.identity);
        liveTerrain.transform.parent = anchorParent.transform;
        liveTerrain.name = string.Format("TerrainModel {0} ({1}, {2})", terrainType, gridX, gridY);
        liveTerrain.transform.localPosition = new Vector3(-64, 0, -64);


        // anchorParent.transform.eulerAngles = new Vector3(0, rotationQuad * 90, 0);
        // ....
        // turns out you can _not_ rotate terrains in Unity
        // Wtf ?

        Debug.Log("Generated " + anchorParent.name);

        if (debugTerrainOutline) {
            GameObject debugOutline = Instantiate(debugOutlinePrefab, new Vector3(), Quaternion.identity);
            debugOutline.transform.parent = anchorParent.transform;
            debugOutline.name = "DebugOutline";
            debugOutline.transform.localPosition = new Vector3(-64, 11, -64);
        }

        _createdTerrains.Add(gridPosition, anchorParent);
        return true;

    }

    public bool DestroyTerrainAt(int2 gridPosition)
    {
        if (!_createdTerrains.ContainsKey(gridPosition)) return false;

        Destroy(_createdTerrains[gridPosition]);

        _createdTerrains.Remove(gridPosition);

        return true;
    }

    public Terrain GetClosestCurrentTerrain(Vector3 position)
    {
        Terrain[] terrains = Terrain.activeTerrains;

        //Get the closest one to the player
        var center = new Vector3(terrains[0].transform.position.x + terrains[0].terrainData.size.x / 2, position.y, terrains[0].transform.position.z + terrains[0].terrainData.size.z / 2);
        float lowDist = (center - position).sqrMagnitude;
        var terrainIndex = 0;

        for (int i = 0; i < terrains.Length; i++)
        {
            center = new Vector3(terrains[i].transform.position.x + terrains[i].terrainData.size.x / 2, position.y, terrains[i].transform.position.z + terrains[i].terrainData.size.z / 2);

            //Find the distance and check if it is lower than the last one then store it
            var dist = (center - position).sqrMagnitude;
            if (dist < lowDist)
            {
                lowDist = dist;
                terrainIndex = i;
            }
        }

        return terrains[terrainIndex];
    }

}
