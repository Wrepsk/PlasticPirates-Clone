using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MinimapCamera : MonoBehaviour
{
[SerializeField] private Transform PlayerTransform;
[SerializeField] private float yoffset;
private void LateUpdate()
{
Vector3 targetPosition = PlayerTransform.position;
targetPosition.y = yoffset;
transform.position = targetPosition;
}
}