using TMPEffects.TMPEvents;
using UnityEngine;

[CreateAssetMenu(fileName = "BarData", menuName = "ShopCrafter/BarData")]
public class BarData : ScriptableObject
{
    public virtual BarBehaviour GetBehaviour()
    {
        BarBehaviour stats = new BarBehaviour(this);
        return stats;
    }
}

public class BarBehaviour
{
    protected BarData data;
    protected bool countingUp;
    protected float currentSpeed;
    protected TierList tierlist;
    protected float barPos;

    public BarBehaviour (BarData data)
    {
        this.data = data;
    }

    public virtual void OnStart(MiniGameView miniGameView, TierList tierList) 
    { 
        this.tierlist = tierList;
        currentSpeed = tierList.Tiers[0].TierSpeed;
    }

    public virtual float OnUpdate(MiniGameView miniGameView, float currentSpeed)
    {
        if (countingUp)
        {
            barPos += Time.deltaTime * currentSpeed;

            if (barPos >= 1f)
            {
                barPos = 1f;
                countingUp = false;
            }
        }
        else
        {
            barPos -= Time.deltaTime * currentSpeed;

            if (barPos <= 0f)
            {
                barPos = 0f;
                countingUp = true;
            }
        }

        return barPos;
    }

    public virtual void OnStop(MiniGameView miniGameView) { }

}
