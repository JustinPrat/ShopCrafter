using TMPro;
using UnityEngine;

public class CoinAmountUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinAmountText;

    [SerializeField]
    private ManagerRefs managerRefs;

    private void Start()
    {
        coinAmountText.text = managerRefs.SellManager.CoinAmount.ToString();
        managerRefs.GameEventsManager.OnMoneyUpdated += UpdateCoinAmountUI;
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.OnMoneyUpdated -= UpdateCoinAmountUI;
    }

    private void UpdateCoinAmountUI(int amount)
    {
        coinAmountText.text = amount.ToString();
    }
}
