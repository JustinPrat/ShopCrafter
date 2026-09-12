using UnityEngine;

public class UIBillboarding : MonoBehaviour
{
    [SerializeField]
    private bool onlyOnEnable;
    private Camera mainCamera;

    private void OnEnable()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (onlyOnEnable)
            SetRotation();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (onlyOnEnable)
            return;

        SetRotation();
    }

    private void SetRotation()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}
