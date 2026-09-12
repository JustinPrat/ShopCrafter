using UnityEngine;

[ExecuteAlways]
public class CameraPlayerDitherSync : MonoBehaviour
{
    public Transform player;
    public Vector3 playerOffset = new Vector3(0, 1f, 0);
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (player == null) return;
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) return;

        Vector3 targetPos = player.position + playerOffset;

        // Position écran en UV [0, 1]
        Vector3 screenPoint = cam.WorldToViewportPoint(targetPos);
        Shader.SetGlobalVector("_PlayerScreenPos", new Vector4(screenPoint.x, screenPoint.y, 1.0f, 0.0f));

        // Profondeur caméra linéaire exacte (axe Z local de la caméra)
        float playerDepth = cam.transform.InverseTransformPoint(targetPos).z;
        Shader.SetGlobalFloat("_PlayerCameraDepth", playerDepth);

        Shader.SetGlobalVector("_PlayerWorldPos", targetPos);
    }
}