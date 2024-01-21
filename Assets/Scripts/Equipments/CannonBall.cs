using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosionParticle;
    public float cannonLifetime = 5f;
    public int explosionRadius;

    public AudioClip audioClip;

    private void Update()
    {
        cannonLifetime -= Time.deltaTime;
        if(cannonLifetime <= 0 || transform.position.y < -5)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
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

        GameObject audioObject = new GameObject("AudioObject");
        audioObject.transform.position = transform.position;

        // Füge einen temporären AudioSource hinzu
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.15f;
        audioSource.clip = audioClip;

        // Spiele den Sound ab
        audioSource.Play();

        // Starte die Zerstörung des AudioObjects mit Verzögerung entsprechend der Sounddauer
        Destroy(audioObject, audioClip.length);

        // Erzeuge die Explosion und zerstöre das Haupt-GameObject
        Instantiate(explosionParticle, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
