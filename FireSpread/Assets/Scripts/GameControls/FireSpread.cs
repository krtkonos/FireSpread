using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireSpread : MonoBehaviour
{
    public static event System.Action<GameObject> TreeDestroyed;
    public static UnityEvent<GameObject> TreeCreated;
    public void SetFire()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = Color.red;
        StartCoroutine(TreeBurnedCO());
    }
    private IEnumerator TreeBurnedCO() 
    {
        yield return new WaitForSeconds(5f);
        TreeDestroyed?.Invoke(gameObject);
        Destroy(gameObject);
    }
}
