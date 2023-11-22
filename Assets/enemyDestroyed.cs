using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class enemyDestroyed : MonoBehaviour
{
    public float health = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        health = (float) Variables.Object(gameObject).Get("health");
    }

    // Update is called once per frame
    void Update()
    {
        if(health == 0) {
            Destroy(gameObject);
            Debug.Log("Does this print?");
        }
    }

    void FixedUpdate(){
        health -= 1;
        Debug.Log(health);
    }
}
