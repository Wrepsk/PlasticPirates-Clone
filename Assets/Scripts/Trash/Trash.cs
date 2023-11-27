using UnityEngine;

public class Trash : MonoBehaviour
{

    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Player") OnTrashCollected();
    }

    void OnTrashCollected() {
        StatsManager.instance.CollectedTrash += 1;
        Destroy(gameObject);
    }
}
