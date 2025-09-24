using System.Collections.Generic;
using UnityEngine;

public class PriceCheckView : UIView
{
    [SerializeField]
    private Transform verticalLayoutTransform;

    [SerializeField]
    private PriceTypeUI priceTypeUIPrefab;

    [SerializeField]
    private ManagerRefs managerRefs;

    public void Setup (Vector3 pos)
    {
        transform.position = pos;
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            foreach (KeyValuePair<ECraftedType, SellManager.PriceVariation> priceVariation in managerRefs.SellManager.PriceVariations)
            {
                PriceTypeUI priceTypeUI = Instantiate(priceTypeUIPrefab, verticalLayoutTransform);
                priceTypeUI.Setup(priceVariation.Key.ToString(), priceVariation.Value.currentPricePercent.ToString("F1") + "%");
            }
        }
        else
        {
            for (int i = verticalLayoutTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(verticalLayoutTransform.GetChild(i).gameObject);
            }
        }
    }
}
