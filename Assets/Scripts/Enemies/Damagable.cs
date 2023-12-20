using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using WaterSystem.Physics;

public class Damagable : MonoBehaviour
{
    //Stats
    public float health = 100;
    public float maxHealth = 100;

    //Actors and Helpers
    public UnityEvent onDeath = new UnityEvent();
    public bool isDead = false;
    //public SimpleBuoyantObject simpleBuoyantObject;
    
    //Movement Helpers
    public Vector3 ownPosition;

    //Vector Helpers
    protected Vector3 initialDirection = new Vector3(1,0,0);

    void Start()
    {
        transform.eulerAngles = initialDirection;
        
        //simpleBuoyantObject = GetComponent<SimpleBuoyantObject>();
        
    }

    private void Update()
    {
        //checking if ded
        if (!isDead) checkIfDead();
    }
    
    public Quaternion RotateQuaternionLeft(Quaternion original, float angleDegrees)
    {

        // Calculate the rotation quaternion
        Quaternion rotationQuaternion = Quaternion.Euler(0f, -angleDegrees, 0f);

        // Multiply the original quaternion by the rotation quaternion
        Quaternion rotatedQuaternion = rotationQuaternion * original;

        return rotatedQuaternion;
    }
    

    public void DealDamage(float damage)
    {
        health = Mathf.Max(0f, health - damage);
        Debug.Log("health left: " + health);
    }

    public void deathAnimation()
    {
        transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

        if (transform.position.y < -5) Destroy(gameObject);
    }
    public void checkIfDead()
    {
        if (health == 0 | health < 0)
        {
            onDeath.Invoke();
            isDead = true;
            //simpleBuoyantObject.enabled = false;
            
        }
    }

    //getters and setters
    public float getHealth() { return health; }
    public void setHealth(float newHealth) {health = newHealth;}

    public float getMaxHealth() { return maxHealth; }
    public void setMaxHealth(float newMaxHealth) {maxHealth = newMaxHealth;}
}
