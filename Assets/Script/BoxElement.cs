using DG.Tweening;
using UnityEngine;

public class BoxElement : MonoBehaviour, IInteractable
{
    [SerializeField] private float squishScale;
    [SerializeField] private Sprite icon;

    public Sprite InteractIcon => icon;

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
