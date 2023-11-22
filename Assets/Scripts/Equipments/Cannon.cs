using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Equipment
{

    public Transform gunPoint;
    public LayerMask enemyLayers;
    public float shootingDistance = 1000f;

    public GameObject cannonBall;
    public Transform barell;

    public float force;


    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        //RaycastHit hit;
        //if(Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, enemyLayers))
        //{
        //    Debug.Log("Hit the enemy: " + hit.transform.name);
        //}
        GameObject bullet = Instantiate(cannonBall, barell.position, barell.rotation);
        bullet.GetComponent<Rigidbody>().velocity = barell.forward * force * Time.deltaTime;
    }
}
