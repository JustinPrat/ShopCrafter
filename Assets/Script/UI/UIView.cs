using UnityEngine;

public class UIView : MonoBehaviour
{
    public virtual void Toggle (bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}
