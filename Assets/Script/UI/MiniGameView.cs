using Alchemy.Inspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MiniGameView : UIView
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private ParticleSystem stepParticle;

    [SerializeField]
    private ToCraftItem toCraftItemHolder;

    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private TierList tierList;

    [SerializeField]
    private GameObject targetPrefab;

    [SerializeField]
    private TextMeshProUGUI debugInfos;

    [SerializeField]
    private Image itemImage;

    [SerializeField, Blockquote("Entre 0 (gauche) et 1 (droite), définit la range pour la target")]
    private Vector2 rangeSpawn;

    private List<Item> items;
    private CraftedObjectRecipe craftedObjectRecipe;

    private float barCount = 0f;
    private bool countingUp;

    private int tierCount = 0;
    private float targetPos = 0f;

    private float currentSpeed = 0f;
    private GameObject currentTarget;

    private bool CanUpgrade => tierCount < tierList.Tiers.Count;
    public CraftingTable CurrentCraftingTable { get; set; }

    private void Awake()
    {
        managerRefs.CraftingManager.OnItemsConsumed += Setup;
        currentTarget = Instantiate(targetPrefab, progressBar.transform);
    }

    private void OnCraftHit (InputAction.CallbackContext ctx)
    {
        OnItemClick();
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        tierCount = 0;
        countingUp = true;
        barCount = 0f;

        if (isOn)
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started += OnCraftHit;
        }
        else
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started -= OnCraftHit;
        }
    }

    public void Setup (List<Item> itemConsumed)
    {
        items = itemConsumed;
        craftedObjectRecipe = managerRefs.CraftingManager.PoolCraftedItem(items);
        toCraftItemHolder.Setup(craftedObjectRecipe);
        toCraftItemHolder.ValidateButton.onClick.AddListener(OnItemClick);

        itemImage.sprite = craftedObjectRecipe.CraftedSprite;

        SetupTarget();
        currentSpeed = tierList.Tiers[tierCount].TierSpeed;
    }

    private void SetupTarget ()
    {
        RectTransform progressRect = progressBar.GetComponent<RectTransform>();

        targetPos = Random.Range(rangeSpawn.x, rangeSpawn.y);
        currentTarget.transform.localScale = new Vector3(tierList.Tiers[tierCount].TierTargetSize * progressRect.sizeDelta.x, currentTarget.transform.localScale.y, currentTarget.transform.localScale.z);
        currentTarget.transform.SetSiblingIndex(currentTarget.transform.GetSiblingIndex() - 1);

        Vector3 leftBorn = progressBar.transform.position - Vector3.right * progressRect.sizeDelta.x / 2;
        float xPosFromLeft = targetPos * progressRect.sizeDelta.x;
        leftBorn.x += xPosFromLeft;

        currentTarget.transform.position = leftBorn;
    }

    private void EndGame ()
    {
        tierCount = 0;
        CurrentCraftingTable.SpawnCraftedItem(craftedObjectRecipe, items, tierCount);
        managerRefs.CraftingManager.OnItemCrafted();
        managerRefs.UIManager.ToggleMiniGameView(false, CurrentCraftingTable);
    }

    private void OnItemClick ()
    {
        RectTransform progressRect = progressBar.GetComponent<RectTransform>();
        if (targetPos - (tierList.Tiers[tierCount].TierTargetSize/2) <= barCount && targetPos + (tierList.Tiers[tierCount].TierTargetSize / 2) >= barCount)
        {
            //win
            tierCount += 1;

            if (CanUpgrade)
            {
                SetupTarget();
                currentSpeed = tierList.Tiers[tierCount].TierSpeed;
            }
            else
            {
                EndGame();
            }
        }
        else
        {
            //lose
            EndGame();
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (countingUp)
        {
            barCount += Time.deltaTime * currentSpeed;

            if (barCount >= 1f)
            {
                barCount = 1f;
                countingUp = false;
            }
        }
        else 
        {
            barCount -= Time.deltaTime * currentSpeed;

            if (barCount <= 0f)
            {
                barCount = 0f;
                countingUp = true;
            }
        }

        progressBar.value = barCount;

        debugInfos.text = "actual pos : " + barCount + "\ntargetPos : " + targetPos + "\ntarget size : " + tierList.Tiers[tierCount].TierTargetSize;
    }
}
