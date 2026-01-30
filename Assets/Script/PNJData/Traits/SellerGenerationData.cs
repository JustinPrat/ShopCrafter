using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Traits/Seller Random Trait")]
public class SellerGenerationData : PNJTraitData
{
    [SerializeField]
    private List<RandomSellingItem> randomSelling;

    [SerializeField, OnValueChanged(nameof(ClampAmount))]
    private int sellAmount;

    public override IPNJTraitRuntime GetRuntimeLogic()
    {
        List<RandomSellingItem> items = new List<RandomSellingItem>(randomSelling);

        SellerData sellerData = new SellerData();
        sellerData.InitialStock = new List<SellingItem>();

        for (int i = 0; i < sellAmount; i++)
        {
            RandomSellingItem sellItem = items.GetRandomElement();

            SellingItem sellingItem = new SellingItem();
            sellingItem.item = sellItem.item;
            sellingItem.priceEach = (int)UnityEngine.Random.Range(sellItem.priceEach.x, sellItem.priceEach.y);
            sellingItem.amount = (int)UnityEngine.Random.Range(sellItem.amount.x, sellItem.amount.y);

            sellerData.InitialStock.Add(sellingItem);
            items.Remove(sellItem);
        }

        return new SellerRuntime(sellerData);
    }

    [Serializable]
    private struct RandomSellingItem
    {
        public Item item;
        public Vector2 priceEach;
        public Vector2 amount;
    }

    private void ClampAmount ()
    {
        if (sellAmount > randomSelling.Count)
        {
            sellAmount = randomSelling.Count;
        }
    }
}
