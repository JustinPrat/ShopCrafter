using System;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField]
    protected ManagerRefs managerRefs;

    public virtual void Toggle (bool isOn)
    {
        gameObject.SetActive(isOn);

        if (isOn)
        {
            managerRefs.InputManager.OnInputDeviceChanged += OnInputDeviceChanged;
        }
    }

    protected virtual void OnInputDeviceChanged()
    {

    }
}
