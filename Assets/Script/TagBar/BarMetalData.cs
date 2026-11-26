using UnityEngine;

[CreateAssetMenu(fileName = "BarMetalData", menuName = "ShopCrafter/BarMetalData")]
public class BarMetalData : BarData
{
    public float BarSize = 0.5f;
    public float OnTargetSpeedMultiplier;
    public int NumberHit = 3;
    public GameObject HitSucessFeedbackPrefab;

    public override BarBehaviour GetBehaviour()
    {
        BarMetalBehavior stats = new BarMetalBehavior(this);
        return stats;
    }

    public class BarMetalBehavior : BarBehaviour
    {
        private BarMetalData barElementData;
	    private int hitNumber;
        public BarMetalBehavior(BarMetalData data) : base(data)
        {
            barElementData = data;
        }

        private void SubmitPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (IsOnTarget(barPos))
            {
                ValidateMove();
            }
        }

        private void ValidateMove()
        {
            hitNumber++;
            miniGameView.SetBarNumber(Mathf.Clamp(hitNumber, 0, barElementData.NumberHit).ToString() + "/" + barElementData.NumberHit);

            if (barElementData.HitSucessFeedbackPrefab != null && TryGetHoveredTarget(out Target hoveredTarget))
            {
                miniGameView.InstantiateParticles(barElementData.HitSucessFeedbackPrefab, hoveredTarget.TargetRect.position);
            }
        }

        public override void OnStart(MiniGameView miniGameView)
        {
            base.OnStart(miniGameView);

            miniGameView.ManagerRefs.InputManager.Actions.UI.Submit.performed += SubmitPerformed;
            hitNumber = 0;
            SetupTarget(0.5f, barElementData.BarSize);

            miniGameView.SetBarNumber(Mathf.Clamp(hitNumber, 0, barElementData.NumberHit).ToString() + "/" + barElementData.NumberHit);
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
        }

        protected override void OnChangeDirection()
        {
            base.OnChangeDirection();
            sucessRatio = Mathf.Clamp01((float)hitNumber/barElementData.NumberHit);
            Stop();
        }

        public override void Stop()
        {
            miniGameView.ManagerRefs.InputManager.Actions.UI.Submit.performed -= SubmitPerformed;
            base.Stop();
        }
    }
}
