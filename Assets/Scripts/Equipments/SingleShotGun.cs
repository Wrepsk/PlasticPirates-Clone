using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Equipment
{

    public Transform gunPoint;
    public LayerMask enemyLayers;

    public float shootingDistance = 1000f; // 1000 meter range
    public float damage = 20f; // deals 20 hp

    public AudioClip[] hitSounds;

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
            AudioClip randomClip = hitSounds[Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(randomClip);

            hit.transform.GetComponent<EnemyBehaviour>().DealDamage(damage);
        }
    }
}
