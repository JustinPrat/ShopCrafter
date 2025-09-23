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
        }
        else
        {
            book.OnRightDrag.RemoveListener(OnRightDrag);
        }
    }

    private void Start()
    {
        book.OnRightDrag.AddListener(OnRightDrag);
    }

    public void OnRightDrag ()
    {
        if (book.currentPage > 0)
        {
            for (int i = (book.currentPage - 1) * 4; i < (book.currentPage) * 4; i++)
            {
                if (i >= managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool.Count)
                {
                    return;
                }

                PageUI craftItemUI = Instantiate(pageUIPrefab, book.LeftNextLayout.transform);
                CraftedObjectRecipe recipe = managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool[i];
                craftItemUI.Setup(recipe, managerRefs.CraftingManager.CraftedRecipes.Contains(recipe));
                pageItemUI.Add(craftItemUI);
            }
        }

        for (int i = (book.currentPage) * 4; i < (book.currentPage + 1) * 4; i++)
        {
            if (i >= managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool.Count)
            {
                return;
            }

            PageUI craftItemUI2 = Instantiate(pageUIPrefab, book.RightLayout.transform);
            CraftedObjectRecipe recipe2 = managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool[i];
            craftItemUI2.Setup(recipe2, managerRefs.CraftingManager.CraftedRecipes.Contains(recipe2));
            pageItemUI.Add(craftItemUI2);
        }

        for (int i = (book.currentPage + 1) * 4; i < (book.currentPage + 2) * 4; i++)
        {
            if (i >= managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool.Count)
            {
                return;
            }

            PageUI craftItemUI3 = Instantiate(pageUIPrefab, book.LeftLayout.transform);
            CraftedObjectRecipe recipe3 = managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool[i];
            craftItemUI3.Setup(recipe3, managerRefs.CraftingManager.CraftedRecipes.Contains(recipe3));
            pageItemUI.Add(craftItemUI3);
        }

        for (int i = (book.currentPage + 2) * 4; i < (book.currentPage + 3) * 4; i++)
        {
            if (i >= managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool.Count)
            {
                return;
            }

            PageUI craftItemUI4 = Instantiate(pageUIPrefab, book.RightNextLayout.transform);
            CraftedObjectRecipe recipe4 = managerRefs.CraftingManager.CurrentCraftedObjectPool.craftedObjectPool[i];
            craftItemUI4.Setup(recipe4, managerRefs.CraftingManager.CraftedRecipes.Contains(recipe4));
            pageItemUI.Add(craftItemUI4);
        }
    }
}
