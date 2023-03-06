using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class FireSpread : MonoBehaviour
{
    public static event System.Action<GameObject> TreeDestroyed;
    public bool isOnFire = false;
    private Color _originColor = new Color(0f, 1f, 0f, 1.0f);
    public static float _treeSpacing = 5;
    public static float _windDIrection = 10;
    private float _timeToCatchFire = 2f;
    private float _timeToBurnOut = 5f;
    private float _timeToFinishFire;
    public void SetFire()
    {
        _timeToFinishFire = _timeToBurnOut - _timeToCatchFire;
        isOnFire = true;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = Color.red;
        StartCoroutine(TreeBurnedCO());
    }
    private IEnumerator TreeBurnedCO()
    {
        yield return new WaitForSeconds(_timeToCatchFire);

        Wait(_treeSpacing);

        yield return new WaitForSeconds(_timeToFinishFire);
        if (isOnFire)
        {
            TreeDestroyed?.Invoke(gameObject);
        };

    }

    public void Extinguish()
    {
        isOnFire = false;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = _originColor;
    }
    private void Wait(float treeSpacing)
    {
        Vector3 centerPoint = GetCirclePoint(transform.position, _treeSpacing - 5, _windDIrection);
        Collider[] nearbyTrees = Physics.OverlapSphere(centerPoint, treeSpacing);
        //BrushBurnt(centerPoint, _treeSpacing);

        if (isOnFire && nearbyTrees.Length > 0)
        {

            for (int i = 0; i < nearbyTrees.Length; i++)
            {
                GameObject tree = nearbyTrees[i].gameObject;
                if (CanSetOnFire(tree))
                {
                    tree.GetComponent<FireSpread>().SetFire();
                }
            }
        }
    }

    private bool CanSetOnFire(GameObject tree)
    {
        return tree != gameObject && tree.layer == LayerMask.NameToLayer("Tree") && !tree.GetComponent<FireSpread>().isOnFire;
    }
    private Vector3 GetCirclePoint(Vector3 center, float radius, float degreeOffset)
    {
        float angle = degreeOffset * Mathf.Deg2Rad;
        Vector3 point = new Vector3(center.x + radius * Mathf.Sin(angle), center.y, center.z + radius * Mathf.Cos(angle));
        return point;
    }
    private void BrushBurnt(Vector3 treePosition, float radius)
    {
        StartCoroutine(BrushBurntCo(treePosition, radius));
    }

    private IEnumerator BrushBurntCo(Vector3 treePosition, float radius)
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        float[,,] alphamapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        int posXInTerrain = (int)((treePosition.x / terrainData.size.x) * terrainData.alphamapWidth);
        int posZInTerrain = (int)((treePosition.z / terrainData.size.z) * terrainData.alphamapHeight);

        int radiusInPixels = (int)((radius / terrainData.size.x) * terrainData.alphamapWidth);

        int startX = Mathf.Max(0, posXInTerrain - radiusInPixels);
        int startZ = Mathf.Max(0, posZInTerrain - radiusInPixels);
        int endX = Mathf.Min(terrainData.alphamapWidth - 1, posXInTerrain + radiusInPixels);
        int endZ = Mathf.Min(terrainData.alphamapHeight - 1, posZInTerrain + radiusInPixels);

        for (int z = startZ; z <= endZ; z++)
        {
            for (int x = startX; x <= endX; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(posXInTerrain, posZInTerrain));
                if (distance <= radiusInPixels)
                {
                    alphamapData[z, x, 0] = 0f;
                    alphamapData[z, x, 1] = 1f;
                }
            }

            // Yield every 100 rows to avoid blocking the main thread for too long
            if (z % 100 == 0)
            {
                yield return null;
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamapData);
    }

    /*private IEnumerator BrushBurntCo(Vector3 treePosition, float radius)
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        float[,,] alphamapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        int posXInTerrain = (int)((treePosition.x / terrainData.size.x) * terrainData.alphamapWidth);
        int posZInTerrain = (int)((treePosition.z / terrainData.size.z) * terrainData.alphamapHeight);

        int radiusInPixels = (int)((radius / terrainData.size.x) * terrainData.alphamapWidth);

        int startX = Mathf.Max(0, posXInTerrain - radiusInPixels);
        int startZ = Mathf.Max(0, posZInTerrain - radiusInPixels);
        int endX = Mathf.Min(terrainData.alphamapWidth - 1, posXInTerrain + radiusInPixels);
        int endZ = Mathf.Min(terrainData.alphamapHeight - 1, posZInTerrain + radiusInPixels);

        for (int z = startZ; z <= endZ; z++)
        {
            for (int x = startX; x <= endX; x++)
            {
                float xDist = Mathf.Abs(x - posXInTerrain);
                float zDist = Mathf.Abs(z - posZInTerrain);
                if (xDist <= radiusInPixels && zDist <= radiusInPixels)
                {
                    alphamapData[z, x, 0] = 0f;
                    alphamapData[z, x, 1] = 1f;
                }
            }

            // Yield every 100 rows to avoid blocking the main thread for too long
            if (z % 100 == 0)
            {
                yield return null;
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamapData);
    }*/
}
