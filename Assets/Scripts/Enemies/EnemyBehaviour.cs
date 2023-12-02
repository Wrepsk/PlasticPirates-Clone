using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    //Actors and Helpers
    public GameObject playerObject;
    public NavMeshAgent agent;
    private bool isAggroed = false;
    
    //Movement Helpers
    public Vector3 ownPosition;
    public Vector3 destination;
    public float turnspeed = 5.0f;
    public float aggroRange = 128f;

    //Vector Helpers
    private Vector3 playerPos;
    private Vector3 diffVector;
    private Vector3 directionDiffVec;
    private Vector3 initialDirection = new Vector3(1,0,0);

    //Physics Objects
    [SerializeField] Transform motorPosition;
    Rigidbody rb;

    void Start()
    {
        // Finding player object without manually assigning it
        playerObject = GameObject.FindGameObjectWithTag("Player");
        //init variables
        rb = GetComponent<Rigidbody>();
        transform.eulerAngles = initialDirection;
        agent = GetComponent<NavMeshAgent>();
        vectorUpdate();
        
    }

    private void FixedUpdate()
    {
        vectorUpdate();
        if (Vector3.Distance(destination, playerPos) > 10.0f & isAggroed) 
        {
            agent.destination = playerPos;
            
        }
        if (diffVector.magnitude < aggroRange)
        {
            agent.destination = playerPos;
            isAggroed = true;
        }
        
        //Turns enemy in direction of player by turnspeed
        Quaternion _lookRotation = Quaternion.LookRotation(directionDiffVec);
        _lookRotation = RotateQuaternionLeft(_lookRotation, 90);
        transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, _lookRotation, Time.deltaTime * turnspeed);
        
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
        //Updates posititon vectors and calculated vectors
        ownPosition = transform.position;
        playerPos = playerObject.transform.position;
        diffVector = playerPos - ownPosition;
        directionDiffVec = diffVector.normalized;

    }
}
