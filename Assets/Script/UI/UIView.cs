using System;
using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField]
    protected ManagerRefs managerRefs;

    protected bool activeState;
    public bool ActiveState => activeState;

    public virtual void Toggle (bool isOn)
    {
        activeState = isOn;
        SetVisualActivationView(isOn);

        if (isOn)
        {
            managerRefs.InputManager.OnInputDeviceChanged += OnInputDeviceChanged;
        }
        else
        {
            managerRefs.InputManager.OnInputDeviceChanged -= OnInputDeviceChanged;
        }
    }

    protected virtual void SetVisualActivationView(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    protected IEnumerator WaitForDuration(Action action, float duration)
    {
        yield return new WaitForSeconds(duration);
        action?.Invoke();
    }

    protected void Deactivate()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnInputDeviceChanged()
    {

    }
}
