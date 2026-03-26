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
        Setup(craftedObjectData.CraftedObjectRecipe, craftedObjectData.IsNew);
    }

    public void Setup(CraftedObjectRecipe craftedObjectRecipe, bool isNew)
    {
        recipeName.text = craftedObjectRecipe.CraftedName;
        recipeImage.sprite = craftedObjectRecipe.CraftedSprite;

        if (isNew)
        {
            newIndicator.SetActive(true);
        }
    }
}
