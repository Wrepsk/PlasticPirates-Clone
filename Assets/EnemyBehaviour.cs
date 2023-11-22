using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject playerObject;

    public float rotateSpeed = 1f;
    public float moveSpeed = 0.005f;
    public Vector3 initialDirection = new Vector3(1,0,0);
    public Vector3 ownPosition;
    public Vector3 playerPos;
    public Vector3 diffVector;
    // Start is called before the first frame update
    void Start()
    {
        vectorUpdate();
        transform.eulerAngles = initialDirection;
    }
    // Update is called once per frame
    void Update()
    {
        vectorUpdate();
        transform.position = Vector3.MoveTowards(ownPosition, playerPos, moveSpeed);
        transform.Rotate(diffVector, Time.deltaTime * rotateSpeed);
    }
    Vector3 vecDiff(Vector3 own, Vector3 play){
        return own - play;
    }
    void vectorUpdate(){
        ownPosition = transform.position;
        playerPos = playerObject.transform.position;
        diffVector = vecDiff(ownPosition, playerPos);
    }
}
