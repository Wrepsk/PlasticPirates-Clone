using UnityEngine;

public class TrashBehaviour : MonoBehaviour
{
    public Trash trash;  // reference to persistent trash data, set by TrashManager

    void OnTriggerEnter(Collider collision)
    {
        // 8 is the layer for player boat colliders
        if (collision.gameObject.layer == 8)
            TrashManager.instance.CollectTrash(trash);
    }

    void Update()
    {
        Transform meshTransform = gameObject.transform.GetChild(0);
        meshTransform.localPosition = trash.GetBuoyancy();
        meshTransform.localRotation = trash.GetBuoyancyRotation();
    }
}
