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
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI descTags;

    [SerializeField]
    private LayoutElement layoutElement;

    [SerializeField]
    private bool canBeMoved;

    [SerializeField, ShowIf(nameof(canBeMoved))]
    private GameObject anchorPrefab;

    [SerializeField, ShowIf(nameof(canBeMoved))]
    private float smoothSpeed = 15f;

    [SerializeField, ShowIf(nameof(canBeMoved))]
    private AdvancedButton button;

    private TagPlaceholderUI anchor;
    private RectTransform parentRect;
    private float lockedYPos;
    private TagValue tagValue;
    private bool blockOrdering;
    private DragType currentDragType;

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

            button.OnLeftClick.AddListener(OnLeftClick);
            button.OnLeftClickReleased.AddListener(OnLeftClickReleased);

            button.OnSelected += OnSelect;
            button.OnDeselected += OnDeSelect;
        }
    }

    private void OnDestroy()
    {
        if (canBeMoved)
        {
            button.OnLeftClick.RemoveListener(OnLeftClick);
            button.OnLeftClickReleased.RemoveListener(OnLeftClickReleased);

            button.OnSelected -= OnSelect;
            button.OnDeselected -=  OnDeSelect;
        }
    }

    private void OnSelect(AdvancedButton button)
    {
        animator.SetBool(HoverBool, true);

        Debug.Log("Pointer Select");
    }
    private void OnDeSelect(AdvancedButton button)
    {
        animator.SetBool(HoverBool, false);

        Debug.Log("Pointer Deselect");
    }

    private void OnLeftClick(AdvancedButton button)
    {
        OnBeginDrag(null);
        currentDragType = DragType.Controller;
        managerRefs.InputManager.Actions.Player.Navigate.started += SwapPlace;
        Navigation noneNav = new Navigation() { mode = Navigation.Mode.None };
        button.navigation = noneNav;
    }

    private void SwapPlace(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        int newSiblingIndex = anchor.transform.GetSiblingIndex();
        int value = input.x > 0 ? 1 : -1;
        if (newSiblingIndex + value >= 0 && newSiblingIndex + value < parentRect.childCount)
        {
            newSiblingIndex += value;
        }

        anchor.transform.SetSiblingIndex(newSiblingIndex);
    }

    private void OnLeftClickReleased(AdvancedButton button)
    {
        managerRefs.InputManager.Actions.Player.Navigate.started -= SwapPlace;
        OnEndDrag(null);
        button.navigation = Navigation.defaultNavigation;
    }

    public void BlockOrdering()
    {
        blockOrdering = true;
    }

    public void Setup(TagValue tagData)
    {
        tagValue = tagData;
        tagImage.color = Color.white;
        tagImage.sprite = tagData.Asset.Icon;
        tagText.text = tagData.Amount.Value.ToString();
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
        Setup(tagValue);
    }

    public void ApplyPreSelectionTag(List<TagValue> tagValues, int tagIndex)
    {
        tagValue.Asset.PreSelectionApplyTagAsset(tagValues, tagIndex);
        Setup(tagValue);
    }

    void Update()
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
        animator.SetBool(HoverBool, true);

        Debug.Log("Pointer Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool(HoverBool, false);

        Debug.Log("Pointer Exit");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canBeMoved || blockOrdering)
            return;

        currentDragType = DragType.Mouse;
        lockedYPos = transform.position.y;
        transform.SetAsLastSibling();
        OnDragStarted?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canBeMoved || blockOrdering)
            return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            transform.position = new Vector3(globalMousePos.x, lockedYPos, transform.position.z);
        }

        int newSiblingIndex = transform.parent.childCount;

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
        if (!canBeMoved || blockOrdering)
            return;

        currentDragType = DragType.None;
        OnDragReleased?.Invoke();
    }

    public int CompareTo(object obj)
    {
        TagIconUI a = this;
        TagIconUI b = obj as TagIconUI;

        if (a.AnchorIndex < b.AnchorIndex)
            return -1;

        if (a.AnchorIndex > b.AnchorIndex)
            return 1;

        return 0;
    }
}
