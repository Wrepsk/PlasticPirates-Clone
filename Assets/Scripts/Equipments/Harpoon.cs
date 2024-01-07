using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : Equipment
{
    private Vector3 grapplePoint;
    private SpringJoint joint;
    private bool harpoonShot, lineReached, hookedToBack;
    private float elapsedFrames = 0;
    private Vector3 pos;
    private Transform currentlyHarpoonedGameObject;

    public LineRenderer lineRenderer;
    public Transform ropePoint, boat, hooksPoint;
    public LayerMask grappableLayers;
    public float maxHarpoonDistance = 50f;


    private void Update()
    {
        if(!hookedToBack && harpoonShot && !transform.GetChild(0).gameObject.activeSelf)
        {
            RetreatHarpoon();
        }

        if(Input.GetKeyDown(KeyCode.F) && harpoonShot)
        {
            hookedToBack = true;
        }
    }
    private void LateUpdate()
    {
        DrawRope();
    }

    public override void Use()
    {
        if(!harpoonShot)
            ShootHarpoon();
        else
            RetreatHarpoon();
    }

    void DrawRope()
    {
        if (joint == null)
            return;

        float interpolationRatio = (float)elapsedFrames / 45;
        elapsedFrames = ((elapsedFrames + (200 * Time.deltaTime)) % (45 + 1));

        Vector3 startPosition = ropePoint.position;
        Vector3 endPosition = currentlyHarpoonedGameObject.position;

        if(hookedToBack)
            lineRenderer.SetPosition(0, hooksPoint.position);
        else
            lineRenderer.SetPosition(0, ropePoint.position);
        
        if(lineReached)
        {
            lineRenderer.SetPosition(1, currentlyHarpoonedGameObject.position);
            return;
        }
        pos = Vector3.Lerp(startPosition, endPosition, interpolationRatio);
        lineRenderer.SetPosition(1, pos);

        if (Vector3.Distance(pos, endPosition) < 1f)
            lineReached = true;
    }

    void ShootHarpoon()
    {
        Debug.Log("Trying to shoot harpoon!");
        RaycastHit hit;
        if (Physics.Raycast(ropePoint.position, -ropePoint.right, out hit, maxHarpoonDistance, grappableLayers))
        {
            currentlyHarpoonedGameObject = hit.transform;
            Debug.Log("Harpoon hit " + hit.transform.name);
            grapplePoint = hit.point;
            joint = boat.gameObject.AddComponent<SpringJoint>();
            //joint.autoConfigureConnectedAnchor = false;
            //joint.connectedAnchor = currentlyHarpoonedGameObject.position;
            joint.connectedBody = hit.rigidbody;

            float distanceFromPoint = Vector3.Distance(boat.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            harpoonShot = true;

            joint.spring = 3f;
            joint.connectedMassScale = 20f;

            lineRenderer.positionCount = 2;
        }
    }

    void RetreatHarpoon()
    {
        lineRenderer.positionCount = 0;
        Destroy(joint);
        lineReached = false;
        hookedToBack = false;
        elapsedFrames = 0;
        currentlyHarpoonedGameObject = null;
        harpoonShot = false;
    }
}
