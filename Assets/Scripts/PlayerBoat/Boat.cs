using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSystem.Physics;

public class Boat : MonoBehaviour
{
    public float defSpeed = 5.0f; // 5 m/s
    private float speed;
    public float boostMultipliar = 4f;
    public float rotationSpeed = 90.0f; // 90 deg/s

    Transform rotatorTransform;

    // Start is called before the first frame update
    void Start()
    {
        // We have a Rotator game object because 
        // SimpleBuoyantObject behavior makes us unable to
        // rotate the object normally. 
        //
        // Instead of rotating the boat, we rotate
        // the first child under it.

        rotatorTransform = transform.GetChild(0);
        speed = defSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.position += rotatorTransform.rotation * new Vector3(verticalInput * speed * Time.deltaTime , 0, 0);

        rotatorTransform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = defSpeed * boostMultipliar;
        }
        else
            speed = defSpeed;

        // TODO: Add collision logic.
    }
}