using TMPro;
using UnityEngine;

public class BonusTagGameUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textNumberIncrease;

    [SerializeField]
    private TextMeshProUGUI textTypeIncrease;

    public void Setup(int value, bool isRarity)
    {
        if (isRarity)
        {
            textNumberIncrease.text = "+" + value.ToString();
            textTypeIncrease.text = "rarity";
        }
        else
        {
            textNumberIncrease.text = "+" + value;
            textTypeIncrease.text = "value";
        }
    }
}
