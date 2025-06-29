using DG.Tweening;
using UnityEngine;

public class BoxElement : MonoBehaviour, IInteractable
{
    [SerializeField] private float squishScale;

    public void DoInteract()
    {
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one * squishScale, 0.3f);
    }
}
