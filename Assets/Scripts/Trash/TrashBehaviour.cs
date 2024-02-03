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

        if (trash.rising)
        {
            gameObject.transform.position += new Vector3(0, trash.manager.riseSpeed, 0);
            trash.position.y += trash.manager.riseSpeed;
            if (trash.position.y > -(trash.manager.meshes[trash.meshType].bounds.size.y / 2))
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -(trash.manager.meshes[trash.meshType].bounds.size.y / 2), gameObject.transform.position.z);
                trash.rising = false;
            }
        }
    }
}
