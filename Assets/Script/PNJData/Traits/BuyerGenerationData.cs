using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Traits/Buyer Random Trait")]
public class BuyerGenerationData : PNJTraitData
{
    public Vector2 BuyProbabilityMinMax;

    public float NumberCraftPref;
    public List<ECraftedType> CraftedPrefTypes;

    public Vector2 TimeBeforeTryBuyMinMax;
    public Vector2 TimeWaitingAtTryBuyMinMax;

    public Sprite WaitingIcon;

    public override IPNJTraitRuntime GetRuntimeLogic()
    {
        BuyerData buyerData = new BuyerData();

        buyerData.BaseBuyProbability = UnityEngine.Random.Range(BuyProbabilityMinMax.x, BuyProbabilityMinMax.y);
        buyerData.CraftedPrefTypes = new List<ECraftedType>();

        List<ECraftedType> CraftedPrefTypesTmp = new List<ECraftedType>(CraftedPrefTypes);

        if (NumberCraftPref < CraftedPrefTypes.Count)
        {
            for (int i = 0; i < NumberCraftPref; i++)
            {
                ECraftedType type = CraftedPrefTypes.GetRandomElement();
                buyerData.CraftedPrefTypes.Add(CraftedPrefTypesTmp.GetRandomElement());
                CraftedPrefTypesTmp.Remove(type);
            }
        }
        else
        {
            buyerData.CraftedPrefTypes.AddRange(CraftedPrefTypes);
        }

        buyerData.TimeBeforeTryBuy = UnityEngine.Random.Range(TimeBeforeTryBuyMinMax.x, TimeBeforeTryBuyMinMax.y);
        buyerData.TimeWaitingAtTryBuy = UnityEngine.Random.Range(TimeWaitingAtTryBuyMinMax.x, TimeWaitingAtTryBuyMinMax.y);
        buyerData.WaitingIcon = WaitingIcon;

        return new BuyerRuntime(buyerData);
    }
}
