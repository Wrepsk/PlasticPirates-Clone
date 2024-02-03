using System;
using UnityEngine;
using UnityEngine.UIElements;
using WaterSystem.Physics;

public class Damagable : MonoBehaviour
{
    //Stats
    [field: SerializeField]
    public float MaxHealth { get; set; } = 100;
    public float Health { get; set; }
    [field: SerializeField]
    public float Armor { get; set; } = 0;
    [field: SerializeField]
    public float DefaultSpeed { get; set; } = 100.0f;

    //Actors and Helpers
    public event Action OnDeath;
    public event Action<float> HealthChanged;
    public bool IsDead => Health <= 0;
    

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

    //Trash manager for spawning after death
    public TrashManager trashManager; //assigned at start
    public int trashDropAmount { get; set; } = 5;
    private float objectHeigth;

    protected virtual void Start()
    {
        trashManager = FindObjectsOfType<TrashManager>()[0];
        objectHeigth = gameObject.GetComponentsInChildren<MeshRenderer>()[0].bounds.size.y;
        if (objectHeigth < 1) objectHeigth = 10;
        
        Health = MaxHealth;
        alive = true;
    }
    
    protected virtual void Update()
    {

        float sixtyPercentHealth = MaxHealth * 0.6f;
        float thirtyPercentHealth = MaxHealth * 0.3f;

        // sinking animation
        if (Health == 0)
        {
            if (alive)
            {
                StartDeathVisuals();
                alive = false;
            }

            DeathAnimation();
        }
        else if (Health <= thirtyPercentHealth && !onFire)
        {
            StartFireVisuals();
        }
        else if (Health <= sixtyPercentHealth && !smoking)
        {
            StartSmokeVisuals();
        }
        else if (Health > sixtyPercentHealth && (onFire || smoking)) 
        {	
            StopFireAndSmoke();
        }
    }

    public void DealDamage(float damage)
    {
        float _damage = (damage * (100f - Armor)) / 100f;

        Health = Mathf.Max(0f, Health - _damage);
        Debug.Log("health left: " + Health);

        if (Health <= 0 & alive) OnDeath?.Invoke();
        HealthChanged?.Invoke(Health);
    }

    public void DeathAnimation()
    {
        transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

        if (transform.position.y < -Mathf.Max(objectHeigth, 4))
        {
            trashManager.SpawnRandomTrashWithinCube(new Vector2(transform.position.x, transform.position.z),
                    new Vector3(10, 0, 5), 1, trashDropAmount, 10);
            Debug.Log("Trash spawn function went through");
            Destroy(gameObject);
        }
        
    }


    // Visuals

    private void StopFireAndSmoke()
    {
        if (audioSource != null) audioSource.Stop();
        if (fireParticles != null) fireParticles?.Stop();
        if (smokeParticles != null) smokeParticles?.Stop();

        onFire = false;
        smoking = false;
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
            audioSource.volume = 0.35f;
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
        if (audioSource != null) audioSource.volume = 0.3f;
    }

    private void StartSmokeVisuals()
    {
        if (smokeParticles != null) smokeParticles.Play();
        smoking = true;

        if (audioSource != null)
        {
            audioSource.clip = fireSound;
            audioSource.loop = true;
            audioSource.volume = 0.15f;
            audioSource.Play();
        }
    }
}
