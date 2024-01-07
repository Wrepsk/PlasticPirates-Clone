using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;
using WaterSystem.Physics;

public class EnemyBehaviour : MonoBehaviour
{
    //Stats
    public float MaxHealth { get; } = 100;
    public float Health { get; private set; }

    //Actors and Helpers
    public GameObject playerObject;
    public NavMeshAgent agent;
    private bool isAggroed = false;
    private SimpleBuoyantObject simpleBuoyantObject;

    //Movement Helpers
    public Vector3 ownPosition;
    public Vector3 destination;
    public float turnspeed = 5.0f;
    public float aggroRange = 128f;

    //Vector Helpers
    private Vector3 playerPos;
    private Vector3 diffVector;
    private Vector3 directionDiffVec;
    private Vector3 initialDirection = new Vector3(1, 0, 0);

    //Physics Objects
    [SerializeField] Transform motorPosition;
    Rigidbody rb;

    //Smoke/Fire/Bubbles Animations
    public ParticleSystem smokeParticles;
    public ParticleSystem fireParticles;
    public ParticleSystem bubbles;
    private bool onFire;
    private bool smoking;
    private bool alive;

    //Sounds
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip bubbleSound;

    void Start()
    {
        // Finding player object without manually assigning it
        playerObject = GameObject.FindGameObjectWithTag("Player");
        //init variables
        rb = GetComponent<Rigidbody>();
        transform.eulerAngles = initialDirection;
        agent = GetComponent<NavMeshAgent>();
        simpleBuoyantObject = GetComponent<SimpleBuoyantObject>();
        VectorUpdate();

        onFire = false;
        smoking = false;
        alive = true;
        Health = MaxHealth;
    }

    private void Update()
    {

        float sixtyPercentHealth = MaxHealth * 0.6f;
        float thirtyPercentHealth = MaxHealth * 0.3f;

        // sinking animation
        if (Health == 0)
        {
            if (alive) StartDeathVisuals();

            alive = false;
            agent.enabled = false;
            simpleBuoyantObject.enabled = false;
            transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

            if (transform.position.y < -5) Destroy(gameObject);
        }
        else if (Health <= thirtyPercentHealth && !onFire)
        {
            StartFireVisuals();
        }
        else if (Health <= sixtyPercentHealth && !smoking)
        {
            StartSmokeVisuals();
        }
    }

    private void FixedUpdate()
    {
        if (Health == 0) return;


        VectorUpdate();
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

    void VectorUpdate()
    {
        //Updates posititon vectors and calculated vectors
        ownPosition = transform.position;
        playerPos = playerObject.transform.position;
        diffVector = playerPos - ownPosition;
        directionDiffVec = diffVector.normalized;

    }

    public void DealDamage(float damage)
    {
        Health = Mathf.Max(0f, Health - damage);
    }


    // Visuals

    private void StopFireAndSmoke()
    {
        if (audioSource != null) audioSource.Stop();
        if (fireParticles != null) fireParticles?.Stop();
        if (smokeParticles != null) smokeParticles?.Stop();
    }

    private void StartBubbles()
    {
        if (audioSource != null) audioSource.volume = 1f;
        if (audioSource != null) audioSource.PlayOneShot(bubbleSound);
        if (bubbles != null) bubbles.Play();
    }

    private void StartDeathVisuals()
    {
        if (fireParticles != null && !fireParticles.isPlaying)
        {
            fireParticles.Play();
            audioSource.clip = fireSound;
            audioSource.volume = 0.8f;
            audioSource.loop = true;
            audioSource.Play();
        }

        Invoke(nameof(StopFireAndSmoke), 0.7f);
        Invoke(nameof(StartBubbles), 1.35f);
    }

    private void StartFireVisuals()
    {
        if (smokeParticles != null) smokeParticles.Stop();
        if (fireParticles != null) fireParticles.Play();
        onFire = true;
        if (audioSource != null) audioSource.volume = 0.65f;
    }

    private void StartSmokeVisuals()
    {
        if (smokeParticles != null) smokeParticles.Play();
        smoking = true;
        
        if (audioSource != null)
        {
            audioSource.clip = fireSound;
            audioSource.loop = true;
            audioSource.volume = 0.3f;
            audioSource.Play();
        }
    }
}
