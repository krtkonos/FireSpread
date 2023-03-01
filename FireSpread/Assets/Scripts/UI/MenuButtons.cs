using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private Button _generateButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _simulateButton;
    [SerializeField] private Button _modeButton;
    [SerializeField] private Button _fireButton;

    [SerializeField] private TreeGenerator _TerrainGenerator;
    [SerializeField] private int _randomFiredTreesNum;

    // Start is called before the first frame update
    void Start()
    {
        GetRefferences();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void GetRefferences()
    {
        _generateButton.onClick.AddListener(GenerateTerrain);
        _clearButton.onClick.AddListener(ClearTerrain);
        _fireButton.onClick.AddListener(() => SetRandomFire(_randomFiredTreesNum));
    }
    private void GenerateTerrain()
    {
        ClearTerrain();
        _TerrainGenerator.GenerateTrees();
    }
    private void ClearTerrain()
    {
        List<GameObject> trees = _TerrainGenerator.TreesList;
        if (trees.Count > 0)
        {
            for (int i = 0; i < trees.Count; i++)
            {
                Destroy(trees[i]);
            }
            trees.Clear();
        }
    }
    private void SetRandomFire(int _randomFiredTreesNum)
    {
        _TerrainGenerator.SetRandomFire(_randomFiredTreesNum);        
    }
}
