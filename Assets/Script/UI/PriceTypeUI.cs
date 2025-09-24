using TMPro;
using UnityEngine;

public class PriceTypeUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI elementTypeName;

    [SerializeField]
    private TextMeshProUGUI elementPricePercent;

    public void Setup (string elementName, string elementPrice)
    {
        elementTypeName.text = elementName;
        elementPricePercent.text = elementPrice;
    }
}
