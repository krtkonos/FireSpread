using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private Button _generateButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _simulateButton;
    [SerializeField] private Button _fireButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Toggle _modeButton;
    [SerializeField] private Toggle _SimulationButton;
    [SerializeField] private Slider _windStrenght;
    [SerializeField] private Slider _windDirection;
    [SerializeField] private TextMeshProUGUI _modeButtonText;
    [SerializeField] private TextMeshProUGUI _SimulateButtonText;

    [SerializeField] private TreeGenerator _TerrainGenerator;
    [SerializeField] private CameraController _camController;
    [SerializeField] private int _randomFiredTreesNum;
    //[SerializeField] private GameObject _windArrow;
    [SerializeField] private RectTransform _windArrow;

    public static event System.Action<GameObject> Fire_TreeModeChanged;

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
        _exitButton.onClick.AddListener(ExitApp);
        _fireButton.onClick.AddListener(() => SetRandomFire(_randomFiredTreesNum));

        _modeButton.onValueChanged.AddListener(OnModeButtonValueChanged);
        _SimulationButton.onValueChanged.AddListener(OnSimulateButtonValueChanged);

        _windStrenght.onValueChanged.AddListener(OnWindStrengthChanged);
        _windDirection.onValueChanged.AddListener(OnWindDirectionChanged);

        _windArrow.rotation = Quaternion.Euler(0, 0, 540);
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
        List<GameObject> trunk = _TerrainGenerator.TrunkList;
        if (trunk.Count > 0)
        {
            for (int i = 0; i < trunk.Count; i++)
            {
                Destroy(trunk[i]);
            }
            trunk.Clear();
        }
        ChangeTerrainToGrass();
    }
    private void SetRandomFire(int _randomFiredTreesNum)
    {
        _TerrainGenerator.SetRandomFire(_randomFiredTreesNum);        
    }
    private void OnModeButtonValueChanged(bool isOn)
    {
        _camController.ChangefireMode(isOn);
        _modeButtonText.text = isOn ? "Tree Mode" : "Fire Mode";
    }
    private void OnWindStrengthChanged(float value)
    {
        FireSpread._treeSpacing = Mathf.Lerp(10, 50, value);
    }
    private void OnWindDirectionChanged(float value)
    {
        float trueDirection = Mathf.Lerp(0, 359, value);
        float directionImage = Mathf.Lerp(540, 181, value);
        FireSpread._windDIrection = trueDirection;
        _windArrow.rotation = Quaternion.Euler(0, 0, directionImage);
    }
    private void ExitApp()
    {
        Application.Quit();
    }
    private void OnSimulateButtonValueChanged(bool isOn)
    {
        _SimulateButtonText.text = isOn ? "Pause" : "Play";
        Time.timeScale = isOn ? 1 : 0;
    }
    private void ChangeTerrainToGrass()
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        float[,,] alphamapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int i = 0; i < terrainData.alphamapHeight; i++)
        {
            for (int j = 0; j < terrainData.alphamapWidth; j++)
            {
                alphamapData[j, i, 0] = 1f;
                alphamapData[j, i, 1] = 0f;
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamapData);
    }

}
