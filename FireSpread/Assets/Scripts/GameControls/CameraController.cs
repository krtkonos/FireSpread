using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float zoomSpeed = 10f;
    public float maxZoom = 200f;
    public float minZoom = 20f;

    private GameObject _treeToInstantiate;
    private bool _treeMode = true;

    [SerializeField] private TreeGenerator _treeGen;
    [SerializeField] private Transform _treeParent;

    private float zoom = 10f;

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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Return to prevent spawning or destroying trees
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (_treeMode)
                {
                    SpawnOrDeleteTree(hit);
                }
                else 
                {
                    FireOrStopFire(hit);
                }
                
            }
        }
    }
    private void SpawnOrDeleteTree(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer == 7) // Check if the hit object is on the "Terrain" layer
        {
            _treeToInstantiate = _treeGen.GetTree();
            GameObject tree = Instantiate(_treeToInstantiate, hit.point, Quaternion.identity, _treeParent);
            _treeGen.TreesList.Add(tree);
        }
        else if (hit.collider.gameObject.layer == 6) // Check if the hit object is on the "Tree" layer
        {
            GameObject tree = hit.collider.gameObject;
            _treeGen.TreesList.Remove(tree);
            Destroy(tree);
        }
    }
    private void FireOrStopFire(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer == 6) // Check if the hit object is on the "Tree" layer
        {
            GameObject tree = hit.collider.gameObject;
            FireSpread fire = tree.GetComponent<FireSpread>();
            if (fire.isOnFire)
            {
                fire.Extinguish();
            }
            else
            {
                fire.SetFire();
            }
        }
    }
    public void ChangefireMode(bool isOn)
    {
        _treeMode = isOn;
    }
}
