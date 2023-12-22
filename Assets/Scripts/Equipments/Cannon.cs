using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Equipment
{

    public float shootingDistance = 1000f;

    public GameObject cannonBall;
    public Transform barrel;

    public float force;
    long unixTimeLastShot = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

    public AudioSource audioSource;
    public AudioClip audioClip;


    public override void Use()
    {
        DateTime currentTime = DateTime.UtcNow;
        long unixTimeNow = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        if (unixTimeNow - unixTimeLastShot >= 1) {
            Shoot();
            unixTimeLastShot = unixTimeNow;
        }
    }

    void Shoot()
    {
        audioSource.PlayOneShot(audioClip);
        GameObject bullet = Instantiate(cannonBall, barrel.position, barrel.rotation);
        bullet.GetComponent<Rigidbody>().velocity = barrel.forward * force * Time.deltaTime;
    }

    public override void StopUsing(){}
}
