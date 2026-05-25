using UnityEngine;

public class ViewPointBillboard : MonoBehaviour
{
    private Camera mainCam;
    public float spriteTiltAngleX = 10f;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        Vector3 forwardDir = mainCam.transform.forward;
        transform.forward = forwardDir;
        transform.Rotate(spriteTiltAngleX, 0f, 0f, Space.Self);
    }
}