using Alchemy.Inspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MiniGameView : UIView
{
    [SerializeField]
    private ParticleSystem stepParticle;

    [SerializeField]
    private ToCraftItem toCraftItemHolder;

    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private GameObject targetPrefab;

    [SerializeField]
    private TextMeshProUGUI debugInfos;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Transform targetHolder;

    [SerializeField, Blockquote("Entre 0 (gauche) et 1 (droite), définit la range pour la target")]
    private Vector2 rangeSpawn;

    [SerializeField]
    private int maxLife;

    [SerializeField]
    private Sequencer.Sequencer onHitSequence;

    [SerializeField]
    private LifeHeartUI lifePrefab;

    [SerializeField]
    private RectTransform lifeParent;

    [SerializeField]
    private GameObject goodWord;

    private float barCount = 0f;
    private int tierCount = 0;
    private float targetPos = 0f;
    private float currentSpeed = 0f;
    private GameObject currentTarget;
    private CraftedObjectData craftedObjectData;
    private BarBehaviour currentBarBehaviour;
    private TierList currentTierList;
    private int currentLife;

    private List<LifeHeartUI> lifeHearts = new List<LifeHeartUI>();

    private bool HasTierLeft => tierCount < craftedObjectData.CraftedObjectRecipe.TierList.Tiers.Count;
    public CraftingTable CurrentCraftingTable { get; set; }

    private void Awake()
    {
        currentTarget = Instantiate(targetPrefab, targetHolder);
    }

    private void Start()
    {
        SpawnLife();
    }

    private void OnCraftHit (InputAction.CallbackContext ctx)
    {
        OnItemClick();
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            tierCount = 0;
            barCount = 0f;
            SetLife(maxLife);
            managerRefs.InputManager.Actions.UI.Submit.started += OnCraftHit;
        }
        else
        {
            managerRefs.InputManager.Actions.UI.Submit.started -= OnCraftHit;
        }
    }

    public void Setup (CraftedObjectData data)
    {
        craftedObjectData = data;
        currentTierList = data.CraftedObjectRecipe.TierList;

        toCraftItemHolder.Setup(data.CraftedObjectRecipe);
        toCraftItemHolder.ValidateButton.onClick.AddListener(OnItemClick);
        toCraftItemHolder.SetShadowFadeRatio(1);

        itemImage.sprite = data.CraftedObjectRecipe.CraftedSprite;

        if (data.CraftedObjectRecipe.BarElementData != null)
        {
            currentBarBehaviour = data.CraftedObjectRecipe.BarElementData.GetBehaviour();
            currentBarBehaviour.OnStart(this, currentTierList);
        }

        SetupTarget();
        currentSpeed = currentTierList.Tiers[tierCount].TierSpeed;
    }

    private void SetupTarget ()
    {
        RectTransform progressRect = progressBar.GetComponent<RectTransform>();

        targetPos = Random.Range(rangeSpawn.x, rangeSpawn.y);
        currentTarget.transform.localScale = new Vector3(currentTierList.Tiers[tierCount].TierTargetSize * progressRect.sizeDelta.x, currentTarget.transform.localScale.y, currentTarget.transform.localScale.z);
        currentTarget.transform.SetSiblingIndex(currentTarget.transform.GetSiblingIndex() - 1);

        Vector3 leftBorn = progressBar.transform.position - Vector3.right * progressRect.sizeDelta.x / 2;
        float xPosFromLeft = targetPos * progressRect.sizeDelta.x;
        leftBorn.x += xPosFromLeft;

        currentTarget.transform.position = leftBorn;
    }

    public void ChangeTargetPos ()
    {
        SetupTarget();
    }

    public void InstantiateParticles (GameObject particlePrefab)
    {
        GameObject ps = Instantiate(particlePrefab);
        ps.transform.position = currentTarget.transform.position;
    }

    private void WinGame ()
    {
        if (currentBarBehaviour != null)
        {
            currentBarBehaviour.OnStop(this);
            currentBarBehaviour = null;
        }
        CraftedObject craftedObject = managerRefs.CraftingManager.CraftItem(craftedObjectData);
        CurrentCraftingTable.SpawnCraftedItem(craftedObject);
        managerRefs.GameEventsManager.craftEvents.CraftItem(craftedObject.CraftedData);
        managerRefs.UIManager.HideMiniGameView();

        if (craftedObjectData.IsNew)
        {
            managerRefs.UIManager.ToggleRewardView(true, craftedObjectData.CraftedObjectRecipe);
        }

        tierCount = 0;
    }

    private void LooseGame()
    {
        if (currentBarBehaviour != null)
        {
            currentBarBehaviour.OnStop(this);
            currentBarBehaviour = null;
        }

        managerRefs.UIManager.HideMiniGameView();
        tierCount = 0;
    }

    private void MissHit()
    {
        SetLife(currentLife - 1);
        
        if (onHitSequence != null)
            onHitSequence.StartSequence();
    }

    private void SetLife(int life)
    {
        for (int i = 0; i < lifeHearts.Count; i++)
        {
            LifeHeartUI lifeHeartUI = lifeHearts[i];

            if (i < life && !lifeHeartUI.Activated)
            {
                lifeHeartUI.Activate();
            }
            else if (i >= life && lifeHeartUI.Activated)
            {
                lifeHeartUI.Deactivate();
            }
        }

        currentLife = life;
    }

    private void SpawnLife()
    {
        for (int i = 0; i < maxLife; i++)
        {
            lifeHearts.Add(Instantiate(lifePrefab, lifeParent));
        }

        currentLife = maxLife;
    }

    private void OnItemClick ()
    {
        if (targetPos - (currentTierList.Tiers[tierCount].TierTargetSize/2) <= barCount && targetPos + (currentTierList.Tiers[tierCount].TierTargetSize / 2) >= barCount)
        {
            tierCount += 1;

            if (HasTierLeft)
            {
                stepParticle.transform.position = currentTarget.transform.position;
                stepParticle.Play();
                goodWord.transform.position = currentTarget.transform.position;
                goodWord.gameObject.SetActive(true);

                toCraftItemHolder.SetShadowFadeRatio(1 - ((float)tierCount / currentTierList.Tiers.Count));
                
                SetupTarget();
                currentSpeed = currentTierList.Tiers[tierCount].TierSpeed;
            }
            else
            {
                WinGame();
            }
        }
        else
        {
            MissHit();
            if (currentLife <= 0)
            {
                LooseGame();
            }
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy || currentBarBehaviour == null)
            return;

        barCount = currentBarBehaviour.OnUpdate(this, currentSpeed);
        progressBar.value = barCount;
        debugInfos.text = "actual pos : " + barCount + "\ntargetPos : " + targetPos + "\ntarget size : " + currentTierList.Tiers[tierCount].TierTargetSize;
    }
}
