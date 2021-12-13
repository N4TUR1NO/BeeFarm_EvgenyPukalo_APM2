using System;
using UnityEngine;

public class HealthCanvasBillboard : MonoBehaviour
{
    private Camera _camera;

    #region LifeCycle

    private void Awake()
    {
        _camera = Camera.main;
        GetComponent<Canvas>().worldCamera = _camera;
    }

    private void LateUpdate()
    {
        transform.LookAt(_camera.transform.position);
    }
    
    #endregion
}
