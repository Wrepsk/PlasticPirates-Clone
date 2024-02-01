using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class Trash 
{
    public TrashManager manager;
    public int meshType;
    public LinkedListNode<Trash> node;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject gameObject;
    public float xPhase;
    public float zPhase;

    public void Materialize()
    {
        Assert.IsNull(gameObject);
        Transform trashParent = manager.transform.Find("Trash");
        gameObject = GameObject.Instantiate(manager.prefab, position, rotation, trashParent);
        gameObject.GetComponent<TrashBehaviour>().trash = this;
        GameObject mesh = GameObject.Instantiate(manager.meshPrefabs[meshType], gameObject.transform);
    }

    public void Dematerialize()
    {
        Assert.IsNotNull(gameObject);

        position = gameObject.transform.position;
        rotation = gameObject.transform.rotation;

        GameObject.Destroy(gameObject);
        gameObject = null;
    }

    public bool IsMaterialized()
    {
        return gameObject != null;
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
            manager.yRange * Mathf.Sin((xPhase + zPhase) / 2),
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

public class TrashManager : MonoBehaviour
{
    public static TrashManager instance;

    [Header("General")]
    public int totalTrash = 1000;
    public Sampler2d sampler;
    public GameObject prefab;
    public GameObject[] meshPrefabs;
    
    [Header("LOD")]
    public float lodRange = 100.0f;
    public float lodSlack = 10.0f;

    [Header("Buoyancy")]
    public float buoyancyRandomness = 1.0f;
    public float buoyancySpatialScaling = 0.25f;
    public float xFrequency = 2.8f;
    public float zFrequency = 3.3f;
    public float xDegreeRange = 5.0f;
    public float zDegreeRange = 2.0f;
    public float zPhaseOffset = 1.5f;
    public float yRange = 0.25f;

    private Mesh[] meshes;
    private Material[] materials;

    private LinkedList<Trash> trashList = new LinkedList<Trash>();
    private Matrix4x4[] instData;

    public Trash CreateTrash(Vector3 position, int meshType = -1)
    {
        Trash trash = new Trash();
        trash.manager = this;
        trash.position = position;
        trash.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
        trash.xPhase = trash.position.x * buoyancySpatialScaling + Random.Range(0.0f, buoyancyRandomness);
        trash.zPhase = trash.position.z * buoyancySpatialScaling + Random.Range(0.0f, buoyancyRandomness);

        if (meshType == -1)
            trash.meshType = Random.Range(0, meshPrefabs.Length);
        else
            trash.meshType = meshType;

        AddTrash(trash);
        return trash;
    }

    public void CollectTrash(Trash trash)
    {
        StatsManager.instance.CollectedTrash += 1;
        RemoveTrash(trash);
    }

    private void AddTrash(Trash trash)
    {
        if (trashList.Count >= totalTrash)
            throw new System.Exception("Can't add more trash. Maximum reached.");

        trashList.AddLast(trash);
        trash.node = trashList.Last;
    }

    private void RemoveTrash(Trash trash)
    {
        trashList.Remove(trash.node);
        if (trash.gameObject != null)
            trash.Dematerialize();
    }

    private void Render()
    {
        for (int meshType = 0; meshType < meshPrefabs.Length; meshType++)
        {
            int i = 0;
            foreach(Trash trash in trashList)
            {
                if (trash.IsMaterialized() || trash.meshType != meshType)
                    continue;
                Matrix4x4 worldTransform = Matrix4x4.TRS(trash.position, trash.rotation, Vector3.one);
                Matrix4x4 buoyancyTransform = Matrix4x4.TRS(trash.GetBuoyancy(), trash.GetBuoyancyRotation(), Vector3.one);
                instData[i] = worldTransform * buoyancyTransform;
                i++;
            }
            if (i > 0)
            {
                RenderParams rp = new RenderParams(materials[meshType]);
                rp.shadowCastingMode = ShadowCastingMode.Off;
                rp.receiveShadows = false;
                Graphics.RenderMeshInstanced(rp, meshes[meshType], 0, instData, i);
            }
        }
    }

    private void UpdateTrash()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 position = player.transform.position;
        foreach(Trash trash in trashList)
        {
            trash.UpdateBuoyancy();

            float distance = Vector3.Distance(trash.position, position);
            if (distance < lodRange && !trash.IsMaterialized())
                trash.Materialize();
            if (distance > lodRange + lodSlack && trash.IsMaterialized())
                trash.Dematerialize();
        }
    }

    private void PrepopulateMeshes()
    {
        meshes = new Mesh[meshPrefabs.Length];
        materials = new Material[meshPrefabs.Length];
        for(int i = 0; i < meshPrefabs.Length; i++)
        {
            meshes[i] = meshPrefabs[i].GetComponent<MeshFilter>().sharedMesh;
            materials[i] = new Material(meshPrefabs[i].GetComponent<Renderer>().sharedMaterial);  // copy so that we modify safely
            materials[i].enableInstancing = true;
        }
    }

    private void SpawnTrash()
    {
        for(int i = 0; i < totalTrash; i++)
        {
            int type = Random.Range(0, meshPrefabs.Length);
            float yOffset = meshes[type].bounds.size.y / 2;
            Vector2 position2d = sampler.SampleVector2();
            Vector3 position3d = new Vector3(position2d.x, -yOffset, position2d.y);
            CreateTrash(position3d, type);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTrash();
        Render();
    }

    void Awake() {
        instance = this;
        instData = new Matrix4x4[totalTrash];
    }

    void Start()
    {
        PrepopulateMeshes();
        SpawnTrash();
    }

}
