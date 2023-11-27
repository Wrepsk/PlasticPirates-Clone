using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Equipment
{

    public float shootingDistance = 1000f;

    public GameObject cannonBall;
    public Transform barrel;

    public float force;


    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(cannonBall, barrel.position, barrel.rotation);
        bullet.GetComponent<Rigidbody>().velocity = barrel.forward * force * Time.deltaTime;
    }
}
