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

    public ParticleSystem bulltetsDropping;
    public ParticleSystem shootingAnimation;

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        if (bulltetsDropping != null && !bulltetsDropping.isPlaying) bulltetsDropping.Play();
        if (shootingAnimation != null && !shootingAnimation.isPlaying) shootingAnimation.Play();

        RaycastHit hit;
        if(Physics.Raycast(gunPoint.position, -gunPoint.right, out hit, shootingDistance, enemyLayers))
        {
            Debug.Log("Hit the enemy: " + hit.transform.name);

            hit.transform.GetComponent<EnemyBehaviour>().DealDamage(damage);

            if (hitSounds.Length > 0 && audioSource != null)
            {
                AudioClip randomClip = hitSounds[Random.Range(0, hitSounds.Length)];
                audioSource.PlayOneShot(randomClip, 0.8f);
            }
        }
    }

    public override void StopUse()
    {
        if (bulltetsDropping != null) bulltetsDropping.Stop();
        if (shootingAnimation != null) shootingAnimation.Stop();
    }
}
