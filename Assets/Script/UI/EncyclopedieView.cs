using System.Collections.Generic;
using UnityEngine;

public class EncyclopedieView : UIView
{
    [SerializeField]
    private Book book;

    [SerializeField] 
    private PageUI pageUIPrefab;

    [SerializeField]
    private ManagerRefs managerRefs;

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

            OnFlip();
        }
        else
        {
            book.OnRightDrag.RemoveListener(OnRightDrag);
            book.OnLeftDrag.RemoveListener(OnLeftDrag);
            book.OnFinishFlip.RemoveListener(OnFlip);
            book.OnAbortFlip.RemoveListener(OnFlip);
            managerRefs.InputManager.Actions.Player.Back.started -= EscapePressed;
        }
    }

    private void EscapePressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        managerRefs.UIManager.ToggleEncyclopedieView(false);
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

        UpdatePage(farLeftIndex + 1, left);
        UpdatePage(farLeftIndex + 2, right);

        if (farLeftIndex + 3 >= book.bookPages.Length)
        {
            UpdatePage(farLeftIndex + 3, farRight);
        }
    }
}
