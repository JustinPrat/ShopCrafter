using UnityEngine;

[CreateAssetMenu(fileName = "ControllerData", menuName = "ShopCrafter/ControllerData", order = 1)]
public class ControllerData : ScriptableObject
{
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float interactionRange = 1f;
    [SerializeField] private LayerMask interactionLayer;

    public float WalkSpeed => walkSpeed;
    public float InteractionRange => interactionRange;
    public LayerMask InteractionLayer => interactionLayer;
}
