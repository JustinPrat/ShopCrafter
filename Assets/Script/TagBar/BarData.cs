using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarData", menuName = "ShopCrafter/BarData")]
public class BarData : ScriptableObject
{
    public float BarSpeed = 0.5f;
    public int MinTagAmount;
    public GameObject AnimatedTuto;

    public virtual BarBehaviour GetBehaviour()
    {
        BarBehaviour stats = new BarBehaviour(this);
        return stats;
    }
}

public class BarBehaviour
{
    protected BarData data;
    protected bool countingUp = true;
    protected float baseSpeed;
    protected float currentSpeed;
    protected float barPos;
    protected float sucessRatio;
    protected MiniGameView miniGameView;
    protected List<Target> currentTargets = new List<Target>();

    protected class Target
    {
        public RectTransform TargetRect;
        public float Pos;
        public float Size;
    }

    public BarBehaviour (BarData data)
    {
        this.data = data;
    }

    public virtual void OnStart(MiniGameView miniGameView) 
    { 
        this.miniGameView = miniGameView;
        currentSpeed = data.BarSpeed;
        baseSpeed = currentSpeed;
    }

    public virtual void OnUpdate()
    {
        if (countingUp)
        {
            barPos += Time.deltaTime * currentSpeed;

            if (barPos >= 1f)
            {
                barPos = 1f;
                countingUp = false;
                OnChangeDirection();
            }
        }
        else
        {
            barPos -= Time.deltaTime * currentSpeed;

            if (barPos <= 0f)
            {
                barPos = 0f;
                countingUp = true;
                OnChangeDirection();
            }
        }

        miniGameView.UpdateBarPos(barPos);
    }

    protected virtual void OnChangeDirection()
    {
    }

    public virtual void Stop()
    {
        for (int i = currentTargets.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(currentTargets[i].TargetRect.gameObject);
        }

        currentTargets.Clear();
        miniGameView.ValidateTag(sucessRatio);
    }

    protected void SetupTarget(float pos, float size)
    {
        RectTransform progressRect = miniGameView.ProgressBar.GetComponent<RectTransform>();
        RectTransform newBar = miniGameView.MakeTarget();

        currentTargets.Add(new Target() { TargetRect = newBar, Pos = pos, Size = size});
        miniGameView.SetSizePosition(newBar, progressRect, pos, size);
    }

    protected bool IsOnTarget(float barPos)
    {
        bool isOn = false;
        foreach (Target target in currentTargets)
        {
            if (target.Pos - (target.Size / 2) <= barPos && target.Pos + (target.Size / 2) >= barPos)
            {
                isOn = true; 
                break;
            }
        }

        return isOn;
    }

    protected bool TryGetHoveredTarget(out Target hoveredTarget)
    {
        hoveredTarget = null;
        foreach (Target target in currentTargets)
        {
            if (target.Pos - (target.Size / 2) <= barPos && target.Pos + (target.Size / 2) >= barPos)
            {
                hoveredTarget = target;
                return true;
            }
        }

        return false;
    }
}
