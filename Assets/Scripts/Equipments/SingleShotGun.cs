using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Equipment
{

    public Transform gunPoint;
    public LayerMask enemyLayers;
    public LayerMask playerLayers;

    public float shootingDistance = 1000f; // 1000 meter range
    public float cooldown = 0.35f; // 0.35 seconds
    public float damage = 20f; // deals 20 hp


    private float timeLastShot = -Mathf.Infinity;

    private void Start(){
        equipmentInfo.cooldown = cooldown;
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        if (Time.realtimeSinceStartup - timeLastShot < cooldown) return;

        timeLastShot = Time.realtimeSinceStartup;

        RaycastHit hit;
        Debug.Log("Enemy hit?:" + Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, enemyLayers));
        Debug.Log("Player hit?:" + Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, playerLayers));
        if(Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, enemyLayers))
        {
            Debug.Log("Hit the enemy: " + hit.transform.name);
            hit.transform.GetComponent<EnemyBehaviour>().DealDamage(damage);
        }else if(Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, playerLayers)){
            Debug.Log("Hit the player: " + hit.transform.name);
            hit.transform.GetComponent<BoatMovement>().DealDamage(damage);
        }
    }
}
