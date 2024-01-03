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
    public float armor = 100;
    public float defSpeed = 5.0f;

    //Actors and Helpers
    public UnityEvent onDeath = new UnityEvent();
    public bool isDead = false;
    //public SimpleBuoyantObject simpleBuoyantObject;
    
    //Movement Helpers
    public Vector3 ownPosition;

    //Vector Helpers
    protected Vector3 initialDirection = new Vector3(1,0,0);

    //Physics stuff
    public Ray sight;
    
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

    public void DeathAnimation()
    {
        transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

        if (transform.position.y < -5) Destroy(gameObject);
    }
    public void CheckIfDead()
    {
        if (health == 0 | health < 0)
        {
            Debug.Log("Dies");
            onDeath.Invoke();
            isDead = true;
        }
    }

    //getters and setters
    public float GetHealth() { return health; }
    public void SetHealth(float newHealth) {health = newHealth;}

    public float GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(float newMaxHealth) {maxHealth = newMaxHealth;}
}
