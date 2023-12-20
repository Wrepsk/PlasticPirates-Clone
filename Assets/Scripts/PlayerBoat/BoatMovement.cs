using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMovement : MonoBehaviour
{
    PlayerControls controls;
    public Vector2 movementInput;
    Rigidbody rb;

    public bool inUpgradeIsland;

    [SerializeField] Transform motorPosition;

    [Header("Boat Control parameters")]
    [SerializeField] float acceleration, maxSpeed, steeringStrength, maxAngularSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.BoatMovement.Move.performed += ctxt => OnMove(ctxt);
        controls.BoatMovement.Enable();


        // See Unity Bug at the bottom of this class
        InvokeRepeating(nameof(TemporarySolutionFixRotation), 0.1f, 0.1f);
    }

    private void OnMove(InputAction.CallbackContext ctxt)
    {
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

}
