public abstract class WorldViewInteract<TView> : BaseWorldInteract where TView : UIView
{
    public override void StartInteract(PlayerBrain playerBrain)
    {
        isInteracting = true;
        managerRefs.UIManager.OnViewClosed -= OnViewClosed;
        managerRefs.UIManager.OnViewClosed += OnViewClosed;

        base.StartInteract(playerBrain);
    }

    protected virtual void OnViewClosed(UIView view)
    {
        if (view is TView)
        {
            managerRefs.UIManager.OnViewClosed -= OnViewClosed;
            isInteracting = false;
        }
    }

    protected override void OnDestroy()
    {
        managerRefs.UIManager.OnViewClosed -= OnViewClosed;
        base.OnDestroy();
    }
}
