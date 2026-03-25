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
    private Transform tagMaterialHolder;

    [SerializeField]
    private GameObject tagMaterialPrefab;

    private List<TagIconUI> tagIcons = new List<TagIconUI>();

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
                for (int i = 0; i < tagIcons.Count; i++)
                {
                    tagIcons[i].HideTag();
                }
            }
        }
    }

    private void SetupCraftMaterials (CraftedObjectRecipe recipe)
    {
        ClearCraftMaterials();
        AddCraftMaterials(recipe);
    }

    private void ClearCraftMaterials()
    {
        for (int i = tagIcons.Count - 1; i >= 0; i--)
        {
            Destroy(tagIcons[i].gameObject);
        }

        tagIcons.Clear();
    }

    private void AddCraftMaterials(CraftedObjectRecipe recipe)
    {
        for (int i = 0; i < recipe.RequiredTags.Count; i++)
        {
            TagIconUI tagIcon = Instantiate(tagMaterialPrefab, tagMaterialHolder).GetComponent<TagIconUI>();
            tagIcon.Setup(recipe.RequiredTags[i]);
            tagIcons.Add(tagIcon);
        }
    }
}
