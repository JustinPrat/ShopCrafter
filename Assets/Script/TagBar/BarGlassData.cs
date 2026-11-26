using UnityEngine;

[CreateAssetMenu(fileName = "BarGlassData", menuName = "ShopCrafter/BarGlassData")]
public class BarGlassData : BarData
{
    public float BarSize = 0.3f;
    public float OnTargetSpeedMultiplier;
    public float HitDuration = 1;
    public GameObject HitSucessFeedbackPrefab;

    public override BarBehaviour GetBehaviour()
    {
        BarGlassBehavior stats = new BarGlassBehavior(this);
        return stats;
    }

    public class BarGlassBehavior : BarBehaviour
    {
        private BarGlassData barElementData;
	    private float hitDuration;
        private bool isPressing;
        public BarGlassBehavior(BarGlassData data) : base(data)
        {
            barElementData = data;
        }

        public override void OnStart(MiniGameView miniGameView)
        {
            base.OnStart(miniGameView);

            miniGameView.ManagerRefs.InputManager.Actions.UI.Submit.started += SubmitStarted;
            miniGameView.ManagerRefs.InputManager.Actions.UI.Submit.canceled += SubmitCanceled;
            hitDuration = 0;
            SetupTarget(0.5f, barElementData.BarSize);
            miniGameView.SetBarNumber(((int)(Mathf.Clamp01(hitDuration / barElementData.HitDuration) * 100)).ToString() + "/100%");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsOnTarget(barPos))
            {
                currentSpeed = baseSpeed * barElementData.OnTargetSpeedMultiplier;
            }
            else if (currentSpeed != baseSpeed)
            {
                currentSpeed = baseSpeed;
            }

            if (isPressing && IsOnTarget(barPos))
            {
                hitDuration += Time.unscaledDeltaTime;
                miniGameView.SetBarNumber(((int)(Mathf.Clamp01(hitDuration / barElementData.HitDuration) * 100)).ToString() + "/100%");
            }
        }

        protected override void OnChangeDirection()
        {
            base.OnChangeDirection();
            sucessRatio = Mathf.Clamp01(hitDuration/ barElementData.HitDuration);
            Stop();
        }

        public override void Stop()
        {
            miniGameView.ManagerRefs.InputManager.Actions.UI.Submit.performed -= SubmitStarted;
            base.Stop();
        }

        private void SubmitStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            isPressing = true;
        }

        private void SubmitCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            isPressing = false;
        }
    }
}
