using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TagIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IComparable
{
    private const string HoverBool = "Hover";
    private const string CountTrigger = "Count";

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Image tagImage;

    [SerializeField]
    private TextMeshProUGUI tagText;

    [SerializeField]
    private TextMeshProUGUI tagBonusText;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI descTags;

    [SerializeField]
    private LayoutElement layoutElement;

    [SerializeField]
    private AdvancedButton button;

    [SerializeField]
    private bool canBeMoved;

    [SerializeField, ShowIf(nameof(canBeMoved))]
    private GameObject anchorPrefab;

    [SerializeField, ShowIf(nameof(canBeMoved))]
    private float smoothSpeed = 15f;

    [SerializeField]
    private Sequencer.Sequencer pressedGamepadSequence;

    private TagPlaceholderUI anchor;
    private RectTransform parentRect;
    private float lockedYPos;
    private TagValue tagValue;
    private bool blockOrdering;
    private DragType currentDragType;
    private bool isHovered;
    private bool isSelected;
    private bool isMousePressed;
    private bool swapped;
    private Navigation savedNavigation;

    public Action OnDragReleased;
    public Action OnDragStarted;

    public enum DragType
    {
        None,
        Mouse,
        Controller
    }

    public TagValue TagValue => tagValue;
    public GameObject Anchor => anchor.gameObject;
    public int AnchorIndex => anchor.transform.GetSiblingIndex();

    private void Awake()
    {
        if (canBeMoved)
        {
            anchor = Instantiate(anchorPrefab).GetComponent<TagPlaceholderUI>();
            anchor.Setup(GetComponent<RectTransform>());
            anchor.transform.SetParent(transform.parent, false);
            anchor.transform.SetSiblingIndex(transform.GetSiblingIndex());

            parentRect = anchor.transform.parent as RectTransform;
            transform.SetParent(transform.parent.parent);
            layoutElement.ignoreLayout = true;

            if (button != null)
            {
                savedNavigation = button.navigation;
                button.OnLeftClick.AddListener(OnLeftClick);
                button.OnLeftClickReleased.AddListener(OnLeftClickReleased);
                button.OnLeftClickDown += OnLeftClickDown;
                button.OnLeftClickUp += OnLeftClickUp;
            }
        }

        if (button != null)
        {
            button.OnSelected += OnSelect;
            button.OnDeselected += OnDeSelect;
        }
    }

    private void OnDisable()
    {
        CancelActiveDrag();
        isHovered = false;
        isSelected = false;
        isMousePressed = false;
        UpdateHoverState();
    }

    private void OnDestroy()
    {
        CancelActiveDrag();

        if (canBeMoved && button != null)
        {
            button.OnLeftClick.RemoveListener(OnLeftClick);
            button.OnLeftClickReleased.RemoveListener(OnLeftClickReleased);
            button.OnLeftClickDown -= OnLeftClickDown;
            button.OnLeftClickUp -= OnLeftClickUp;
        }

        if (button != null)
        {
            button.OnSelected -= OnSelect;
            button.OnDeselected -= OnDeSelect;
        }
    }

    private void UpdateHoverState()
    {
        if (animator == null)
            return;

        bool shouldHover = currentDragType == DragType.None && !isMousePressed && (isHovered || isSelected);
        animator.SetBool(HoverBool, shouldHover);
    }

    private void OnSelect(AdvancedButton btn)
    {
        isSelected = true;
        UpdateHoverState();
    }

    private void OnDeSelect(AdvancedButton btn)
    {
        isSelected = false;
        UpdateHoverState();
    }

    // Controller
    private void OnLeftClick(AdvancedButton btn)
    {
        if (!canBeMoved || blockOrdering || !managerRefs.InputManager.IsGamepad)
            return;

        if (currentDragType == DragType.Controller)
            return;

        if (pressedGamepadSequence != null)
            pressedGamepadSequence.StartSequence();

        currentDragType = DragType.Controller;
        savedNavigation = btn.navigation;
        btn.navigation = new Navigation { mode = Navigation.Mode.None };

        managerRefs.InputManager.Actions.Player.Navigate.performed += SwapPlace;

        transform.SetAsLastSibling();
        UpdateHoverState();
        OnDragStarted?.Invoke();
    }

    private void SwapPlace(InputAction.CallbackContext ctx)
    {
        if (currentDragType != DragType.Controller || anchor == null || parentRect == null)
            return;

        Vector2 input = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) < 0.3f)
        {
            swapped = false;
            return;
        }

        if (swapped)
            return;

        swapped = true;
        int newSiblingIndex = anchor.transform.GetSiblingIndex();
        int delta = input.x > 0f ? 1 : -1;
        int targetIndex = newSiblingIndex + delta;

        if (targetIndex >= 0 && targetIndex < parentRect.childCount)
        {
            anchor.transform.SetSiblingIndex(targetIndex);
        }
    }

    private void OnLeftClickReleased(AdvancedButton btn)
    {
        if (currentDragType == DragType.Controller)
        {
            EndControllerDrag();
            OnDragReleased?.Invoke();
        }

        UpdateHoverState();
    }

    private void EndControllerDrag()
    {
        swapped = false;

        if (managerRefs != null && managerRefs.InputManager != null)
        {
            managerRefs.InputManager.Actions.Player.Navigate.performed -= SwapPlace;
        }

        if (button != null)
        {
            button.navigation = savedNavigation;
        }

        currentDragType = DragType.None;
    }

    private void CancelActiveDrag()
    {
        if (currentDragType == DragType.Controller)
        {
            EndControllerDrag();
            OnDragReleased?.Invoke();
        }
        else if (currentDragType == DragType.Mouse)
        {
            currentDragType = DragType.None;
            OnDragReleased?.Invoke();
        }
    }

    public void BlockOrdering()
    {
        blockOrdering = true;
        CancelActiveDrag();
        UpdateHoverState();
    }

    public void UpdateValue()
    {
        Setup(tagValue);
    }

    public void Setup(TagValue tagData)
    {
        tagValue = tagData;
        tagImage.color = Color.white;
        tagImage.sprite = tagData.Asset.Icon;
        tagText.text = tagData.Amount.BaseValue.ToString();

        float amountBonus = tagData.Amount.Value - tagData.Amount.BaseValue;
        tagBonusText.text = amountBonus > 0 ? "+" + amountBonus : "";

        descTags.text = tagData.Asset.Description;
    }

    public int CountTag(int score)
    {
        animator.SetTrigger(CountTrigger);
        return tagValue.Asset.ApplyTagAsset(score + tagValue.Amount.Value);
    }

    public void RemovePreSelectionTag(List<TagValue> tagValues, int tagIndex)
    {
        tagValue.Asset.PreSelectionRemoveTagAsset(tagValues, tagIndex);
    }

    public void ApplyPreSelectionTag(List<TagValue> tagValues, int tagIndex)
    {
        tagValue.Asset.PreSelectionApplyTagAsset(tagValues, tagIndex);
    }

    private void Update()
    {
        if (currentDragType != DragType.Mouse && anchor != null)
        {
            transform.position = Vector3.Lerp(transform.position, anchor.transform.position, Time.unscaledDeltaTime * smoothSpeed);
        }
    }

    public void HideTag()
    {
        tagImage.color = Color.black;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateHoverState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateHoverState();
    }

    // Mouse
    private void OnLeftClickDown(AdvancedButton btn)
    {
        isMousePressed = true;
        UpdateHoverState();
    }

    private void OnLeftClickUp(AdvancedButton btn)
    {
        isMousePressed = false;
        UpdateHoverState();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canBeMoved || blockOrdering || eventData.button != PointerEventData.InputButton.Left)
            return;

        currentDragType = DragType.Mouse;
        lockedYPos = transform.position.y;
        transform.SetAsLastSibling();
        UpdateHoverState();
        OnDragStarted?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentDragType != DragType.Mouse || !canBeMoved || blockOrdering)
            return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            transform.position = new Vector3(globalMousePos.x, lockedYPos, transform.position.z);
        }

        int newSiblingIndex = parentRect.childCount - 1;

        for (int i = 0; i < parentRect.childCount; i++)
        {
            if (anchor.transform.GetSiblingIndex() == i) continue;

            if (transform.position.x < parentRect.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (anchor.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }
                break;
            }
        }

        anchor.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentDragType != DragType.Mouse)
            return;

        currentDragType = DragType.None;
        UpdateHoverState();
        OnDragReleased?.Invoke();
    }

    public int CompareTo(object obj)
    {
        TagIconUI b = obj as TagIconUI;
        if (b == null) return 1;

        if (AnchorIndex < b.AnchorIndex) return -1;
        if (AnchorIndex > b.AnchorIndex) return 1;
        return 0;
    }
}