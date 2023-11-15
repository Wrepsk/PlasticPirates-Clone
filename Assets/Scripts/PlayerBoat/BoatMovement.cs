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
    [SerializeField] float acceleration, maxSpeed;
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
        var forceVec = transform.forward * movementInput.y * acceleration;

        rb.AddForceAtPosition(forceVec, motorPosition.position, ForceMode.Force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
