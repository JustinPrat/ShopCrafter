using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageUI : MonoBehaviour
{
    [SerializeField] 
    private Image iconObject;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI descText;

    public void Setup (CraftedObjectRecipe recipe, bool isValidated)
    {
        iconObject.sprite = recipe.CraftedSprite;

        if (isValidated)
        {
            iconObject.color = Color.white;
            nameText.text = recipe.CraftedName;
            descText.text = recipe.CraftedDescription;
        }
        else
        {
            iconObject.color = Color.black;
            nameText.text = "...";
            descText.text = "...";
        }
    }
}
