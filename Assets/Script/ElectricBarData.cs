using UnityEngine;

[CreateAssetMenu(fileName = "ElectricBarData", menuName = "ShopCrafter/ElectricBarData")]
public class ElectricBarData : BarData
{
    public float timeBetweenMove;
    public GameObject particlePrefab;

    public override BarBehaviour GetBehaviour()
    {
        ElectricBarBehaviour stats = new ElectricBarBehaviour(this);
        return stats;
    }
}

public class ElectricBarBehaviour : BarBehaviour
{
    private ElectricBarData currentData => (ElectricBarData)data;
    private float electricTimer;

    public ElectricBarBehaviour(BarData data) : base(data)
    {
    }

    public override float OnUpdate(MiniGameView miniGameView, float currentSpeed)
    {
        if (Time.time >= electricTimer)
        {
            if (currentData.particlePrefab != null)
            {
                miniGameView.InstantiateParticles(currentData.particlePrefab);
            }

            electricTimer = Time.time + currentData.timeBetweenMove;
            miniGameView.ChangeTargetPos();
        }

        return base.OnUpdate(miniGameView, currentSpeed);
    }
}
