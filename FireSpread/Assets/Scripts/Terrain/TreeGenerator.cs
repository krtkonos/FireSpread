using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    private GameObject[] _trees = new GameObject[3];
    private GameObject _trunk;
    public int _treeCount;
    public Terrain _terrain;
    private int _maxTrunkCount = 6000;
    [HideInInspector] public List<GameObject> TreesList = new List<GameObject>();
    [HideInInspector] public List<GameObject> TrunkList = new List<GameObject>();
    void Start()
    {
        GetRefferences();
        GenerateTrees();        
    }
    private void OnEnable()
    {
        FireSpread.TreeDestroyed += OnTreeDestroyed;
    }
    private void OnDisable()
    {
        FireSpread.TreeDestroyed -= OnTreeDestroyed;        
    }

    /// <summary>
    /// Generate [_treeCount] of trees over the terrain
    /// </summary>
    public void GenerateTrees()
    {
        for (int i = 0; i < _treeCount; i++)
        {
            float x = Random.Range(0f, 1f) * _terrain.terrainData.size.x;
            float z = Random.Range(0f, 1f) * _terrain.terrainData.size.z;
            float y = _terrain.SampleHeight(new Vector3(x, 0, z)) + _terrain.transform.position.y;
            Vector3 position = new Vector3(x, y, z);
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            GameObject tree = Instantiate(GetTree(), position, rotation, transform);
            TreesList.Add(tree);
        }
    }

    /// <summary>
    /// Get important references.
    /// Fills up _trees List and _trunk GO from Resources. 
    /// </summary>
    private void GetRefferences()
    {
        _terrain = GetComponent<Terrain>();
        _trees[0] = Resources.Load<GameObject>("Prefabs/Tree1");
        _trees[1] = Resources.Load<GameObject>("Prefabs/Tree2");
        _trees[2] = Resources.Load<GameObject>("Prefabs/Tree3");
        _trunk = Resources.Load<GameObject>("Prefabs/TreeTrunk1");
    }

    /// <summary>
    /// Set radom trees from TreesList on a fire.
    /// </summary>
    /// <param name="treesNum">Number of trees set on fire</param>
    public void SetRandomFire(int treesNum)
    {
        if (TreesList.Count <= 0)
        {
            return;
        }
        if (treesNum > 20)
        {
            treesNum = 20;
        }
        for (int i = 0; i < treesNum; i++)
        {
            TreesList[Random.Range(0, TreesList.Count)].GetComponent<FireSpread>().SetFire();
        }
    }

    /// <summary>
    /// Fnc called by FireSpread.TreeDestroyed Action. Deletes tree, removing it from the list, spawn a trunk, and checks if there is too much Decals.
    /// </summary>
    /// <param name="tree"></param>
    private void OnTreeDestroyed(GameObject tree)
    {
        Transform tr = tree.transform;
        GameObject trunk = Instantiate(_trunk, tr.position, tr.rotation, transform);

        TrunkList.Add(trunk);
        TreesList.Remove(tree);
        Destroy(tree);
        if (TrunkList.Count > _maxTrunkCount)
        {
            DeleteOffScreen();
        }
    }
    /// <summary>
    /// Get random tree from a _trees List
    /// </summary>
    /// <returns></returns>
    public GameObject GetTree()
    {
        return _trees[Random.Range(0, _trees.Length)];
    }

    /// <summary>
    /// Delete object which are off screen. Used to delete decals for the app to run smoothly
    /// </summary>
    private void DeleteOffScreen()
    {
        int count = 0;
        int numToDelete = TrunkList.Count - _maxTrunkCount;
        for (int i = 0; i < TrunkList.Count && count < numToDelete; i++)
        {
            if (!IsInView(TrunkList[i]))
            {
                Destroy(TrunkList[i]);
                TrunkList.RemoveAt(i);
                count++;
            }
        }
    }
    /// <summary>
    /// Checks if desired gameobject is in field of view of the camera
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>

    private bool IsInView(GameObject obj)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(obj.transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
