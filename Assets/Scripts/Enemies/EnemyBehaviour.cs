using UnityEngine;
using UnityEngine.AI;
using WaterSystem.Physics;

public class EnemyBehaviour : Damagable
{
    //Stats
    


    //Actors and Helpers
    public GameObject playerObject;
    public NavMeshAgent agent;
    private bool isAggroed = false;
    private SimpleBuoyantObject simpleBuoyantObject;
    
    //Movement Helpers
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
        simpleBuoyantObject = GetComponent<SimpleBuoyantObject>();
        VectorUpdate();
        
    }

    private void Update()
    {
        // sinking animation
        if (health == 0)
        {
            agent.enabled = false;
            simpleBuoyantObject.enabled = false;
            transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

            if (transform.position.y < -5) Destroy(gameObject);
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
    
    void VectorUpdate()
    {
        //Updates posititon vectors and calculated vectors
        ownPosition = transform.position;
        playerPos = playerObject.transform.position;
        diffVector = playerPos - ownPosition;
        directionDiffVec = diffVector.normalized;

    }

}
