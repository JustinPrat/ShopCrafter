using TMPro;
using UnityEngine;

public class CoinAmountUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinAmountText;

    [SerializeField]
    private TextMeshProUGUI bonusAmountText;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private float durationBonus = 1f;

    [SerializeField]
    private float durationCountToTarget = 0.5f;

    [SerializeField]
    private Color positiveBonusColor;

    [SerializeField]
    private Color negativeBonusColor;

    [SerializeField]
    private Sequencer.Sequencer appearCoinBonus;

    [SerializeField]
    private Sequencer.Sequencer disappearCoinBonus;

    private int currentCoins;
    private int bonusCoins;
    private int targetCoins;

    private float timer;
    private bool isCounting;

    private void Start()
    {
        currentCoins = managerRefs.SellManager.CoinAmount;
        coinAmountText.text = currentCoins.ToString();
        managerRefs.GameEventsManager.OnMoneyUpdated += UpdateCoinAmountUI;
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.OnMoneyUpdated -= UpdateCoinAmountUI;
    }

    private void UpdateCoinAmountUI(int amount)
    {
        timer = durationBonus;
        targetCoins = amount;
        bonusCoins = targetCoins - currentCoins;

        bonusAmountText.color = bonusCoins >= 0 ? positiveBonusColor : negativeBonusColor;
        bonusAmountText.text = (bonusCoins >= 0 ? "+" : "-") + Mathf.Abs(bonusCoins).ToString();

        if (appearCoinBonus != null)
            appearCoinBonus.StartSequence();
    }

    private void Update()
    {
        if (timer > 0 && !isCounting)
        {
            timer -= Time.unscaledDeltaTime;

            if (timer <= 0)
            {
                timer = durationCountToTarget;
                isCounting = true;

                if (disappearCoinBonus != null)
                    disappearCoinBonus.StartSequence();
            }
        }

        if (isCounting)
        {
            timer -= Time.unscaledDeltaTime;
            currentCoins = (int)Mathf.Lerp(targetCoins - bonusCoins, targetCoins, durationCountToTarget - timer);
            bonusAmountText.text = (bonusCoins >= 0 ? "+" : "-") + ((int)Mathf.Abs(Mathf.Lerp(bonusCoins, 0, durationCountToTarget - timer))).ToString();

            coinAmountText.text = currentCoins.ToString();

            if (timer <= 0)
            {
                currentCoins = targetCoins;
                coinAmountText.text = currentCoins.ToString();
                isCounting = false;
            }
        }
    }
}
