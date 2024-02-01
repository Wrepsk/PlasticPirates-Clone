using UnityEngine;

public class BuoyantObject : MonoBehaviour
{
    public TrashManager manager;

    public Vector3 position;
    public Quaternion rotation;
    
    public float xPhase;
    public float zPhase;

    private float initialYoffset = 0;
    
    private void Start() {
        manager = TrashManager.instance;

        initialYoffset = gameObject.transform.GetChild(0).position.y;
    }

    private void Update()
    {
        Transform meshTransform = gameObject.transform.GetChild(0);


        meshTransform.localPosition = GetBuoyancy();
        meshTransform.localRotation = GetBuoyancyRotation();

        UpdateBuoyancy();
    }

    public void UpdateBuoyancy()
    {
        xPhase += Time.deltaTime * manager.xFrequency;
        if (xPhase > 32*Mathf.PI)
            xPhase = xPhase-32*Mathf.PI;
        
        zPhase += Time.deltaTime * manager.zFrequency;
        if (zPhase > 32*Mathf.PI)
            zPhase = zPhase-32*Mathf.PI;
    }

    public Vector3 GetBuoyancy()
    {
        return new Vector3(
            0.0f,
            manager.yRange * Mathf.Sin((xPhase + zPhase) / 2) + initialYoffset,
            0.0f
        );
    }

    public Quaternion GetBuoyancyRotation()
    {
        return Quaternion.Euler(
            Mathf.Sin(xPhase) * manager.xDegreeRange,
            0.0f,
            Mathf.Sin(manager.zPhaseOffset+zPhase) * manager.zDegreeRange
        );
    }
}