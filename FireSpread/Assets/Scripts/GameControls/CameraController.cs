using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 10f;
    public float maxZoom = 200f;
    public float minZoom = 20f;
    private GameObject _treeToInstantiate;
    private LayerMask layerMask;
    private const string _terrainLayerName = "Terrain";
    [SerializeField] private TreeGenerator _treeGen;

    private float zoom = 10f;
    private void Start()
    {
        //layerMask = LayerMask.NameToLayer(_terrainLayerName);
    }

    private void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        transform.position += direction * moveSpeed * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            zoom -= scroll * zoomSpeed;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            transform.position += transform.forward * scroll * zoomSpeed;
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minZoom, maxZoom), transform.position.z);
        }


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log("hit");
                _treeToInstantiate = _treeGen.GetTree();
                GameObject tree = Instantiate(_treeToInstantiate, hit.point, Quaternion.identity);
                _treeGen.TreesList.Add(tree);
            }
        }
    }
}
