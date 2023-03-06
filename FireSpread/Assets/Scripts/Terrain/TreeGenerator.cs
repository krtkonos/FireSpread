using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    private GameObject[] _trees = new GameObject[3];
    private GameObject _trunk;
    public int _treeCount;
    public Terrain _terrain;
    private int _maxTrunkCount = 8000;
    private int _countToDelete = 10;
    [HideInInspector] public List<GameObject> TreesList = new List<GameObject>();
    [HideInInspector] public List<GameObject> TrunkList = new List<GameObject>();
    public static TreeGenerator Instance { get; set; }
    private void Awake()
    {
        Instance = this;
    }
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
    private void GetRefferences()
    {
        _terrain = GetComponent<Terrain>();
        _trees[0] = Resources.Load<GameObject>("Prefabs/Tree1");
        _trees[1] = Resources.Load<GameObject>("Prefabs/Tree2");
        _trees[2] = Resources.Load<GameObject>("Prefabs/Tree3");
        _trunk = Resources.Load<GameObject>("Prefabs/TreeTrunk1");
    }

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
    private void OnTreeDestroyed(GameObject tree)
    {
        Transform tr = tree.transform;
        GameObject trunk = Instantiate(_trunk, tr.position, tr.rotation, transform);

        TrunkList.Add(trunk);
        TreesList.Remove(tree);
        Destroy(tree);
        if (TrunkList.Count > _maxTrunkCount)
        {
            DeleteOver();
        }
    }
    public GameObject GetTree()
    {
        return _trees[Random.Range(0, _trees.Length)];
    }
    
    private void DeleteOver()
    {
        for (int i = 0; i < TrunkList.Count - _maxTrunkCount; i++)
        {
            Destroy(TrunkList[i]);
            TrunkList.Remove(TrunkList[i]);
        }
    }
}
