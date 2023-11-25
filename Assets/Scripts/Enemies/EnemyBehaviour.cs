using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject playerObject;

    public float rotateSpeed = 1f;
    public float moveSpeed = 0.005f;
    private Vector3 initialDirection = new Vector3(1,0,0);
    public Vector3 ownPosition;
    private Vector3 playerPos;
    private Vector3 diffVector;
    // Start is called before the first frame update
    void Start()
    {
        vectorUpdate();
        transform.eulerAngles = initialDirection;
    }
    
    Vector3 vecDiff(Vector3 own, Vector3 play){
        return own - play;
    }

    private void FixedUpdate()
    {
        vectorUpdate();
        var forceVec = Vector3.Scale(new Vector3(1,0,1), transform.forward) * diffVector.y * acceleration + Vector3.Scale(new Vector3(1, 0, 1), transform.right) * -diffVector.x * steeringStrength;

        rb.AddForceAtPosition(forceVec, motorPosition.position, ForceMode.Force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if(rb.angularVelocity.magnitude > maxAngularSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeed;
        }
    }
    void vectorUpdate(){
        ownPosition = transform.position;
        playerPos = playerObject.transform.position;
        diffVector = vecDiff(ownPosition, playerPos);
    }
}
