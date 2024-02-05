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
using System.Runtime.CompilerServices;
using Microsoft.Unity.VisualStudio.Editor;


public class NewCargoshipBehaviour : EnemyBehaviour
{
    //Trash Management
    [SerializeField] GameObject trashPoint;
    public float trashSpawnRate = 3f;
    private float currentTrashDelay;
    public float throwForce = 50f;
    public float trashMass = 10f;
    public float timeBeforeEnablingSBO = 1.5f;


    private bool rotated = false;
    public Vector3 moveTo;
    public bool stopped;

    protected override void EquipEquipment(int index) {}
    
    protected override void Start()
    {
        base.Start();
        currentTrashDelay = trashSpawnRate;
        agent.isStopped = false;
        SetRandomDestination();
    }

    protected override void Update()
    {
        //basic shit
        base.Update();
        if (IsDead)
        {
            agent.enabled = false;
            return;
        }
        base.Update();
        //spawn trash
        currentTrashDelay -= Time.deltaTime;
        if(currentTrashDelay <= 0)
        {
            SpawnTrashAtTheBack();
            currentTrashDelay = trashSpawnRate;
        }
    }
    protected override void FixedUpdate()
    {
        //checks if dead
        if (IsDead) 
        {
            healthbarDeath.enabled = true;
            healthbarComplete.enabled = false;
            healthbarForeground.enabled = false;
            return;
        }
        
        if (!agent.isOnNavMesh)
        {
            Vector3 warpPosition = GetRandomOnNavmesh(transform.position, 0, 10); //Set to position you want to warp to
            agent.transform.position = warpPosition;
            agent.enabled = false;
            agent.enabled = true;
        }

        VectorUpdate();
        stopped = agent.isStopped;

        isAggroed = diffVector.magnitude < aggroRange;

        //recalibrates destination if destination reached
        if (Vector3.Distance(moveTo, transform.position) <= 30.0f || !agent.hasPath)
        {
            agent.isStopped = false;
            SetRandomDestination();
        }

        if (isAggroed)
        {
            if (!alarmPlayed)
            {
                //Alarm signal to player
                audioSource.clip = alarm;
                audioSource.volume = alarmVolume;
                audioSource.loop = false;
                audioSource.Play();
                alarmPlayed = true;
            }
            
            //Updates the healthbar
            healthbarComplete.enabled = true;
            healthbarForeground.enabled = true;
            UpdateHealthBar(MaxHealth, Health);
        }
        else
        {
            healthbarComplete.enabled = false;
            healthbarForeground.enabled = false;
        }

        Vector3 _lookdirection = moveTo - transform.position;
        TurnToPlayer(transform, turnspeed, _lookdirection);
    }


    public Vector3 GetRandomOnNavmesh(Vector3 centerPosition, float minDistance = 60f, float maxDistance = 110f) {
        //generate initial position
        Vector2 centerPosition2d = new Vector2(centerPosition.x, centerPosition.z);
        bool goodSpot = false;
        int searchradius = 100;
        Vector3 navPosition = centerPosition;
        while(!goodSpot)
        {
            Vector2 initPosition = centerPosition2d + UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minDistance, maxDistance);
            //move onto navmesh
            navPosition = new Vector3(initPosition.x, 0, initPosition.y);
            if (UnityEngine.AI.NavMesh.SamplePosition(navPosition, out UnityEngine.AI.NavMeshHit navHit, searchradius, UnityEngine.AI.NavMesh.AllAreas))
            {
                if (-1f <= navHit.position.y && 1f >= navHit.position.y)
                {
                    goodSpot = true;
                    navPosition = new Vector3(navHit.position.x, 0, navHit.position.z);
                }
            }
            minDistance += 2;
            maxDistance += 2;
            searchradius += 2;
            if (searchradius >= 200) goodSpot = true;
        }
        return navPosition;
    }
    private void SetRandomDestination()
    {
        moveTo = GetRandomOnNavmesh(transform.position);
        agent.destination = moveTo;
        Debug.Log("Destination Set");
        //agent.isStopped = true;
        //rotated = false;
        //CheckPointOnPath();
    }
    /*
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
    */
    private void SpawnTrashAtTheBack()
    {
        int type = UnityEngine.Random.Range(0, TrashManager.instance.meshPrefabs.Length);

        Trash trash = TrashManager.instance.CreateTrash(trashPoint.transform.position, type);
        trash.ignoreDematerialize = true;
        GameObject spawnedTrash = trash.Materialize();

        //spawnedTrash.GetComponent<TrashBehaviour>().enabled = false;
        spawnedTrash.GetComponent<TrashBehaviour>().spawnedByCargoShip = true;
        spawnedTrash.GetComponent<Rigidbody>().useGravity = true;
        spawnedTrash.GetComponent<Rigidbody>().drag = 0;
        spawnedTrash.GetComponent<Rigidbody>().mass = trashMass;
        spawnedTrash.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, throwForce));
        //StartCoroutine(EnableBuoyantObjectForTrash(spawnedTrash));
    }

    /*
    IEnumerator EnableBuoyantObjectForTrash(GameObject spawnedTrash)
    {
        yield return new WaitForSeconds(timeBeforeEnablingSBO);
        spawnedTrash.GetComponent<TrashBehaviour>().enabled = true;
        spawnedTrash.GetComponent<Rigidbody>().useGravity = false;
        spawnedTrash.GetComponent<Rigidbody>().drag = 20;
        spawnedTrash.GetComponent<Rigidbody>().mass = 1f;
        spawnedTrash.transform.position = new Vector3(spawnedTrash.transform.position.x, 0, spawnedTrash.transform.position.z);
    }
    */

}