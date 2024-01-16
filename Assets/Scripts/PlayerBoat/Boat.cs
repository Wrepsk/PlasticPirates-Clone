using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSystem.Physics;

public class Boat : Damagable
{
    private float speed;
    public float boostMultipliar = 4f;
    public float rotationSpeed = 90.0f; // 90 deg/s

    Transform rotatorTransform;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // We have a Rotator game object because 
        // SimpleBuoyantObject behavior makes us unable to
        // rotate the object normally. 
        //
        // Instead of rotating the boat, we rotate
        // the first child under it.

        rotatorTransform = transform.GetChild(0);
        speed = DefaultSpeed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.position += rotatorTransform.rotation * new Vector3(verticalInput * speed * Time.deltaTime , 0, 0);

        rotatorTransform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = DefaultSpeed * boostMultipliar;
        }
        else
            speed = DefaultSpeed;

        // TODO: Add collision logic.
    }

}