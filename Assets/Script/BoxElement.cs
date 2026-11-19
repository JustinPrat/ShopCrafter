using PrimeTween;
using UnityEngine;

public class BoxElement : BaseWorldInteract
{
    [SerializeField] 
    private float squishScale;

    [SerializeField]
    private float squishDuration = 0.3f;

    public override void StartInteract(PlayerBrain playerBrain)
    {
        transform.localScale = Vector3.one;
        Tween.PunchScale(transform, Vector3.one * squishScale, squishDuration);
        base.StartInteract(playerBrain);
    }
}
