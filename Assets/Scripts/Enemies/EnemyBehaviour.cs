using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject playerObject;
    private Vector3 initialDirection = new Vector3(1,0,0);
    public Vector3 ownPosition;
    private Vector3 playerPos;
    private Vector3 diffVector;
    private Vector3 directionDiffVec;
    private NavMeshAgent agent;
    Rigidbody rb;
    public Vector3 destination;


    [SerializeField] Transform motorPosition;

    void Start()
    {
        playerObject = GameObject.FindWithTag("Player");

        rb = GetComponent<Rigidbody>();
        vectorUpdate();
        transform.eulerAngles = initialDirection;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = playerPos; 
    }

    private void FixedUpdate()
    {
        vectorUpdate();
        if (Vector3.Distance(destination, playerPos) > 10.0f)
        {
            destination = playerPos;
            agent.destination = destination;
            
        }
        
        Quaternion _lookRotation = Quaternion.LookRotation(directionDiffVec);
        _lookRotation = RotateQuaternionLeft(_lookRotation, 90);
        transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, _lookRotation, Time.deltaTime * 5);
        
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
