using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTilter : MonoBehaviour
{
    // The rigidbody of the boat
    public Rigidbody rigidbody;

    // Maps velocity to tilt
    public AnimationCurve tiltCurve = AnimationCurve.EaseInOut(2.5f, 0f, 10f, 8f);
    
    // Maps velocity to y offset
    public AnimationCurve offsetCurve = AnimationCurve.EaseInOut(2.5f, 0f, 10f, 0.1f);

    public float targetTilt;

    // Player
    private Damagable _player;


    void Start()
    {
        _player = GetComponentInParent<Damagable>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_player.Health <= 0) return;

        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        float velocity = rigidbody.velocity.magnitude;

        targetTilt = -1 * tiltCurve.Evaluate(velocity);
        float targeOffeset = offsetCurve.Evaluate(velocity);

        // We don't need any extra, time-dependent interpolation here (i think)
        // Because the rigidbody / velocity is already interpolated
        transform.eulerAngles = new Vector3(targetTilt, rotation.eulerAngles.y, rotation.eulerAngles.z);
        transform.position = new Vector3(position.x, targeOffeset, position.z);
    }
}
