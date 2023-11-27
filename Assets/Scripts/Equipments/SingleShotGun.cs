using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Equipment
{

    public Transform gunPoint;
    public LayerMask enemyLayers;
    public float shootingDistance = 1000f;

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, enemyLayers))
        {
            Debug.Log("Hit the enemy: " + hit.transform.name);
        }
    }
}
