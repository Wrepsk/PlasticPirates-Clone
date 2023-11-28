using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject playerObject;
    private Vector3 initialDirection = new Vector3(1,0,0);
    public Vector3 ownPosition;
    private Vector3 playerPos;
    private Vector3 diffVector;
    private Vector3 directionDiffVec;

    Rigidbody rb;


    [SerializeField] Transform motorPosition;

    [Header("Boat Control parameters")]
    [SerializeField] float accelerationfwd, maxSpeed, steeringStrength, maxAngularSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vectorUpdate();
        transform.eulerAngles = initialDirection;
    }

    private void FixedUpdate()
    {
        vectorUpdate();
        Quaternion _lookRotation = Quaternion.LookRotation(directionDiffVec);
        _lookRotation = RotateQuaternionLeft(_lookRotation, 90);
        transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, _lookRotation, Time.deltaTime * steeringStrength);
        
        moveEnemy(accelerationfwd);
        
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if(rb.angularVelocity.magnitude > maxAngularSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeed;
        }
        
        
    }

    void moveEnemy(float forwardInput){
        var forceVec = Vector3.Scale(new Vector3(1,0,1), transform.GetChild(0).right) * forwardInput;

        rb.AddForce(forceVec, ForceMode.Force);
    }

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
    void vectorUpdate(){
        //Updates posiiton vectors and calculated vectors
        ownPosition = transform.position;
        playerPos = playerObject.transform.position;
        diffVector = playerPos - ownPosition;
        directionDiffVec = diffVector.normalized;

    }
}
