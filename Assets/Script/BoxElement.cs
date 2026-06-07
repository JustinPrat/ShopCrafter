using DG.Tweening;
using UnityEngine;

public class BoxElement : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private float squishScale;

    [SerializeField]
    private string interactText;

    [SerializeField] 
    private Collider collider;

    public bool IsLocked { get; set; }
    public Collider PhysicCollider => collider;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one * squishScale, 0.3f);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }
}
