using UnityEngine;

[System.Serializable]
public class PerlinParameters
{
    public Vector2 offset = Vector2.zero;
    public Vector2 scale = Vector2.one * 10.0f;
    public float weight = 1.0f;
    public bool multiply = false; 
}


public class PerlinSampler2d : Sampler2d
{
    public Bounds bounds = new Bounds(Vector3.zero, new Vector3(100, 0, 100)); 
    public float resolution = 0.25f;
    public PerlinParameters[] perlinParameters;
    public bool renderTexture = false;

    public Texture2D texture;
    private float[,] densityMap;
    private float maxDensity;

    public void Start()
    {
        densityMap = new float[
            Mathf.CeilToInt(bounds.size.x / resolution),
            Mathf.CeilToInt(bounds.size.z / resolution)
        ];    
        GenerateDensityMap();
    }

    private void GenerateDensityMap()
    {
        float sum = 0.0f;
        for (int i = 0; i < densityMap.GetLength(0); i++) 
        {
            for (int j = 0; j < densityMap.GetLength(1); j++) 
            {
                densityMap[i, j] = 0.0f;
                for (int k = 0; k < perlinParameters.Length; k++) 
                {
                    PerlinParameters parameters = perlinParameters[k];
                    float value = parameters.weight * Mathf.PerlinNoise(
                        i*resolution*(1/parameters.scale.x) + parameters.offset.x,
                        j*resolution*(1/parameters.scale.y) + parameters.offset.y
                    );
                    if (parameters.multiply)
                        densityMap[i, j] *= value;
                    else
                        densityMap[i, j] += value;
                }
                sum += densityMap[i, j]; 
            }
        }

        // Normalize density map
        for (int i = 0; i < densityMap.GetLength(0); i++) {
            for (int j = 0; j < densityMap.GetLength(1); j++) {
                densityMap[i, j] /= sum;
                if (densityMap[i, j] > maxDensity)
                    maxDensity = densityMap[i, j];
            }
        }

        // Visualize for debugging purposes
        if (renderTexture) {
            texture = new Texture2D(densityMap.GetLength(0), densityMap.GetLength(1));
            Color[] colors = new Color[densityMap.GetLength(0) * densityMap.GetLength(1)];
            for (int i = 0; i < densityMap.GetLength(0); i++) {
                for (int j = 0; j < densityMap.GetLength(1); j++) {
                    float value = densityMap[i, j] / maxDensity;
                    colors[i * densityMap.GetLength(1) + j] = new Color(value, value, value);
                }
            } 
            texture.SetPixels(colors);
            texture.Apply();
        }
    }

    private (int,int) GetIndices(Vector2 position)
    {
        return (
            Mathf.FloorToInt((position.x - bounds.min.x) / resolution),
            Mathf.FloorToInt((position.y - bounds.min.z) / resolution)
        );
    }

    private Vector2 GetPosition(int i, int j)
    {
        return new Vector2(
            i * resolution + bounds.min.x,
            j * resolution + bounds.min.z
        );
    }

    private float GetDensity(Vector2 position)
    {
        (int i, int j) = GetIndices(position);
        return densityMap[i, j];
    }

    override public Vector2 SampleVector2()
    {
        float uniformDensity = 1/(bounds.size.x * bounds.size.z);
        float M = maxDensity / uniformDensity;

        Vector2 candidate = Vector2.zero;
        for (int i = 0; i < 50; i++) {
            candidate = new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
            float density = GetDensity(candidate);
            float u = UnityEngine.Random.Range(0.0f, 1.0f);
            if (u < density / (M*uniformDensity)) {
                return candidate;
            }
        }
        print("Failed to sample position");
        return candidate;
    }
}
