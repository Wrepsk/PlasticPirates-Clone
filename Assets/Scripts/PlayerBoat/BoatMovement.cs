using System;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMovement : MonoBehaviour
{
    PlayerControls controls;
    public Vector2 movementInput;
    Rigidbody rb;

    public AudioSource audioSource;
    public AudioClip idleClip;
    public AudioClip accClip;
    public AudioClip loopClip;
    public AudioClip stopClip;
    private Boolean idle;
    private Boolean acc;
    private Boolean loop;


    [SerializeField] Transform motorPosition;

    [Header("Boat Control parameters")]
    [SerializeField] float acceleration, maxSpeed, steeringStrength, maxAngularSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.BoatMovement.Move.performed += ctxt => OnMove(ctxt);
        controls.BoatMovement.Enable();

        audioSource.volume -= 0.3f;
        audioSource.clip = idleClip;
        audioSource.loop = true;
        idle = true;
        acc = false;
        loop = false;

        // Start playing the loop
        audioSource.Play();


        // See Unity Bug at the bottom of this class
        InvokeRepeating(nameof(TemporarySolutionFixRotation), 0.1f, 0.1f);
    }

    private void OnMove(InputAction.CallbackContext ctxt)
    {
        float horizontalInput = ctxt.ReadValue<Vector2>().x;
        float verticalInput = ctxt.ReadValue<Vector2>().y;

        if (idle) {
            audioSource.Stop();
            idle = false;
        }
        if (!acc)
        {
            acc = true;
            audioSource.PlayOneShot(accClip);
        }

        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            idle = true;
            acc = false;
            loop = false;
            audioSource.Stop();
            audioSource.PlayOneShot(stopClip);
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

        if (acc && !audioSource.isPlaying && !loop)
        {
            loop = true;
            audioSource.clip = loopClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if(idle && !audioSource.isPlaying && !loop)
        {
            audioSource.clip = idleClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

}
