using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosionParticle;
    public float cannonLifetime = 5f;


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
        Instantiate(explosionParticle, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
