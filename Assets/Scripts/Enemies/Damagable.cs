using UnityEngine;
using WaterSystem.Physics;

public class Damagable : MonoBehaviour
{
    //Stats
    public float health = 100;
    public float maxHealth = 100;


    //Actors and Helpers
    private SimpleBuoyantObject simpleBuoyantObject;
    
    //Movement Helpers
    public Vector3 ownPosition;

    //Vector Helpers
    private Vector3 initialDirection = new Vector3(1,0,0);

    void Start()
    {
        transform.eulerAngles = initialDirection;
        
        simpleBuoyantObject = GetComponent<SimpleBuoyantObject>();
        
    }

    private void Update()
    {
        // sinking animation
        if (health == 0)
        {
            simpleBuoyantObject.enabled = false;
            transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

            if (transform.position.y < -5) Destroy(gameObject);
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
    

    public void DealDamage(float damage)
    {
        health = Mathf.Max(0f, health - damage);
    }

    public void setHealth(float newHealth)
    {
        health = newHealth;
    }

    public float getHealth()
    {
        return health;
    }
}
