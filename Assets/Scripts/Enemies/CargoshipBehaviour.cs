
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using WaterSystem.Physics;
using System.Drawing;
using static Unity.Burst.Intrinsics.X86;
using static UnityEditor.FilePathAttribute;
using System.Transactions;

public class CargoshipBehaviour : Damagable
{
    //Actors and Helpers
    public NavMeshAgent agent;
    private SimpleBuoyantObject simpleBuoyantObject;

    //Trash Management
    [SerializeField] GameObject trashPoint;
    public float trashSpawnRate = 3f;
    private float currentTrashDelay;
    public float throwForce = 50f;
    public float trashMass = 10f;
    public float timeBeforeEnablingSBO = 1.5f;

    //Movement Helpers
    public Vector3 dest = new Vector3(250.0f, 0.0f, 250.0f);
    public float turnspeed = 5.0f;
    public GameObject boundsObject;

    //Vector Helpers
    private Vector3 initialDirection = new Vector3(1, 0, 0);
    private Vector3 direction;
    private Quaternion lookRotation;
    private bool rotated = false;

    //Movement Helpers
    public Vector3 ownPosition;
    private Vector3 moveTo;

    //Physics Objects
    [SerializeField] Transform motorPosition;
    Rigidbody rb;

    void DeathDisable()
    {
        agent.enabled = false;
    }
    
    protected override void Start()
    {
        base.Start();
  
        
        //sets object in initial direction
        transform.eulerAngles = initialDirection;
        
        //init navigation vars
        agent = GetComponent<NavMeshAgent>();
        agent.speed = DefaultSpeed;
        agent.acceleration = DefaultSpeed;
        //agent.enabled = false;

        
        //initiates deathListener
        OnDeath += DeathDisable;

        //rigidbody
        rb = GetComponent<Rigidbody>();


        currentTrashDelay = trashSpawnRate;
        //SetRandomDestination();
    }

    protected override void Update()
    {
        
        if (IsDead)
        {
            agent.enabled = false;
            return;
        }
        base.Update();
        
        if (agent.isOnNavMesh && agent.enabled && moveTo != null)
        {
            SetRandomDestination();
            agent.enabled = true;
        }
        
        currentTrashDelay -= Time.deltaTime;
        if(currentTrashDelay <= 0)
        {
            SpawnTrashAtTheBack();
            currentTrashDelay = trashSpawnRate;
        }

    }
    private void FixedUpdate()
    {
        if (IsDead) return;

        TurnToDestination();


        if (agent.hasPath == false)
        {
            SetRandomDestination();
        }
    }


    public Vector3 GetRandomOnNavmesh(Vector3 centerPosition, float minDistance = 30f, float maxDistance = 80f) {
        //generate initial position
        Vector2 centerPosition2d = new Vector2(centerPosition.x, centerPosition.z);
        Vector2 initPosition = centerPosition2d + UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minDistance, maxDistance);

        //move onto navmesh
        Vector3 navPosition = new Vector3(initPosition.x, 0, initPosition.y);
        if (UnityEngine.AI.NavMesh.SamplePosition(new Vector3(initPosition.x, 0, initPosition.y), out UnityEngine.AI.NavMeshHit navHit, 100, UnityEngine.AI.NavMesh.AllAreas))
        {
            navPosition = new Vector3(navHit.position.x, 0, navHit.position.z);
        }
        return navPosition;
    }
    private void SetRandomDestination()
    {
        moveTo = GetRandomOnNavmesh(transform.position);
        agent.SetDestination(moveTo);
        //agent.isStopped = true;
        rotated = false;
        //CheckPointOnPath();
    }

    private void TurnToDestination()
    {
        if (Quaternion.Angle(transform.rotation, lookRotation) <= 1f || rotated == true)
        {
            //agent.isStopped = false;
            rotated = true;
            return;
        }

        direction = (moveTo - transform.position).normalized;

        lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnspeed);
    }

    private void CheckPointOnPath()
    {
        if (agent.pathEndPosition != moveTo)
        {
            SetRandomDestination();
        }
    }
    private void SpawnTrashAtTheBack()
    {
        int type = UnityEngine.Random.Range(0, TrashManager.instance.meshPrefabs.Length);
        
        Trash trash = TrashManager.instance.CreateTrash(trashPoint.transform.position, type);
        trash.ignoreDematerialize = true;
        GameObject spawnedTrash = trash.Materialize();

        spawnedTrash.GetComponent<TrashBehaviour>().enabled = false;
        spawnedTrash.GetComponent<Rigidbody>().useGravity = true;
        spawnedTrash.GetComponent<Rigidbody>().drag = 0;
        spawnedTrash.GetComponent<Rigidbody>().mass = trashMass;
        spawnedTrash.GetComponent<Rigidbody>().AddForce(new Vector3(0 , 0, throwForce));
        StartCoroutine(EnableBuoyantObjectForTrash(spawnedTrash));
    }

    IEnumerator EnableBuoyantObjectForTrash(GameObject spawnedTrash)
    {
        yield return new WaitForSeconds(timeBeforeEnablingSBO);
        spawnedTrash.GetComponent<TrashBehaviour>().enabled = true;
        spawnedTrash.GetComponent<Rigidbody>().useGravity = false;
        spawnedTrash.GetComponent<Rigidbody>().drag = 20;
        spawnedTrash.GetComponent<Rigidbody>().mass = 1f;
        spawnedTrash.transform.position = new Vector3(spawnedTrash.transform.position.x, 0, spawnedTrash.transform.position.z);
    }

}
