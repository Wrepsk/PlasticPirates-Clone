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
    private bool idle;
    private bool fullPower;
    private bool invokedIdle;
    private bool invokedFullPower;


    [SerializeField] Transform motorPosition;

    [Header("Boat Control parameters")]
    [SerializeField] float acceleration, maxSpeed, steeringStrength, maxAngularSpeed;
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.BoatMovement.Move.performed += ctxt => OnMove(ctxt);
        controls.BoatMovement.Enable();

        if (audioSource != null)
        {

            audioSource.volume -= 0.3f;
            audioSource.clip = idleClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        idle = true;
        fullPower = false;
        invokedIdle = false;

        // Start playing the loop

        // See Unity Bug at the bottom of this class
        InvokeRepeating(nameof(TemporarySolutionFixRotation), 0.1f, 0.1f);
    }

    private void OnMove(InputAction.CallbackContext ctxt)
    {
        float horizontalInput = ctxt.ReadValue<Vector2>().x;
        float verticalInput = ctxt.ReadValue<Vector2>().y;

        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            if (fullPower)
            {
                idle = true;
                fullPower = false;

                if (audioSource != null)
                {
                    audioSource.Stop();

                    if (stopClip != null) audioSource.PlayOneShot(stopClip);
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

                if (audioSource != null && accClip != null )
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(accClip);
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

        movementInput = ctxt.ReadValue<Vector2>().normalized;
    }


    private void FixedUpdate()
    {
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
        if (idle && audioSource != null)
        {
            audioSource.clip = idleClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void PlayFullPower()
    {
        invokedFullPower = false;
        if (fullPower && audioSource != null)
        {
            audioSource.clip = fullPowerClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
