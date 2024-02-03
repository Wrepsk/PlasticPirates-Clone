using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMovement : Damagable
{
    PlayerControls controls;
    public Vector2 movementInput;
    Rigidbody rb;

    public bool inUpgradeIsland;


    public AudioSource motorAudioSource;
    public AudioClip idleClip;
    public AudioClip accClip;
    public AudioClip fullPowerClip;
    public AudioClip stopClip;
    public AudioClip trashCollectingClip;
    private bool idle;
    private bool fullPower;
    private bool invokedIdle;
    private bool invokedFullPower;


    [SerializeField] Transform motorPosition;

    [Header("Boat Control parameters")]
    [SerializeField] float acceleration, maxSpeed, steeringStrength, maxAngularSpeed;

    [SerializeField] AnimationCurve massMult;

    float ogMass;
    protected override void Start()
    {
        base.Start();

        PreloadAudioClips();

        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.BoatMovement.Move.performed += ctxt => OnMove(ctxt);
        controls.BoatMovement.Enable();

        if (motorAudioSource != null)
        {

            motorAudioSource.volume -= 0.65f;
            motorAudioSource.clip = idleClip;
            motorAudioSource.loop = true;
            motorAudioSource.Play();
        }
        idle = true;
        fullPower = false;
        invokedIdle = false;

        ogMass = rb.mass;
    }

    protected override void Update()
    {
        base.Update();
        TemporarySolutionFixRotation();
        maxSpeed = DefaultSpeed;
    }

    private void PreloadAudioClips() {
        if (idleClip != null)
            idleClip.LoadAudioData();
        if (accClip != null)
            accClip.LoadAudioData();
        if (fullPowerClip != null)
            fullPowerClip.LoadAudioData();
        if (stopClip != null)
            stopClip.LoadAudioData();
        if (trashCollectingClip != null)
            trashCollectingClip.LoadAudioData();
    }

    private void OnMove(InputAction.CallbackContext ctxt)
    {
        float verticalInput = ctxt.ReadValue<Vector2>().y;
        float horizontalInput = ctxt.ReadValue<Vector2>().x * Mathf.Sign(verticalInput);

        if (HUD.instance.HudActive)
        {
            verticalInput = 0;
            horizontalInput = 0;
        }

        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            if (fullPower)
            {
                idle = true;
                fullPower = false;

                if (motorAudioSource != null)
                {
                    motorAudioSource.Stop();

                    if (stopClip != null) motorAudioSource.PlayOneShot(stopClip);
                    float delay = stopClip != null ? stopClip.length : 0f;

                    if (!invokedIdle)
                    {
                        Invoke("PlayIdle", delay);
                        invokedIdle = true;
                    }
                    else
                    {
                        CancelInvoke("PlayIdle");
                        Invoke("PlayIdle", delay);
                    }
                }
            }
        }
        else
        {
            if (idle)
            {
                idle = false;
                fullPower = true;

                if (motorAudioSource != null && accClip != null )
                {
                    motorAudioSource.Stop();
                    motorAudioSource.PlayOneShot(accClip);
                    float delay = accClip.length;
                    if (!invokedFullPower)
                    {
                        Invoke("PlayFullPower", delay);
                        invokedFullPower = true;
                    }
                    else
                    {
                        CancelInvoke("PlayFullPower");
                        Invoke("PlayFullPower", delay);
                    }
                }
            }

        }

        movementInput = new Vector2(horizontalInput, verticalInput).normalized;
    }


    private void FixedUpdate()
    {
        rb.mass = ogMass * massMult.Evaluate((float)(StatsManager.instance.CollectedTrash + 0.001f) / (float)StatsManager.instance.maxTrashCapacity ) + ogMass;

        var forceVec = Vector3.Scale(new Vector3(1,0,1), transform.forward) * movementInput.y * acceleration + Vector3.Scale(new Vector3(1, 0, 1), transform.right) * -movementInput.x * steeringStrength;
        //var forceVec = Vector3.Scale(new Vector3(1,0,1), transform.forward) * movementInput.y * acceleration;

        rb.AddForceAtPosition(forceVec, motorPosition.position, ForceMode.Force);

        //rb.AddTorque(Vector3.up * movementInput.x * steeringStrength);
        
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if(rb.angularVelocity.magnitude > maxAngularSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeed;
        }

    }


    // Unity Bug:
    // Rigidbody3D freeze rotation on X and Z axes
    // do not work when applying force to the boat.
    // a temporary workaround is to forcefuly update
    // the rotation to be 0 on X and Z axes.
    // the Y axis is independent.
    private void TemporarySolutionFixRotation()
    {
        transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "UpgradeIsland")
        {
            inUpgradeIsland = true;
        }
        else if (other.tag == "Trash")
        {
            motorAudioSource.PlayOneShot(trashCollectingClip, 0.45f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "UpgradeIsland")
        {
            inUpgradeIsland = false;
        }
    }

    private void PlayIdle()
    {
        invokedIdle = false;
        if (idle && motorAudioSource != null)
        {
            motorAudioSource.clip = idleClip;
            motorAudioSource.loop = true;
            motorAudioSource.Play();
        }
    }

    private void PlayFullPower()
    {
        invokedFullPower = false;
        if (fullPower && motorAudioSource != null)
        {
            motorAudioSource.clip = fullPowerClip;
            motorAudioSource.loop = true;
            motorAudioSource.Play();
        }
    }
}
