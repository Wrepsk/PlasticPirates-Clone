using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float mouseSensitivity = 350f;
    public Transform target;
    public float distanceFromTarget = 3f;

    float rotationY;
    float rotationX;

    Vector3 currentRotation;
    Vector3 smoothVelocity = Vector3.zero;
    float smoothTime = 0.2f;


    private void Start() {
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -10, 45f);
        Vector3 nextRotation = new Vector3(rotationX, rotationY);
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
        transform.localEulerAngles = currentRotation;
        transform.position = (target.position - transform.forward * distanceFromTarget) + new Vector3(0, 2.5f, 0);
    }
}
