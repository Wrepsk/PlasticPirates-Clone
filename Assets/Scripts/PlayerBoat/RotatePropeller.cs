using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePropeller : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotation;
    [SerializeField]
    private float rotationSpeed;
    
    void Update()
    {
        // Rotation of the propeller by the vector rotation
        transform.Rotate(rotation * rotationSpeed);
    }
}
