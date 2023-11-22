// Focuses Camera on target
using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour 
{
    public Transform target;

    void Update () {
        transform.LookAt(target);
    }
}