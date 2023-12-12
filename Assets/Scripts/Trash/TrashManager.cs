using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public static TrashManager instance;

    public GameObject trashPrefab; // assigned in editor


    [Header("Generation Settings")]

    [Tooltip("Set whether TrashManager will automatically generate trash.")]
    public bool autoGenerate = true;


    [Header("Optimization Settings")]

    [Tooltip("Set trash generation grid diameter.")]
    public int trashDiameter = 3;


    private List<int2> _trashedGrids;

    void Awake()
    {
        instance = this;

        if (trashDiameter % 2 == 0)
        {
            throw new Exception("Trash generation grid diameter can not be even!");
        }

        _trashedGrids = new List<int2>();
    }

    public void SpawnTrashWithinPlayerRange()
    {
        int2 playerGrid = TerrainManager.instance.GetPlayerGrid();

        int halve = (trashDiameter - 1) / 2;

        for (int x = -halve; x <= halve; x++)
        {
            for (int y = -halve; y <= halve; y++)
            {
                SpawnTrashWithinGrid(new int2(x + playerGrid.x, y + playerGrid.y), 5);
            }
        }
    }

    public void RemoveAllTrashOutsidePlayerRange()
    {
        int2 playerGrid = TerrainManager.instance.GetPlayerGrid();

        foreach (int2 terrainGrid in _trashedGrids.ToList())
        {
            int terrainRange = (trashDiameter - 1) / 2;

            if (Mathf.Abs(terrainGrid.x - playerGrid.x) > terrainRange ||
                Mathf.Abs(terrainGrid.y - playerGrid.y) > terrainRange)
            {
                RemoveAllTrashWithinGrid(terrainGrid);
            }
        }
    }

    public bool SpawnTrashWithinGrid(int2 gridPosition, int trashGroupAmount)
    {
        if (_trashedGrids.Contains(gridPosition)) return false;

        SpawnRandomTrashWithinArea(
            new Vector2(gridPosition.x * 128, gridPosition.y * 128), // center of terrain
            new Vector2(128, 128), // terrain size
            trashGroupAmount
        );

        _trashedGrids.Add(gridPosition);

        return true;
    }

    public bool RemoveAllTrashWithinGrid(int2 gridPosition)
    {
        if (!_trashedGrids.Contains(gridPosition)) return false;

        int gridX = gridPosition.x;
        int gridY = gridPosition.y;

        float terrainPosX = gridX * 128;
        float terrainPosY = gridY * 128;

        RemoveAllTrashWithinArea(new Vector2(terrainPosX, terrainPosY), new Vector2(128, 128));
        _trashedGrids.Remove(gridPosition);

        return true;
    }


    // IMPORTANT: The area is a rectangle!
    public void SpawnRandomTrashWithinArea(Vector2 center, Vector2 size, int groupAmount = 10, int trashPerGroup = 10, float groupRadius = 10/*,  float trashRadius = 2 */)
    {
        for (int i = 0; i < groupAmount; i++)
        {
            float centerX = UnityEngine.Random.Range(0f, size.x);
            float centerY = UnityEngine.Random.Range(0f, size.y);

            for (int j = 0; j < trashPerGroup; j++)
            {
                float offsetX = UnityEngine.Random.Range(-groupRadius / 2, groupRadius / 2);
                float offsetY = UnityEngine.Random.Range(-groupRadius / 2, groupRadius / 2);

                float trashX = centerX + offsetX;
                float trashY = centerY + offsetY;

                // trashX += UnityEngine.Random.Range(-trashRadius / 2, trashRadius / 2);
                // trashY += UnityEngine.Random.Range(-trashRadius / 2, trashRadius / 2);


                trashX = Mathf.Clamp(trashX, 0, size.x);
                trashY = Mathf.Clamp(trashY, 0, size.y);

                Vector2 spawnPosition = center - size / 2 + new Vector2(trashX, trashY);

                //  -- TODO: Optimise this --
                Terrain currentTerrain;

                // Only use TerrainManager if it is actually in the scene
                if (TerrainManager.instance != null)
                {
                    currentTerrain = TerrainManager.instance
                        .GetClosestCurrentTerrain(new Vector3(spawnPosition.x, 0, spawnPosition.y));
                }
                // Else default to activeTerrain
                else
                {
                    currentTerrain = Terrain.activeTerrain;
                }

                float terrainHeightAtLocation = currentTerrain
                    .SampleHeight(new Vector3(spawnPosition.x, 0, spawnPosition.y));

                Debug.Log(spawnPosition + " " + terrainHeightAtLocation + " " + currentTerrain.name);

                if (terrainHeightAtLocation > -currentTerrain.transform.position.y)
                {
                    Debug.Log("Trash within island, skipping: " + terrainHeightAtLocation);
                    continue;
                }
                // --------------------------

                SpawnSingleTrashAt(spawnPosition);
            }

        }
    }

    public void SpawnBulkTrashWithinRadius(Vector2 location, float radius, float gap)
    {
        for (float radi = 0; radi <= radius; radi += gap)
        {
            float perimeter = 2 * Mathf.PI * radi;

            for (float i = 0; i < perimeter; i += gap)
            {
                float angle = (i / perimeter) * 360; // angle in degrees

                Vector2 displacement = Quaternion.Euler(0, 0, angle) * new Vector2(radi, 0);

                GameObject trashObject = SpawnSingleTrashAt(location + displacement);
            }
        }
    }

    public GameObject SpawnSingleTrashAt(Vector2 location)
    {
        Debug.Log(location);

        float scale = UnityEngine.Random.Range(0.5f, 1.0f);

        GameObject trashObject = Instantiate(trashPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);
        trashObject.transform.localScale = new Vector3(scale, scale, scale);

        GameObject trashCube = trashObject.transform.GetChild(0).gameObject;
        trashCube.transform.rotation = Quaternion.Euler(
            UnityEngine.Random.Range(0, 360),
            UnityEngine.Random.Range(0, 360),
            UnityEngine.Random.Range(0, 360)
        );

        return trashObject;
    }

    // IMPORTANT: The area is a rectangle!
    public void RemoveAllTrashWithinArea(Vector2 center, Vector2 size)
    {
        Trash[] allTrash = FindObjectsOfType<Trash>();

        foreach (Trash trash in allTrash)
        {
            Vector2 trashLocation = new Vector2(trash.transform.position.x, trash.transform.position.z);

            if (trashLocation.x > center.x + size.x / 2) continue;
            if (trashLocation.x < center.x - size.x / 2) continue;

            if (trashLocation.y > center.y + size.y / 2) continue;
            if (trashLocation.y < center.y - size.y / 2) continue;

            Destroy(trash.gameObject);
        }

    }

    public void RemoveAllTrash()
    {
        Trash[] allTrash = FindObjectsOfType<Trash>();

        foreach (Trash trash in allTrash)
        {
            Destroy(trash.gameObject);
        }

    }
}
