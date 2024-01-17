using System;
using UnityEngine;
using UnityEngine.UIElements;
using WaterSystem.Physics;

public class Damagable : MonoBehaviour
{
    //Stats
    [field: SerializeField]
    public float MaxHealth { get; set; } = 100;
    public float Health { get; private set; }
    [field: SerializeField]
    public float Armor { get; set; } = 100;
    [field: SerializeField]
    public float DefaultSpeed { get; set; } = 5.0f;

    //Actors and Helpers
    public event Action OnDeath;
    public event Action<float> HealthChanged;
    public bool IsDead => Health <= 0;
    //public SimpleBuoyantObject simpleBuoyantObject;
    

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

    protected virtual void Start()
    {
        Debug.Log(MaxHealth);
        Health = MaxHealth;
    }
    
    protected virtual void Update()
    {

        float sixtyPercentHealth = MaxHealth * 0.6f;
        float thirtyPercentHealth = MaxHealth * 0.3f;

        // sinking animation
        if (Health == 0)
        {
            if (alive) StartDeathVisuals();

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
        Health = Mathf.Max(0f, Health - damage);
        Debug.Log("health left: " + Health);

        if (damage <= 0) OnDeath?.Invoke();
        HealthChanged?.Invoke(Health);
    }

    public void DeathAnimation()
    {
        transform.position -= new Vector3(0, 1 * Time.deltaTime, 0);

        if (transform.position.y < -5) Destroy(gameObject);
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
