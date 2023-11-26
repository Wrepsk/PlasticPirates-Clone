using UnityEngine;

public class Trash : MonoBehaviour
{

    void OnTriggerEnter(Collider collision) {
        // 6 is the layer for player boat colliders
        if (collision.gameObject.layer == 6) OnTrashCollected();
    }

    void OnTrashCollected() {
        StatsManager.instance.CollectedTrash += 1;
        Destroy(gameObject);
    }
}
