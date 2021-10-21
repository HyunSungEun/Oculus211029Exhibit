using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCoach : MonoBehaviour
{
    public GameObject rayCylinder;
    private void Start()
    {
        rayCylinder.GetComponent<MeshRenderer>().materials = new Material[] { rayCylinder.GetComponent<MeshRenderer>().materials[0] };
    }
    private void Update()
    {
        if (PipeDataPanelManager.Instance.gameObject.activeSelf) Destroy(gameObject);
    }
}
