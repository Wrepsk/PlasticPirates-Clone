using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosionParticle;
    public float cannonLifetime = 5f;
    public int explosionRadius;

    public AudioClip audioClip;
    public AudioSource audioSource;

    private bool destroyInvoked = false;

    private void Update()
    {
        cannonLifetime -= Time.deltaTime;
        if((cannonLifetime <= 0 || transform.position.y < -5) && !destroyInvoked)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!destroyInvoked)
        {
            destroyInvoked = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    collider.transform.root.GetComponent<BoatMovement>().DealDamage(100);
                }
                else if (collider.CompareTag("Enemy"))
                {
                    collider.transform.root.GetComponent<EnemyBehaviour>().DealDamage(100);
                }
            }

            audioSource.PlayOneShot(audioClip, 0.15f);

            Instantiate(explosionParticle, transform.position, transform.rotation);
            Destroy(gameObject, audioClip.length);
        }
    }
}
