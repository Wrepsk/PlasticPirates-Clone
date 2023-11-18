using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WaterSystem;
using WaterSystem.Physics;

public class Trash : MonoBehaviour
{

    void OnCollisionEnter(Collision collision) {

        // when destroyed, SimpleBuoyantObject behavior results in glitchy and jittery
        // behavior on all other buoyant objects due to an unknown bug.
        // Thus, we disable the script before destroying.
        
        

        if (collision.gameObject.tag == "Player") Destroy(gameObject);

    }
}
