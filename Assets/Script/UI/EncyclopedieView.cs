using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopedieView : UIView
{
    [SerializeField]
    private Book book;

    [SerializeField] 
    private PageUI pageUIPrefab;

    [SerializeField]
    private AutoFlip flip;

    [SerializeField]
    private float delayBetweenPageFlip = 0.5f;

    [SerializeField]
    private float bookYOffsetAppear = -1200;

    private List<PageUI> pageItemUI = new List<PageUI>();

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            book.OnRightDrag.AddListener(OnRightDrag);
            book.OnLeftDrag.AddListener(OnLeftDrag);
            book.OnFinishFlip.AddListener(OnFlip);
            book.OnAbortFlip.AddListener(OnFlip);
            managerRefs.InputManager.Actions.Player.Back.started += EscapePressed;
            managerRefs.InputManager.Actions.Player.Navigate.started += NavigateStarted;

            OnFlip();
            AppearAnim();
        }
        else
        {
            book.OnRightDrag.RemoveListener(OnRightDrag);
            book.OnLeftDrag.RemoveListener(OnLeftDrag);
            book.OnFinishFlip.RemoveListener(OnFlip);
            book.OnAbortFlip.RemoveListener(OnFlip);
            managerRefs.InputManager.Actions.Player.Back.started -= EscapePressed;
            managerRefs.InputManager.Actions.Player.Navigate.started -= NavigateStarted;

            ClearBook();
        }
    }

    private void AppearAnim()
    {
        book.transform.localPosition = new Vector3(book.transform.localPosition.x, bookYOffsetAppear, book.transform.localPosition.z);
        book.currentPage = book.TotalPageCount;

        Sequence animSequence = Sequence.Create(cycles: 1)
            .Chain(Tween.LocalPositionY(book.transform, 0, 1f, ease: Ease.InOutSine))
            .InsertCallback(0.5f, () => StartCoroutine(GoToPage(1)));
    }

    private IEnumerator GoToPage(int pageIndex)
    {
        while (true)
        {
            if (pageIndex == book.currentPage || pageIndex == book.currentPage - 1)
            {
                break;
            }

            if (pageIndex > book.currentPage)
            {
                flip.FlipRightPage();
            }
            if (pageIndex < book.currentPage - 1)
            {
                flip.FlipLeftPage();
            }

            yield return new WaitForSeconds(delayBetweenPageFlip);
        }
    }

    private void EscapePressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        managerRefs.UIManager.ToggleEncyclopedieView(false);
    }

    private void NavigateStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        bool isInputRight = obj.ReadValue<Vector2>().x > 0 ? false : true;

        if (isInputRight)
        {
            flip.FlipRightPage();
        }
        else
        {
            flip.FlipLeftPage();
        }
    }

    private void OnFlip ()
    {
        ClearBook();
        UpdateBook(book.currentPage - 1, book.LeftNextLayout.transform, book.RightNextLayout.transform);
    }

    private void OnRightDrag()
    {
        ClearBook();
        UpdateBook(book.currentPage - 1, book.LeftNextLayout.transform, book.RightLayout.transform, book.LeftLayout.transform, book.RightNextLayout.transform);
    }

    private void OnLeftDrag()
    {
        ClearBook();
        UpdateBook(book.currentPage - 3, book.LeftNextLayout.transform, book.RightLayout.transform, book.LeftLayout.transform, book.RightNextLayout.transform);
    }

    private void UpdatePage (int pageIndex, Transform parent)
    {
        for (int i = (pageIndex) * 4; i < (pageIndex + 1) * 4; i++)
        {
            if (i >= managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool.Count)
            {
                return;
            }

            PageUI craftItemUI = Instantiate(pageUIPrefab, parent);
            CraftedObjectRecipe recipe = managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool[i];
            craftItemUI.Setup(recipe, managerRefs.CraftingManager.CraftedRecipes.Contains(recipe), managerRefs.CraftingManager.BlueprintRecipes.Contains(recipe));
            pageItemUI.Add(craftItemUI);
        }
    }

    private void ClearBook ()
    {
        for (int i = book.LeftNextLayout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(book.LeftNextLayout.transform.GetChild(i).gameObject);
        }

        for (int i = book.RightLayout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(book.RightLayout.transform.GetChild(i).gameObject);
        }

        for (int i = book.LeftLayout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(book.LeftLayout.transform.GetChild(i).gameObject);
        }

        for (int i = book.RightNextLayout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(book.RightNextLayout.transform.GetChild(i).gameObject);
        }
    }

    private void UpdateBook (int farLeftIndex, Transform farLeft, Transform left, Transform right = default, Transform farRight = default)
    {
        if (farLeftIndex >= 0)
        {
            UpdatePage(farLeftIndex, farLeft);
        }

        if (farLeftIndex + 1 >= 0)
        {
            UpdatePage(farLeftIndex + 1, left);
        }

        if (farLeftIndex + 2 >= 0)
        {
            UpdatePage(farLeftIndex + 2, right);
        }

        if (farLeftIndex + 3 <= book.bookPages.Length - 1)
        {
            UpdatePage(farLeftIndex + 3, farRight);
        }
    }
}
