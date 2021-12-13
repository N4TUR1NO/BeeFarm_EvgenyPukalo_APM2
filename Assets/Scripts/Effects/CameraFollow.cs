using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private Transform target;
    [SerializeField] [Range(0.01f, 1f)] private float smoothSpeed;
    [SerializeField] private Vector3 offset;
    
    #endregion
    
    #region LifeCycle
    
    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    #endregion
}
