using Alchemy.Inspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class TagIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IComparable
{
    private const string HoverBool = "Hover";
    private const string CountTrigger = "Count";

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

    private TagPlaceholderUI anchor;
    private RectTransform parentRect;
    private float lockedYPos;
    private bool isDragging;
    private TagValue tagValue;
    private bool blockOrdering;

    public Action OnDragReleased;
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
        }
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
        tagText.text = tagData.Amount.ToString();
        descTags.text = tagData.Asset.Description;
    }

    public int CountTag(int score)
    {
        animator.SetTrigger(CountTrigger);
        return tagValue.Asset.ApplyTagAsset(score + tagValue.Amount);
    }

    void Update()
    {
        if (!isDragging && anchor != null)
        {
            transform.position = Vector3.Lerp(transform.position, anchor.transform.position, Time.deltaTime * smoothSpeed);
        }
    }

    public void HideTag()
    {
        tagImage.color = Color.black;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool(HoverBool, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool(HoverBool, false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canBeMoved || blockOrdering)
            return;

        isDragging = true;
        lockedYPos = transform.position.y;
        transform.SetAsLastSibling();
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
        
        isDragging = false;
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
