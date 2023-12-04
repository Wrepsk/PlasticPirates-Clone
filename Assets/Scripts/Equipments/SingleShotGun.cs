using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Equipment
{

    public Transform gunPoint;
    public LayerMask enemyLayers;

    public float shootingDistance = 1000f; // 1000 meter range
    public float cooldown = 0.35f; // 0.35 seconds
    public float damage = 20f; // deals 20 hp


    private float timeLastShot = -Mathf.Infinity;

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        if (Time.realtimeSinceStartup - timeLastShot < cooldown) return;

        timeLastShot = Time.realtimeSinceStartup;

        RaycastHit hit;
        if(Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, enemyLayers))
        {
            Debug.Log("Hit the enemy: " + hit.transform.name);

            hit.transform.GetComponent<EnemyBehaviour>().DealDamage(damage);
        }
    }
}
