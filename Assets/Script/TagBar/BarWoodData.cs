using UnityEngine;

[CreateAssetMenu(fileName = "BarWoodData", menuName = "ShopCrafter/BarWoodData")]
public class BarWoodData : BarData
{
    public float BarSize = 0.3f;
    public float OnTargetSpeedMultiplier;
    public int NumberHit = 3;
    public GameObject HitSucessFeedbackPrefab;

    public override BarBehaviour GetBehaviour()
    {
        BarWoodBehavior stats = new BarWoodBehavior(this);
        return stats;
    }

    public class BarWoodBehavior : BarBehaviour
    {
        private BarWoodData barElementData;
        private bool requireLeftMove;
        private int hitNumber;

        public BarWoodBehavior(BarWoodData data) : base(data)
        {
            barElementData = data;
        }

        public override void OnStart(MiniGameView miniGameView)
        {
            base.OnStart(miniGameView);
            miniGameView.ManagerRefs.InputManager.Actions.Player.SideMove.performed += OnSideMovePerformed;
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

            sucessRatio = Mathf.Clamp01((float)hitNumber / barElementData.NumberHit);
            Stop();
        }

        public override void Stop()
        {
            miniGameView.ManagerRefs.InputManager.Actions.Player.SideMove.performed -= OnSideMovePerformed;
            base.Stop();
        }

        private void OnSideMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            bool performedLeft = ctx.ReadValue<float>() < -0.9f;
            bool performedRight = ctx.ReadValue<float>() > 0.9f;

            if (IsOnTarget(barPos) && ((requireLeftMove && performedLeft) || (!requireLeftMove && performedRight)))
            {
                ValidateMove();
            }
        }

        private void ValidateMove()
        {
            hitNumber++;
            miniGameView.SetBarNumber(Mathf.Clamp(hitNumber, 0, barElementData.NumberHit).ToString() + "/" + barElementData.NumberHit);
            requireLeftMove = !requireLeftMove;

            if (barElementData.HitSucessFeedbackPrefab != null && TryGetHoveredTarget(out Target hoveredTarget))
            {
                miniGameView.InstantiateParticles(barElementData.HitSucessFeedbackPrefab, hoveredTarget.TargetRect.position);
            }
        }
    }
}
