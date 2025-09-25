using System.Collections.Generic;
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

    [SerializeField]
    private List<Image> craftMaterialImages;

    public void Setup (CraftedObjectRecipe recipe, bool isValidated, bool isBlueprint)
    {
        iconObject.sprite = recipe.CraftedSprite;

        if (isValidated)
        {
            iconObject.color = Color.white;
            nameText.text = recipe.CraftedName;
            descText.text = recipe.CraftedDescription;

            SetupCraftMaterials(recipe);
        }
        else
        {
            iconObject.color = Color.black;
            nameText.text = "...";
            descText.text = "...";

            if (isBlueprint)
            {
                SetupCraftMaterials(recipe);
            }
            else
            {
                for (int i = 0; i < craftMaterialImages.Count; i++)
                {
                    Image image = craftMaterialImages[i];
                    image.color = Color.black;
                }
            }
        }
    }

    private void SetupCraftMaterials (CraftedObjectRecipe recipe)
    {
        for (int i = 0; i < craftMaterialImages.Count; i++)
        {
            Image image = craftMaterialImages[i];
            if (i < recipe.RequiredItems.Count)
            {
                image.sprite = recipe.RequiredItems[i].Icon;
                image.color = Color.white;
            }
            else
            {
                image.sprite = null;
                image.color = Color.black;
            }
        }
    }
}
