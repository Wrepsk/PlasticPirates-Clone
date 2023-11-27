using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMovement : MonoBehaviour
{
    PlayerControls controls;
    public Vector2 movementInput;
    Rigidbody rb;


    [SerializeField] Transform motorPosition;

    [Header("Boat Control parameters")]
    [SerializeField] float acceleration, maxSpeed, steeringStrength, maxAngularSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.BoatMovement.Move.performed += ctxt => OnMove(ctxt);
        controls.BoatMovement.Enable();
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
}
