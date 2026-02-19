using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftedRecipeDayUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI recipeName;

    [SerializeField]
    private Image recipeImage;

    [SerializeField]
    private GameObject newIndicator;

    public void Setup(CraftedObjectData craftedObjectData)
    {
        recipeName.text = craftedObjectData.CraftedObjectRecipe.CraftedName;
        recipeImage.sprite = craftedObjectData.CraftedObjectRecipe.CraftedSprite;

        if (craftedObjectData.IsNew)
        {
            newIndicator.SetActive(true);
        }
    }
}
