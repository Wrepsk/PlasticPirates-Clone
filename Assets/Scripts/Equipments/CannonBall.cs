using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosionParticle;
    public float cannonLifetime = 5f;

    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Update()
    {
        cannonLifetime -= Time.deltaTime;
        if(cannonLifetime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
            return;

        if (other.transform.root.tag == "Enemy")
        {
            other.transform.GetComponent<EnemyBehaviour>().DealDamage(100);
        }

        audioSource.PlayOneShot(audioClip);

        Instantiate(explosionParticle, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
