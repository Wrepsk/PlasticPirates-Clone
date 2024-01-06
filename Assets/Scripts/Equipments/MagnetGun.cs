using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MagnetGun : Equipment
{

    public Transform attractor, boat;
    public Vector3 sizeOfCollider = new Vector3();
    public float attractionStrenght = 5f;

    private void Awake()
    {
        suppressLastAudio = false;
    }

    public override void Use()
    {
        AttractTrash();
    }

    void AttractTrash()
    {
        Debug.Log("Attracting trash!");
        Collider[] trashColliders = Physics.OverlapBox(attractor.position, sizeOfCollider);
        foreach(Collider collider in trashColliders)
        {
            if(collider.CompareTag("Trash"))
            {
                Debug.Log("Attracting " + collider.transform.name);
                Vector3 forceDirection = boat.position - collider.transform.position;
                collider.GetComponent<Rigidbody>().AddForce(forceDirection.normalized * attractionStrenght);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(attractor.position.x, attractor.position.y, attractor.position.z), new Vector3(sizeOfCollider.x * 2, sizeOfCollider.y * 2, sizeOfCollider.z * 2));
    }
    void DrawRect(Rect rect)
    {
    }
}
