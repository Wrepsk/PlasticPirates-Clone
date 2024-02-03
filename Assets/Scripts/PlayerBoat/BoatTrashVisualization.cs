using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BoatTrashVisualization : MonoBehaviour
{

    [SerializeField] Transform TrashDropPosition;
    [SerializeField] Collider BoatColl;

    List<GameObject> visualTrashes;

    void Start()
    {
        visualTrashes = new List<GameObject>();
        
        TrashManager.instance.OnTrashCollected += HandleTrashCollected;
        StatsManager.instance.PropertyChanged += StatsManager_PropertyChanged;
    }


    private void HandleTrashCollected(Trash trash)
    {
        var clone = Instantiate(TrashManager.instance.meshPrefabs[trash.meshType], TrashDropPosition);
        clone.transform.localScale *= 0.5f;
        Physics.IgnoreCollision(clone.GetComponent<Collider>(), BoatColl);
        visualTrashes.Add(clone);
    }

    private void OnDestroy()
    {
        TrashManager.instance.OnTrashCollected -= HandleTrashCollected;
        StatsManager.instance.PropertyChanged -= StatsManager_PropertyChanged;
    }

    private void StatsManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "CollectedTrash")
        {
            while (StatsManager.instance.CollectedTrash < visualTrashes.Count)
            {
                Destroy(visualTrashes[0]);
                visualTrashes.RemoveAt(0);
            }
        }
    }
}
