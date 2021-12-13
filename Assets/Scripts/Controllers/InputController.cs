using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    #region Fields

    [SerializeField] private FloatingJoystick joystick;

    public static Action<float, float> OnDrag;
    public static Action OnRelease;

    #endregion

    #region LifeCycle

    private void FixedUpdate()
    {
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            OnDrag?.Invoke(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            OnRelease?.Invoke();
        }
    }
    
    #endregion
}
