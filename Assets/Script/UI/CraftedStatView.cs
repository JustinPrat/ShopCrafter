using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftedStatView : UIView
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Image rarityIconImage;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [SerializeField]
    private TextMeshProUGUI priceText;

    [SerializeField]
    private TextMeshProUGUI priceBonusText;

    [SerializeField]
    private Vector2 minMaxRotationToPlayer;

    [SerializeField]
    private float damping = 0.9f;

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);
    }

    public void Setup (CraftedObjectData craftedObjectData, Vector3 pos)
    {
        itemImage.sprite = craftedObjectData.CraftedObjectRecipe.CraftedSprite;
        rarityIconImage.sprite = craftedObjectData.Rarity.RarityIcon;
        nameText.text = craftedObjectData.CraftedObjectRecipe.CraftedName;
        descriptionText.text = craftedObjectData.CraftedObjectRecipe.CraftedDescription;
        priceText.text = craftedObjectData.CraftedObjectRecipe.BasePrice.ToString();

        int bonusPrice = craftedObjectData.GetPrice() - craftedObjectData.CraftedObjectRecipe.BasePrice;
        if (bonusPrice > 0)
            priceBonusText.text = "+" + bonusPrice.ToString();
        else
            priceBonusText.text = "";

        transform.position = pos;
    }

    private void FixedUpdate()
    {
        Vector3 playerPosition = managerRefs.PlayerManager.PlayerBrain.transform.position;

        Vector3 flatDirection = playerPosition - transform.position;
        flatDirection.y = 0f;

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            float yAngle = Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg;
            yAngle += 180f;

            if (yAngle > 180f) yAngle -= 360f;
            yAngle = Mathf.Clamp(yAngle, minMaxRotationToPlayer.x, minMaxRotationToPlayer.y);

            yAngle = Mathf.LerpAngle(transform.localEulerAngles.y, yAngle, damping);

            transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, yAngle, 0f);
        }
    }
}
