using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToCraftItem : MonoBehaviour
{
    private const string Fade = "_Fade";
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Image itemShadowImage;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private Button validateButton;

    [SerializeField]
    private float shadowSpeed = 1;

    public Button ValidateButton => validateButton;

    private float targetShadowFade;

    public void Setup(CraftedObjectRecipe itemCraft)
    {
        itemImage.sprite = itemCraft.CraftedSprite;
        itemShadowImage.sprite = itemCraft.CraftedSprite;
        itemName.text = itemCraft.CraftedName;

        targetShadowFade = 1;
        itemShadowImage.material.SetFloat(Fade, 1);
    }

    public void SetShadowFadeRatio(float ratio)
    {
        targetShadowFade = ratio;
    }

    private void Update()
    {
        float currentFade = itemShadowImage.material.GetFloat(Fade);
        if (currentFade > targetShadowFade || currentFade < targetShadowFade)
        {
            itemShadowImage.material.SetFloat(Fade, Mathf.Lerp(currentFade, targetShadowFade, shadowSpeed * Time.unscaledDeltaTime));
        }
    }
}
