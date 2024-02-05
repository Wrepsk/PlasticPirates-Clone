using UnityEngine;
using UnityEngine.UIElements;

public class TrashBehaviour : MonoBehaviour
{
    public Trash trash;  // reference to persistent trash data, set by TrashManager
    public bool spawnedByCargoShip = false;

    void OnTriggerEnter(Collider collision)
    {
        // 8 is the layer for player boat colliders
        if (collision.gameObject.layer == 8 && StatsManager.instance.CollectedTrash <= 25) {
            TrashManager.instance.CollectTrash(trash);

        }
    }

    void Update()
    {
        if (spawnedByCargoShip && transform.position.y <= 0)
        {
            spawnedByCargoShip = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().drag = 20;
            GetComponent<Rigidbody>().mass = 1f;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            //tried things
            trash.manager = TrashManager.instance;
            trash.position = transform.position;
            trash.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            trash.xPhase = trash.position.x * TrashManager.instance.buoyancySpatialScaling + Random.Range(0.0f, TrashManager.instance.buoyancyRandomness);
            trash.zPhase = trash.position.z * TrashManager.instance.buoyancySpatialScaling + Random.Range(0.0f, TrashManager.instance.buoyancyRandomness);

        }
        else if (spawnedByCargoShip && transform.position.y > 0)
        {
            transform.GetChild(0).gameObject.transform.position = transform.position;
            return;

        }

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
