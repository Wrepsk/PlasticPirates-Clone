using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;
using WaterSystem.Physics;

public class EnemyBehaviour : MonoBehaviour
{
    //Stats
    public float health = 100;

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
    private Vector3 initialDirection = new Vector3(1,0,0);

    //Physics Objects
    [SerializeField] Transform motorPosition;
    Rigidbody rb;

    //Smoke/Fire/Bubbles Animations
    public ParticleSystem SmokeParticles;
    public ParticleSystem FireParticles;
    public ParticleSystem Bubbles;
    private float sixtyPercentHealth;
    private float thirtyPercentHealth;
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

        sixtyPercentHealth = health * 0.6f;
        thirtyPercentHealth = health * 0.3f;
        onFire = false;
        smoking = false;
        alive = true;
    }

    private void Update()
    {
        // sinking animation
        if (health == 0)
        {
            if (alive)
            {
                if (!FireParticles.isPlaying)
                {
                    FireParticles.Play();
                    audioSource.clip = fireSound;
                    audioSource.volume = 0.8f;
                    audioSource.loop = true;
                    audioSource.Play();
                }
                Invoke("stopFireAndSmoke", 0.7f);
                Invoke("startBubbles", 1.35f);
                alive = false;
            }
            
            agent.enabled = false;
            simpleBuoyantObject.enabled = false;
            transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

            if (transform.position.y < -5) Destroy(gameObject);
        }
        else if (health <= thirtyPercentHealth && !onFire)
        {
            SmokeParticles.Stop();
            FireParticles.Play();
            onFire = true;
            audioSource.volume = 0.65f;
        }
        else if (health <= sixtyPercentHealth && !smoking)
        {
            SmokeParticles.Play();
            smoking = true;
            audioSource.clip = fireSound;
            audioSource.loop = true;
            audioSource.volume = 0.3f;
            audioSource.Play();
        }
    }

    private void FixedUpdate()
    {
        if (health == 0) return;


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
        health = Mathf.Max(0f, health - damage);
    }

    private void stopFireAndSmoke()
    {
        audioSource.Stop();
        FireParticles.Stop();
        SmokeParticles.Stop();
    }

    private void startBubbles()
    {
        audioSource.volume = 1f;
        audioSource.PlayOneShot(bubbleSound);
        Bubbles.Play();
    }
}
