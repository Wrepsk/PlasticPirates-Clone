using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosionParticle;
    public float cannonLifetime = 5f;

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
            other.transform.root.GetComponent<BoatMovement>().DealDamage(100);

        if (other.transform.root.tag == "Enemy")
        {
            other.transform.root.GetComponent<EnemyBehaviour>().DealDamage(100);
        }

        GameObject audioObject = new GameObject("AudioObject");
        audioObject.transform.position = transform.position;

        // F�ge einen tempor�ren AudioSource hinzu
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.15f;
        audioSource.clip = audioClip;

        // Spiele den Sound ab
        audioSource.Play();

        // Starte die Zerst�rung des AudioObjects mit Verz�gerung entsprechend der Sounddauer
        Destroy(audioObject, audioClip.length);

        // Erzeuge die Explosion und zerst�re das Haupt-GameObject
        Instantiate(explosionParticle, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
