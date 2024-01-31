using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTrashVisualization : MonoBehaviour
{

    [SerializeField] Transform TrashDropPosition;
    [SerializeField] Collider BoatColl;
    void Start()
    {
        TrashManager.instance.OnTrashCollected += HandleTrashCollected;
    }

    private void HandleTrashCollected(Trash trash)
    {
        var clone = Instantiate(TrashManager.instance.meshPrefabs[trash.meshType], TrashDropPosition);
        clone.transform.localScale *= 0.5f;
        Physics.IgnoreCollision(clone.GetComponent<Collider>(), BoatColl);
        
    }
}
