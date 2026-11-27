using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BarTextileData", menuName = "ShopCrafter/BarTextileData")]
public class BarTextileData : BarData
{
    public float BarSize = 0.1f;
    public float OnTargetSpeedMultiplier;
    public int NumberHit = 3;
    public GameObject HitSucessFeedbackPrefab;

    public override BarBehaviour GetBehaviour()
    {
        BarTextileBehavior stats = new BarTextileBehavior(this);
        return stats;
    }

    public class BarTextileBehavior : BarBehaviour
    {
        private BarTextileData barElementData;
	    private int hitNumber;
        private Dictionary<Vector2, bool> directionValidations;
        private List<Vector2> directionKeys;
        public BarTextileBehavior(BarTextileData data) : base(data)
        {
            barElementData = data;
            directionValidations = new Dictionary<Vector2, bool>();
            directionValidations.Add(new Vector2(0, 1), false);
            directionValidations.Add(new Vector2(-1, 0), false);
            directionValidations.Add(new Vector2(0, -1), false);
            directionValidations.Add(new Vector2(1, 0), false);

            directionKeys = directionValidations.Keys.ToList();
        }

        public override void OnStart(MiniGameView miniGameView)
        {
            base.OnStart(miniGameView);
            miniGameView.ManagerRefs.InputManager.Actions.Player.Navigate.performed += NavigatePerformed;
            miniGameView.ManagerRefs.InputManager.Actions.Player.Navigate.canceled += NavigatePerformed;

            hitNumber = 0;
            for (int i = 1; i < barElementData.NumberHit + 1; i++)
            {
                SetupTarget((float)i / (barElementData.NumberHit + 1), barElementData.BarSize);
            }

            miniGameView.SetBarNumber(Mathf.Clamp(hitNumber, 0, barElementData.NumberHit).ToString() + "/" + barElementData.NumberHit);
        }

        private void NavigatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            Vector2 inputDirection = ctx.ReadValue<Vector2>();

            if (inputDirection.magnitude < 0.8f)
            {
                ResetValidations();
                return;
            }

            ProcessValidations(inputDirection);
            CheckValidations();
        }

        private void ProcessValidations(Vector2 inputDirection)
        {
            foreach (Vector2 directionValidation in directionKeys)
            {
                if (Vector2.Dot(directionValidation, inputDirection) > 0.8f && !directionValidations[directionValidation])
                {
                    directionValidations[directionValidation] = true;
                }
            }
        }

        private void ResetValidations()
        {
            foreach (Vector2 directionValidation in directionKeys)
            {
                directionValidations[directionValidation] = false;
            }
        }

        private void CheckValidations()
        {
            bool isFinished = true;
            foreach (KeyValuePair<Vector2, bool> directionValidation in directionValidations)
            {
                if (!directionValidation.Value)
                {
                    isFinished = false;
                }
            }

            if (isFinished)
            {
                ResetValidations();

                if (IsOnTarget(barPos))
                    ValidateMove();
            }
        }

        private void ValidateMove()
        {
            if (TryGetHoveredTarget(out Target target))
            {
                currentTargets.Remove(target);
                Destroy(target.TargetRect.gameObject);
            }

            hitNumber++;
            miniGameView.SetBarNumber(Mathf.Clamp(hitNumber, 0, barElementData.NumberHit).ToString() + "/" + barElementData.NumberHit);

            if (barElementData.HitSucessFeedbackPrefab != null && TryGetHoveredTarget(out Target hoveredTarget))
            {
                miniGameView.InstantiateParticles(barElementData.HitSucessFeedbackPrefab, hoveredTarget.TargetRect.position);
            }
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
            miniGameView.ManagerRefs.InputManager.Actions.Player.Navigate.performed -= NavigatePerformed;
            miniGameView.ManagerRefs.InputManager.Actions.Player.Navigate.canceled -= NavigatePerformed;
            base.Stop();
        }
    }
}
