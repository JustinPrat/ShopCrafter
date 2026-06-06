using UnityEngine;

[CreateAssetMenu(fileName = "ControllerData", menuName = "ShopCrafter/ControllerData", order = 1)]
public class ControllerData : ScriptableObject
{
    public float WalkSpeed = 1f;
    public float InteractionRange = 1f;
    public LayerMask InteractionLayer;
    public LayerMask GroundLayer;
    public bool ToggleDebug;
}
