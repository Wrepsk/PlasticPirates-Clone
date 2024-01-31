using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using WaterSystem.Physics;
using Microsoft.Unity.VisualStudio.Editor;
using System.Runtime.CompilerServices;

public class EnemyBehaviour : Damagable
{
    //Actors and Helpers
    public GameObject playerObject;
    public NavMeshAgent agent;
    public bool isAggroed = false;
    private SimpleBuoyantObject simpleBuoyantObject;

    //Healthbar Helpers
    [SerializeField]
    private UnityEngine.UI.Image healthbarForeground;   // Sprite of the health indicator
    [SerializeField]
    private UnityEngine.UI.Image healthbarComplete;     // Sprite of the healthbar
    [SerializeField]
    private UnityEngine.UI.Image healthbarDeath;        // Sprite of Deathmarker
    [SerializeField]
    private AudioClip alarm;
    private bool alarmPlayed = false;                   //Indicator if indicator sound was played
    [SerializeField]
    private float alarmVolume = 1.6f;                     //Volume of alarm sound

    //Movement Helpers
    public Vector3 destination;
    public float turnspeed = 5.0f;
    public float aggroRange = 128f;

    //Vector Helpers
    private Vector3 playerPos;
    private Vector3 diffVector;
    private Vector3 directionDiffVec;
    private Vector3 initialDirection = new Vector3(1, 0, 0);

    //Movement Helpers
    public Vector3 ownPosition;

    //Physics stuff
    public Ray sight;

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

    
    // Setting the healthbar
    public void UpdateHealthBar(float healthMax, float currentHealth)
    {
        healthbarForeground.fillAmount = currentHealth / healthMax;
        //healthCanvas.transform.rotation = _cam.transform.rotation;
    }


    protected void Awake()
    {
        //init navigation vars
        agent = GetComponent<NavMeshAgent>();
        agent.speed = DefaultSpeed;
        agent.acceleration = DefaultSpeed;
    }

    protected override void Start()
    {
        base.Start();

        //Gets initial Equipment
        EquipEquipment(0);  

        //Turn off healthbar at start
        healthbarComplete.enabled = false;
        healthbarForeground.enabled = false;
        healthbarDeath.enabled = false;

        // Finding player object without manually assigning it
        playerObject = GameObject.FindGameObjectWithTag("Player");
        
        //sets object in initial direction
        transform.eulerAngles = initialDirection;
        
        //initiates deathListener
        OnDeath += DeathDisable;

        //rigidbody
        rb = GetComponent<Rigidbody>();
        
        VectorUpdate();
    }

    protected override void Update()
    {
        base.Update();


        if (IsDead)
        {
            agent.enabled = false;
        }

    }
    private void FixedUpdate()
    {
        //checks if dead
        if (IsDead) 
        {
            healthbarDeath.enabled = true;
            healthbarComplete.enabled = false;
            healthbarForeground.enabled = false;
            
            if (equipments[equipmentIndex] != null) equipments[equipmentIndex].BaseStopUse();
            return;
        }


        VectorUpdate();

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

        isAggroed = diffVector.magnitude < aggroRange;


        //recalibrates destination if player moves
        if (Vector3.Distance(destination, playerPos) > 10.0f & isAggroed)
        {
            agent.isStopped = false;
            agent.destination = playerPos;
        } else
        {
            agent.isStopped = true;
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
            
            //Turns enemy in direction of player by turnspeed
            TurnToPlayer(transform, turnspeed, direction:diffVector);
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

        //Handles turning and shooting of weapon
        if (diffVector.magnitude < aggroRange / 2 && equipments[equipmentIndex] != null)
        {
            Vector3 weaponToPlayer = playerPos - equipmentMover.transform.position;
            TurnToPlayer(equipmentMover.transform, weaponTurnSpeed, weaponToPlayer, -0.5f, -90f);
            equipments[equipmentIndex].BaseUse();
        } else
        {
            equipments[equipmentIndex].BaseStopUse();
        }

    }

    public Quaternion RotateQuaternionLeft(Quaternion original, float angleDegrees)
    {

        // Calculate the rotation quaternion
        Quaternion rotationQuaternion = Quaternion.Euler(0f, -angleDegrees, 0f);

        // Multiply the original quaternion by the rotation quaternion
        Quaternion rotatedQuaternion = rotationQuaternion * original;

        return rotatedQuaternion;
    }

    //Turns enemy in direction of player by turnspeed
    public void TurnToPlayer(Transform toRotate, float specificTurnspeed, Vector3 direction, float yOffset = 0.0f, float rotateLeft = 0f)
    {

        Vector3 goalVec = direction + new Vector3(0,yOffset,0);
        Quaternion _lookRotation = Quaternion.LookRotation(goalVec);

        // weird unity forward directions... ughhh
        if (rotateLeft != 0f) _lookRotation = RotateQuaternionLeft(_lookRotation, rotateLeft);

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
