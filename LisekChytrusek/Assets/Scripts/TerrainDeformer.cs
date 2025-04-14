using System.Collections;
using UnityEngine;

public class TerrainDeformer : MonoBehaviour
{
    public Transform playerTransform;
    public float deformationRadius = 2f;    // promień platformy
    public float deformationDepth = 0.05f;   // całkowita wysokość podniesienia
    public int steps = 100;                  // liczba kroków deformacji
    public float stepDelay = 0.01f;          // czas między krokami (w sekundach)

    private Terrain terrain;
    private TerrainData terrainData;
    private float[,] originalHeights;

    void Start()
    {
        terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            return;
        }

        terrainData = terrain.terrainData;

        // Zapamiętaj oryginalne wysokości całego terenu
        int res = terrainData.heightmapResolution;
        originalHeights = terrainData.GetHeights(0, 0, res, res);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(DeformTerrainGradually());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTerrain();
        }
    }

    void OnApplicationQuit()
    {
        ResetTerrain();
    }

    void ResetTerrain()
    {
        int res = terrainData.heightmapResolution;
        terrainData.SetHeights(0, 0, originalHeights);
    }

    public IEnumerator DeformTerrainGradually()
    {
        Vector3 terrainPos = playerTransform.position - terrain.transform.position;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        float relativeX = terrainPos.x / terrainData.size.x;
        float relativeZ = terrainPos.z / terrainData.size.z;

        int centerX = Mathf.RoundToInt(relativeX * (heightmapWidth - 1));
        int centerZ = Mathf.RoundToInt(relativeZ * (heightmapHeight - 1));

        int localRadius = 1;
        int localSize = localRadius * 2;

        int sampleStartX = Mathf.Clamp(centerX - localRadius, 0, heightmapWidth - 1);
        int sampleStartZ = Mathf.Clamp(centerZ - localRadius, 0, heightmapHeight - 1);

        float[,] centralPatch = terrainData.GetHeights(sampleStartX, sampleStartZ, localSize, localSize);
        float baseHeight = GetHeight(centralPatch);

        float currentDepth = deformationDepth;
        float currentRadius = deformationRadius;


        {
            int radius = Mathf.RoundToInt(deformationRadius);
            int size = radius * 2;
            int startX = Mathf.Clamp(centerX - radius, 0, heightmapWidth - size);
            int startZ = Mathf.Clamp(centerZ - radius, 0, heightmapHeight - size);

            float[,] heights = terrainData.GetHeights(startX, startZ, size, size);

            int flattenSteps = Mathf.Min(10, steps);

            for (int step = 0; step < flattenSteps; step++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        float current = heights[z, x];
                        float difference = baseHeight - current;
                        heights[z, x] += difference / (flattenSteps - step);
                    }
                }

                terrainData.SetHeights(startX, startZ, heights);
                yield return new WaitForSeconds(stepDelay);
            }
        }

        for (int layer = 0; layer < 2; layer++)
        {
            int radius = Mathf.RoundToInt(currentRadius);
            int size = radius * 2;
            int startX = Mathf.Clamp(centerX - radius, 0, heightmapWidth - size);
            int startZ = Mathf.Clamp(centerZ - radius, 0, heightmapHeight - size);

            float[,] heights = terrainData.GetHeights(startX, startZ, size, size);
            float layerBaseHeight = GetHeight(heights);
            float targetHeight = layerBaseHeight + currentDepth;

            for (int step = 0; step < steps; step++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        float current = heights[z, x];
                        float liftStep = currentDepth / steps;

                        if (current < targetHeight)
                        {
                            heights[z, x] = Mathf.Min(current + liftStep, targetHeight);
                        }
                    }
                }

                terrainData.SetHeights(startX, startZ, heights);
                yield return new WaitForSeconds(stepDelay);
            }

            currentDepth /= 2f;
            currentRadius /= 2f;
        }
    }

    float GetHeight(float[,] heights)
    {
        float sum = 0f;
        int count = heights.GetLength(0) * heights.GetLength(1);

        foreach (float h in heights)
            {
                sum += h;
            }
            
        return sum / count;
    }

}
