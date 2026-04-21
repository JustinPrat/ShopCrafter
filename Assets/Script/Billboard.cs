using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 originalScale;

    void Start()
    {
        mainCam = Camera.main;
        originalScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        Vector3 direction = mainCam.transform.forward;
        direction.y = 0;
        transform.forward = direction;

        float camPitch = mainCam.transform.eulerAngles.x * Mathf.Deg2Rad;
        float scaleCompensation = 1f / Mathf.Clamp(Mathf.Cos(camPitch), 0.1f, 1f);

        transform.localScale = new Vector3(originalScale.x, originalScale.y * scaleCompensation, originalScale.z);
    }
}