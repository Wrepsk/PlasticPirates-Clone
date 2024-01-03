using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System;
using WaterSystem.Physics;

public class EnemyBehaviour : Damagable
{
    //Actors and Helpers
    public GameObject playerObject;
    public NavMeshAgent agent;
    private bool isAggroed = false;
    
    //Movement Helpers
    public Vector3 destination;
    public float turnspeed = 5.0f;
    public float aggroRange = 128f;

    //Vector Helpers
    private Vector3 playerPos;
    private Vector3 diffVector;
    private Vector3 directionDiffVec;
    
    
    //Physics Objects
    public bool CanSeePlayer = false;
    [SerializeField] Transform motorPosition;
    Rigidbody rb;

    //Shooting helpers
    protected Transform weaponPosition;
    [SerializeField] Transform equipmentMover;
    float weaponTurnSpeed = 10;
    [SerializeField] Equipment[] equipments;
    int equipmentIndex;
    int previousEquipmentIndex = -1;
    long unixTimeLastShot = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
    

    void DeathDisable()
    {
        agent.enabled = false;
    }

    void EquipEquipment(int index)
    {
        if (index == previousEquipmentIndex)
            return;

        equipmentIndex = index;
        equipments[equipmentIndex].equipmentGameObject.SetActive(true);

        if(previousEquipmentIndex != -1)
        {
            equipments[previousEquipmentIndex].equipmentGameObject.SetActive(false);
        }

        previousEquipmentIndex = equipmentIndex;
    }

    void Start()
    {
        //Gets initial Equipment
        EquipEquipment(0);  

        // Finding player object without manually assigning it
        playerObject = GameObject.FindGameObjectWithTag("Player");
        
        //sets object in initial direction
        transform.eulerAngles = initialDirection;
        
        //init navigation vars
        agent = GetComponent<NavMeshAgent>();
        agent.speed = defSpeed;
        agent.acceleration = defSpeed;

        
        //initiates deathListener
        onDeath.AddListener(DeathDisable);

        //rigidbody
        rb = GetComponent<Rigidbody>();
        
        VectorUpdate();
        
    }
    private void Update()
    {
        //Is the Enemy dead?
        if (!isDead) CheckIfDead();

        //Handles turning and shooting of weapon
        TurnToPlayer(equipmentMover.transform, weaponTurnSpeed, -0.5f);
        DateTime currentTime = DateTime.UtcNow;
        long unixTimeNow = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        
        if(equipments[equipmentIndex] != null)
        {
            if (unixTimeNow - unixTimeLastShot >= equipments[equipmentIndex].equipmentInfo.cooldown + UnityEngine.Random.Range(0.0f, 2.0f)) {
                unixTimeLastShot = unixTimeNow;
            }
            if (equipments[equipmentIndex].equipmentInfo.isAutomatic && unixTimeNow == unixTimeLastShot && CanSeePlayer){
                equipments[equipmentIndex].Use();
                //Debug.Log("player hit with:" + equipments[equipmentIndex]);
                //Debug.Log("Player health:" + playerObject.GetComponent<BoatMovement>().health);
            }else if(unixTimeNow == unixTimeLastShot && CanSeePlayer)
                    equipments[equipmentIndex].Use();
        }
    }
    private void FixedUpdate()
    {
        //checks if dead
        if (isDead) 
        {
            DeathAnimation();
            return;
        }


        VectorUpdate();

        //recalibrates destination if player moves
        if (Vector3.Distance(destination, playerPos) > 10.0f & isAggroed) 
        {
            agent.destination = playerPos;
            
        }

        //checks if path interrupted by Environment
        sight = new Ray(ownPosition, diffVector);
        Debug.DrawRay(ownPosition, diffVector);
        if (Physics.Raycast(sight, out RaycastHit hit))
        {
            //Debug.Log(hit.collider);
            if (hit.collider.CompareTag("Environment"))
            {
                CanSeePlayer = false;
            }
            else if (hit.collider.CompareTag("Player"))
            {
                CanSeePlayer = true;
            }
        }
        
        //checks if should be aggroed
        if (diffVector.magnitude < aggroRange)
        {
            agent.destination = playerPos;
            isAggroed = true;
            
        }
        
        //Turns enemy in direction of player by turnspeed
        TurnToPlayer(transform.GetChild(0), turnspeed);
        
    }
    
    //Turns enemy in direction of player by turnspeed
    public void TurnToPlayer(Transform toRotate, float specificTurnspeed, float yOffset = 0.0f)
    {
        Vector3 goalVec = diffVector + new Vector3(0,yOffset,0);
        Quaternion _lookRotation = Quaternion.LookRotation(goalVec);
        _lookRotation = RotateQuaternionLeft(_lookRotation, 90);
        toRotate.rotation = Quaternion.Slerp(toRotate.rotation, _lookRotation, Time.deltaTime * specificTurnspeed);
    }
    void VectorUpdate()
    {
        //Updates posititon vectors and calculated vectors
        ownPosition = transform.position;
        playerPos = playerObject.transform.position;
        diffVector = playerPos - ownPosition;
        directionDiffVec = diffVector.normalized;
    }
}
