using TMPro;
using UnityEngine;

public class EndDayView : UIView
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject recipeDayPrefab;

    [SerializeField]
    private Transform recipeDayHolder;

    [SerializeField]
    private TextMeshProUGUI moneyDayText;

    [SerializeField]
    private AdvancedButton nextDayButton;

    private void Awake()
    {
        nextDayButton.OnLeftClick.AddListener(OnLeftClick);
    }

    private void OnDestroy()
    {
        nextDayButton.OnLeftClick.RemoveListener(OnLeftClick);
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            foreach (CraftedObjectData craftedObjectData in managerRefs.GameMetaDataManager.CraftedRecipesToday)
            {
                CraftedRecipeDayUI recipeDay = Instantiate(recipeDayPrefab, recipeDayHolder).GetComponent<CraftedRecipeDayUI>();
                recipeDay.Setup(craftedObjectData);
            }

            moneyDayText.text = managerRefs.GameMetaDataManager.MoneyToday.ToString();
        }
        else
        {
            foreach (Transform child in recipeDayHolder)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void OnLeftClick(AdvancedButton button) 
    {
        managerRefs.PNJManager.StartDay();
        managerRefs.UIManager.ToggleEndDayView(false);
    }
}
