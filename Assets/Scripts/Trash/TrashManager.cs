using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public static TrashManager instance;

    public GameObject trashPrefab; // assigned in editor

    void Awake() {
        instance = this;
    }

    void Start() {
        // SpawnBulkTrashWithinRadius(new Vector2(0, 0), 9, 3);

        SpawnRandomTrashWithinArea(new Vector2(0, 0), new Vector2(250, 250));
    }

    public void SpawnRandomTrashWithinArea(Vector2 center, Vector2 size, int groupAmount = 10, int trashPerGroup = 10, float groupRadius = 10/*,  float trashRadius = 2 */) {
        for (int i = 0; i < groupAmount; i ++) {
            float centerX = UnityEngine.Random.Range(0f, size.x);
            float centerY = UnityEngine.Random.Range(0f, size.y);

            for (int j = 0; j < trashPerGroup; j++) {
                float offsetX = UnityEngine.Random.Range(-groupRadius / 2, groupRadius / 2);
                float offsetY = UnityEngine.Random.Range(-groupRadius / 2, groupRadius / 2);

                float trashX = centerX + offsetX;
                float trashY = centerY + offsetY;

                // trashX += UnityEngine.Random.Range(-trashRadius / 2, trashRadius / 2);
                // trashY += UnityEngine.Random.Range(-trashRadius / 2, trashRadius / 2);
                
                
                trashX = Mathf.Clamp(trashX, 0, size.x);
                trashY = Mathf.Clamp(trashY, 0, size.y);

                Vector2 spawnPosition = center - size / 2 + new Vector2(trashX, trashY);

                float terrainHeightAtLocation = Terrain.activeTerrain.SampleHeight(new Vector3(spawnPosition.x, 0, spawnPosition.y));
                if (terrainHeightAtLocation > 18) {
                    Debug.Log("Trash within island, skipping");
                } else {
                    Debug.Log(terrainHeightAtLocation);
                }

                SpawnSingleTrashAt(spawnPosition);
            }

        }
    }

    public void SpawnBulkTrashWithinRadius(Vector2 location, float radius, float gap) {
        for (float radi = 0; radi <= radius; radi += gap) {
            float perimeter = 2 * Mathf.PI * radi;

            for (float i = 0; i < perimeter; i += gap) {
                float angle = ( i / perimeter ) * 360; // angle in degrees

                Vector2 displacement = Quaternion.Euler(0, 0, angle) * new Vector2(radi, 0);

                GameObject trashObject = SpawnSingleTrashAt(location + displacement);
            } 
        }
    }

    public GameObject SpawnSingleTrashAt(Vector2 location) {
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

    public void RemoveAllTrash() {
        Trash[] allTrash = FindObjectsOfType<Trash>();

        foreach (Trash trash in allTrash)
        {
            Destroy(trash.gameObject);
        }

    }
}
