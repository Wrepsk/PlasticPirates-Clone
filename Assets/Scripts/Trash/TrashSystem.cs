using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSystem : MonoBehaviour
{
    public static TrashSystem instance;

    public GameObject trashPrefab; // assigned in editor

    void Awake() {
        instance = this;
    }

    void Start() {
        GenerateBulkTrashWithinRadius(new Vector3(0, 0, 0), 9, 3);
    }

    public void GenerateBulkTrashWithinRadius(Vector3 location, float radius, float gap) {
        for (float radi = 0; radi <= radius; radi += gap) {
            float perimeter = 2 * Mathf.PI * radi;

            for (float i = 0; i < perimeter; i += gap) {
                float angle = ( i / perimeter ) * 360; // angle in degrees

                GameObject trashObject = GenerateSingleTrashAt(location + Quaternion.Euler(0, angle, 0) * new Vector3(radi, 0, 0));
            } 
        }
    }

    public GameObject GenerateSingleTrashAt(Vector3 location) {
        Debug.Log(location);

        float scale = UnityEngine.Random.Range(0.5f, 1.0f);

        GameObject trashObject = Instantiate(trashPrefab, location, Quaternion.identity);
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
