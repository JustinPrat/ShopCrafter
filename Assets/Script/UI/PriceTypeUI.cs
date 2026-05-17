using TMPro;
using UnityEngine;

public class PriceTypeUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI elementTypeName;

    [SerializeField]
    private TextMeshProUGUI elementPricePercent;

    public void Setup (string elementName, string elementPrice, bool isIncreased)
    {
        elementTypeName.text = elementName;
        elementPricePercent.text = elementPrice;

        if (isIncreased)
        {
            elementPricePercent.color = Color.green;
        }
        else
        {
            elementPricePercent.color = Color.red;
        }
    }
}
