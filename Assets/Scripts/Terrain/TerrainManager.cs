using System;
using Unity.Mathematics;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager instance;

    public GameObject[] terrainTypes; // This is set in the editor
    
    public GameObject terrainsParent; // This is set in the editor

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue; // Base grid, skipping.

                int randomTerrainType = UnityEngine.Random.Range(0, terrainTypes.Length);
                GenerateTerrainAt(new int2(x, y), randomTerrainType);
            }
        }

        TrashManager.instance.SpawnRandomTrashWithinArea(new Vector2(0, 0), new Vector2(392, 392), 50);
    }

    public void GenerateTerrainAt(int2 gridPosition, int terrainType) 
    {
        // Grid (0, 0)  ->  [-64, -64] -- [ 64, 64]
        // Grid (1, 0)  ->  [ 64, -64] -- [192, 64]

        int gridX = gridPosition.x;
        int gridY = gridPosition.y;

        float terrainPosX = gridX * 128 - 64;
        float terrainPosY = gridY * 128 - 64;

        GameObject terrainPrefab = terrainTypes[terrainType];

        GameObject liveTerrain = Instantiate(terrainPrefab, new Vector3(terrainPosX, -10, terrainPosY), Quaternion.identity);
        liveTerrain.name = string.Format("Terrain ({0}, {1})", gridX, gridY);
        liveTerrain.transform.parent = terrainsParent.transform;
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
