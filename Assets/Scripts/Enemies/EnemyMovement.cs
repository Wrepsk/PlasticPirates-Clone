using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float enemDriveSpeed = 2.0f;

    [SerializeField]
    private float speedReductionIfClose = 0.5f;

    [SerializeField]
    private float enemTurnSpeed = 1.0f;

    private Vector3 diffVector;
    private Vector3 diffVectorNorm;
    private float dist;

    // Raycast that simulates the opponent's field of vision
    Ray sight;


    void Start()
    {
        // Rigidbody component of the enemy
        rb = this.GetComponent<Rigidbody>();

        // Finding player object without manually assigning it
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Ray of sight of the enemy
        sight = new Ray(transform.position, diffVector);
        Debug.DrawRay(transform.position, diffVector);
    }


    void FixedUpdate()
    {
        // Calculation to position the enemy
        diffVector = playerTransform.position - transform.position;
        diffVectorNorm = diffVector.normalized;

        // Distance between player and enemy
        dist = diffVector.magnitude;
        Debug.Log(dist);

        // if in sight then turn to player
        if (Physics.Raycast(sight, out RaycastHit hit))
        {
            if (hit.collider.tag == "Environment")
            {
                Debug.Log("Out of sight!");
            }
            else
            {
                // Rotates the enemy in the direction of the player 
                updateDirection();
                // Applys force to move the enemy to the player
                moveEnemy(diffVectorNorm);
            }   
        } 
    }


    // Movement by adding Force to the RigidBody
    void moveEnemy(Vector3 direction)
    {
        // If close then less force
        if(dist < 10)
        {
            rb.AddForce(direction * (enemDriveSpeed * speedReductionIfClose));
        }
        else
        {
            rb.AddForce(direction * enemDriveSpeed);
        }
    }


    // Updates the direction the enemy is facing
    void updateDirection()
    {
        Quaternion _lookRotation = Quaternion.LookRotation(diffVectorNorm);
        _lookRotation = RotateQuaternionLeft(_lookRotation, 90);
        transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, _lookRotation, Time.deltaTime * enemTurnSpeed);
    }


    // Rotates a Quaternion <angleDegrees> to the left
    Quaternion RotateQuaternionLeft(Quaternion original, float angleDegrees)
    {
        // Convert the angle to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate the rotation quaternion
        Quaternion rotationQuaternion = Quaternion.Euler(0f, -angleDegrees, 0f);

        // Multiply the original quaternion by the rotation quaternion
        Quaternion rotatedQuaternion = rotationQuaternion * original;

        return rotatedQuaternion;
    }
}
