using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemGiverWindow : EditorWindow
{
    private enum MainTab
    {
        ItemGiver,
        Creator
    }

    private enum CreatorSubTab
    {
        Items,
        Recipes
    }

    private MainTab currentTab = MainTab.ItemGiver;
    private CreatorSubTab creatorSubTab = CreatorSubTab.Items;

    private List<Item> allItems = new List<Item>();
    private List<CraftedObjectRecipe> allRecipes = new List<CraftedObjectRecipe>();
    private string searchFilter = "";

    private int selectedTypeIndex = 0;
    private string[] typeOptions = Array.Empty<string>();

    private int selectedTagIndex = 0;
    private string[] tagOptions = Array.Empty<string>();
    private List<UnityEngine.Object> allTagAssets = new List<UnityEngine.Object>();

    private int selectedRarityIndex = 0;
    private string[] rarityOptions = Array.Empty<string>();
    private List<Rarity> allRarities = new List<Rarity>();

    private int giveAmount = 1;
    private Vector2 scrollPos;
    private Vector2 leftScrollPos;
    private Vector2 rightScrollPos;
    private CraftingManager cachedCraftingManager;

    private Item selectedItem;
    private Editor cachedItemEditor;
    private string creatorItemSearch = "";
    private string newItemName = "NewItem";
    private EItemType newItemType;
    private Rarity newItemRarity;
    private Sprite newItemSprite;
    private string itemFolder = "Assets/Resources/Data/Items";
    private const string ItemFolderPref = "ShopCrafter_ItemFolder";

    private CraftedObjectRecipe selectedRecipe;
    private Editor cachedRecipeEditor;
    private string creatorRecipeSearch = "";
    private string newRecipeName = "NewRecipe";
    private string recipeFolder = "Assets/Resources/Data/Recipes";
    private const string RecipeFolderPref = "ShopCrafter_RecipeFolder";

    [MenuItem("ShopCrafter/Tools/Item Giver")]
    public static void ShowWindow()
    {
        ItemGiverWindow window = GetWindow<ItemGiverWindow>("Item Giver");
        window.minSize = new Vector2(680f, 480f);
    }

    private void OnEnable()
    {
        itemFolder = EditorPrefs.GetString(ItemFolderPref, itemFolder);
        recipeFolder = EditorPrefs.GetString(RecipeFolderPref, recipeFolder);
        RefreshDatabase();
    }

    private void OnFocus()
    {
        RefreshDatabase();
    }

    private void OnDisable()
    {
        ClearEditor(ref cachedItemEditor);
        ClearEditor(ref cachedRecipeEditor);
    }

    private void ClearEditor(ref Editor editor)
    {
        if (editor != null)
        {
            DestroyImmediate(editor);
            editor = null;
        }
    }

    private void RefreshDatabase()
    {
        allItems.Clear();
        foreach (string guid in AssetDatabase.FindAssets("t:Item"))
        {
            Item item = AssetDatabase.LoadAssetAtPath<Item>(AssetDatabase.GUIDToAssetPath(guid));
            if (item != null) allItems.Add(item);
        }

        allItems = allItems
            .OrderBy(i => i.Type)
            .ThenBy(i => string.IsNullOrEmpty(i.ItemName) ? i.name : i.ItemName)
            .ToList();

        allRecipes.Clear();
        foreach (string guid in AssetDatabase.FindAssets("t:CraftedObjectRecipe"))
        {
            CraftedObjectRecipe recipe = AssetDatabase.LoadAssetAtPath<CraftedObjectRecipe>(AssetDatabase.GUIDToAssetPath(guid));
            if (recipe != null) allRecipes.Add(recipe);
        }
        allRecipes = allRecipes.OrderBy(r => r.name).ToList();

        var types = new List<string> { "Tous les types" };
        types.AddRange(Enum.GetNames(typeof(EItemType)));
        typeOptions = types.ToArray();
        selectedTypeIndex = Mathf.Clamp(selectedTypeIndex, 0, typeOptions.Length - 1);

        allTagAssets = allItems
            .Where(i => i.Tags != null)
            .SelectMany(i => i.Tags)
            .Where(t => t != null && t.Asset != null)
            .Select(t => t.Asset)
            .Cast<UnityEngine.Object>()
            .Distinct()
            .OrderBy(a => a.name)
            .ToList();

        var tags = new List<string> { "Tous les tags" };
        tags.AddRange(allTagAssets.Select(a => a.name));
        tagOptions = tags.ToArray();
        selectedTagIndex = Mathf.Clamp(selectedTagIndex, 0, tagOptions.Length - 1);

        allRarities = allItems
            .Where(i => i.RarityInfos != null)
            .Select(i => i.RarityInfos)
            .Distinct()
            .OrderBy(r => r.name)
            .ToList();

        var rarities = new List<string> { "Toutes les raretés" };
        rarities.AddRange(allRarities.Select(r => r.name));
        rarityOptions = rarities.ToArray();
        selectedRarityIndex = Mathf.Clamp(selectedRarityIndex, 0, rarityOptions.Length - 1);
    }

    private CraftingManager GetCraftingManager()
    {
        if (!Application.isPlaying)
        {
            cachedCraftingManager = null;
            return null;
        }

        if (cachedCraftingManager == null)
        {
            cachedCraftingManager = FindFirstObjectByType<CraftingManager>();
        }

        return cachedCraftingManager;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        currentTab = (MainTab)GUILayout.Toolbar(
            (int)currentTab,
            new[] { "Item Giver (Play)", "Créateur (Items & Recipes)" },
            GUILayout.Height(26)
        );
        EditorGUILayout.Space(6);

        if (currentTab == MainTab.ItemGiver)
        {
            DrawItemGiverTab();
        }
        else
        {
            DrawCreatorTab();
        }

        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void DrawItemGiverTab()
    {
        CraftingManager craftingManager = GetCraftingManager();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Lance le Play Mode pour te give des items dans le CraftingManager.", MessageType.Info);
        }
        else if (craftingManager == null)
        {
            EditorGUILayout.HelpBox("Aucun CraftingManager trouvé dans la scène active.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Inventaire actif : {craftingManager.ItemInventory.Count} types d'items", EditorStyles.boldLabel);
            if (GUILayout.Button("Vider inventaire", GUILayout.Width(115), GUILayout.Height(20)))
            {
                craftingManager.ItemInventory.Clear();
                Repaint();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);
        if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(22)))
        {
            searchFilter = "";
            GUI.FocusControl(null);
        }
        if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            RefreshDatabase();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (typeOptions.Length > 0) selectedTypeIndex = EditorGUILayout.Popup(selectedTypeIndex, typeOptions);
        if (tagOptions.Length > 0) selectedTagIndex = EditorGUILayout.Popup(selectedTagIndex, tagOptions);
        if (rarityOptions.Length > 0) selectedRarityIndex = EditorGUILayout.Popup(selectedRarityIndex, rarityOptions);

        if ((selectedTypeIndex > 0 || selectedTagIndex > 0 || selectedRarityIndex > 0) && GUILayout.Button("Reset", GUILayout.Width(48)))
        {
            selectedTypeIndex = 0;
            selectedTagIndex = 0;
            selectedRarityIndex = 0;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Quantité par clic :", GUILayout.Width(110));
        if (GUILayout.Button("1", GUILayout.Width(28))) giveAmount = 1;
        if (GUILayout.Button("5", GUILayout.Width(28))) giveAmount = 5;
        if (GUILayout.Button("10", GUILayout.Width(32))) giveAmount = 10;
        giveAmount = Mathf.Max(1, EditorGUILayout.IntField(giveAmount, GUILayout.Width(45)));

        GUILayout.FlexibleSpace();

        EditorGUI.BeginDisabledGroup(craftingManager == null);
        if (GUILayout.Button($"Give tout (+{giveAmount})", GUILayout.Width(115)))
        {
            foreach (Item item in GetFilteredItems())
            {
                craftingManager.AddItem(item, giveAmount);
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        List<Item> filteredItems = GetFilteredItems();
        if (filteredItems.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucun item trouvé.", MessageType.None);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (Item item in filteredItems)
        {
            DrawItemRow(item, craftingManager);
        }
        EditorGUILayout.EndScrollView();
    }

    private List<Item> GetFilteredItems()
    {
        return allItems.Where(item =>
        {
            if (item == null) return false;

            if (selectedTypeIndex > 0 && item.Type != (EItemType)(selectedTypeIndex - 1))
                return false;

            if (selectedTagIndex > 0 && selectedTagIndex - 1 < allTagAssets.Count)
            {
                UnityEngine.Object targetTag = allTagAssets[selectedTagIndex - 1];
                if (item.Tags == null || !item.Tags.Any(t => t != null && t.Asset == targetTag))
                    return false;
            }

            if (selectedRarityIndex > 0 && selectedRarityIndex - 1 < allRarities.Count)
            {
                if (item.RarityInfos != allRarities[selectedRarityIndex - 1])
                    return false;
            }

            if (!string.IsNullOrEmpty(searchFilter))
            {
                string displayName = string.IsNullOrEmpty(item.ItemName) ? item.name : item.ItemName;
                bool nameMatch = displayName.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                bool tagMatch = item.Tags != null && item.Tags.Any(t =>
                    t.Asset != null && t.Asset.name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0);

                if (!nameMatch && !tagMatch) return false;
            }

            return true;
        }).ToList();
    }

    private void DrawItemRow(Item item, CraftingManager craftingManager)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(40));

        Color rarityColor = item.RarityInfos != null ? item.RarityInfos.RarityColor : Color.gray;
        Rect colorRect = GUILayoutUtility.GetRect(5f, 36f, GUILayout.Width(5f));
        EditorGUI.DrawRect(colorRect, rarityColor);

        Texture2D tex = item.ItemSprite != null
            ? (AssetPreview.GetAssetPreview(item.ItemSprite) ?? item.ItemSprite.texture)
            : null;
        GUILayout.Label(tex, GUILayout.Width(36), GUILayout.Height(36));

        EditorGUILayout.BeginVertical();
        string displayName = string.IsNullOrEmpty(item.ItemName) ? item.name : item.ItemName;
        EditorGUILayout.LabelField(displayName, EditorStyles.boldLabel);

        string tagsSummary = (item.Tags != null && item.Tags.Count > 0)
            ? " • " + string.Join(", ", item.Tags.Where(t => t.Asset != null).Select(t => $"{t.Asset.name} ({t.Amount.BaseValue})"))
            : "";
        EditorGUILayout.LabelField($"{item.Type}{tagsSummary}", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        int owned = 0;
        if (craftingManager != null && craftingManager.ItemInventory.TryGetValue(item, out int count))
        {
            owned = count;
        }

        GUIStyle countStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleRight,
            normal = { textColor = owned > 0 ? new Color(0.45f, 0.9f, 0.55f) : Color.gray }
        };
        EditorGUILayout.LabelField($"x{owned}", countStyle, GUILayout.Width(38), GUILayout.Height(36));

        EditorGUI.BeginDisabledGroup(craftingManager == null);

        if (GUILayout.Button($"-{giveAmount}", GUILayout.Width(36), GUILayout.Height(32)))
            craftingManager.ConsumeItem(item, giveAmount);

        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button($"+{giveAmount}", GUILayout.Width(44), GUILayout.Height(32)))
            craftingManager.AddItem(item, giveAmount);
        GUI.backgroundColor = Color.white;

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("◎", GUILayout.Width(24), GUILayout.Height(32)))
        {
            EditorGUIUtility.PingObject(item);
            Selection.activeObject = item;
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawCreatorTab()
    {
        creatorSubTab = (CreatorSubTab)GUILayout.Toolbar(
            (int)creatorSubTab,
            new[] { $"Items ({allItems.Count})", $"Recipes ({allRecipes.Count})" },
            GUILayout.Height(22)
        );
        EditorGUILayout.Space(6);

        if (creatorSubTab == CreatorSubTab.Items)
        {
            DrawItemCreatorSubTab();
        }
        else
        {
            DrawRecipeCreatorSubTab();
        }
    }

    private void DrawItemCreatorSubTab()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(285f));

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Nouvel Item", EditorStyles.boldLabel);
        newItemName = EditorGUILayout.TextField("Nom", newItemName);
        newItemType = (EItemType)EditorGUILayout.EnumPopup("Type", newItemType);
        newItemRarity = (Rarity)EditorGUILayout.ObjectField("Rareté", newItemRarity, typeof(Rarity), false);
        newItemSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", newItemSprite, typeof(Sprite), false);

        EditorGUILayout.BeginHorizontal();
        itemFolder = EditorGUILayout.TextField("Dossier", itemFolder);
        if (GUILayout.Button("...", GUILayout.Width(26)))
        {
            string picked = EditorUtility.OpenFolderPanel("Dossier des Items", "Assets", "");
            if (!string.IsNullOrEmpty(picked) && picked.StartsWith(Application.dataPath))
            {
                itemFolder = "Assets" + picked.Substring(Application.dataPath.Length);
                EditorPrefs.SetString(ItemFolderPref, itemFolder);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button("+ Créer Item", GUILayout.Height(26)))
        {
            CreateNewItem();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        creatorItemSearch = EditorGUILayout.TextField(creatorItemSearch, EditorStyles.toolbarSearchField);
        if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(22)))
        {
            creatorItemSearch = "";
            GUI.FocusControl(null);
        }
        if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            RefreshDatabase();
        }
        EditorGUILayout.EndHorizontal();

        leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos);
        foreach (Item item in allItems)
        {
            if (item == null) continue;
            string displayName = string.IsNullOrEmpty(item.ItemName) ? item.name : item.ItemName;
            if (!string.IsNullOrEmpty(creatorItemSearch) &&
                displayName.IndexOf(creatorItemSearch, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            bool isSel = selectedItem == item;
            GUI.backgroundColor = isSel ? new Color(0.6f, 0.85f, 1f) : Color.white;
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button(displayName, EditorStyles.label))
            {
                SelectItem(item);
            }

            EditorGUILayout.LabelField(item.Type.ToString(), EditorStyles.miniLabel, GUILayout.Width(65));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (selectedItem == null)
        {
            EditorGUILayout.HelpBox("Sélectionne un Item à gauche ou crées-en un nouveau pour éditer ses propriétés et ses Tags.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedItem.name, EditorStyles.boldLabel);

            CraftingManager cm = GetCraftingManager();
            if (cm != null && GUILayout.Button($"+{giveAmount} Give", GUILayout.Width(72)))
            {
                cm.AddItem(selectedItem, giveAmount);
            }
            if (GUILayout.Button("Dupliquer", GUILayout.Width(70)))
            {
                DuplicateAsset(selectedItem, SelectItem);
            }
            if (GUILayout.Button("Ping", GUILayout.Width(42)))
            {
                EditorGUIUtility.PingObject(selectedItem);
                Selection.activeObject = selectedItem;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos);
            if (cachedItemEditor == null || cachedItemEditor.target != selectedItem)
            {
                ClearEditor(ref cachedItemEditor);
                cachedItemEditor = Editor.CreateEditor(selectedItem);
            }
            cachedItemEditor?.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawRecipeCreatorSubTab()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(285f));

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Nouvelle Recipe", EditorStyles.boldLabel);
        newRecipeName = EditorGUILayout.TextField("Nom", newRecipeName);

        EditorGUILayout.BeginHorizontal();
        recipeFolder = EditorGUILayout.TextField("Dossier", recipeFolder);
        if (GUILayout.Button("...", GUILayout.Width(26)))
        {
            string picked = EditorUtility.OpenFolderPanel("Dossier des Recipes", "Assets", "");
            if (!string.IsNullOrEmpty(picked) && picked.StartsWith(Application.dataPath))
            {
                recipeFolder = "Assets" + picked.Substring(Application.dataPath.Length);
                EditorPrefs.SetString(RecipeFolderPref, recipeFolder);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button("+ Créer Recipe", GUILayout.Height(26)))
        {
            CreateNewRecipe();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        creatorRecipeSearch = EditorGUILayout.TextField(creatorRecipeSearch, EditorStyles.toolbarSearchField);
        if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(22)))
        {
            creatorRecipeSearch = "";
            GUI.FocusControl(null);
        }
        if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            RefreshDatabase();
        }
        EditorGUILayout.EndHorizontal();

        leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos);
        foreach (CraftedObjectRecipe recipe in allRecipes)
        {
            if (recipe == null) continue;
            if (!string.IsNullOrEmpty(creatorRecipeSearch) &&
                recipe.name.IndexOf(creatorRecipeSearch, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            bool isSel = selectedRecipe == recipe;
            GUI.backgroundColor = isSel ? new Color(0.6f, 0.85f, 1f) : Color.white;
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button(recipe.name, EditorStyles.label))
            {
                SelectRecipe(recipe);
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (selectedRecipe == null)
        {
            EditorGUILayout.HelpBox("Sélectionne une Recipe à gauche ou crées-en une nouvelle pour l'éditer.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedRecipe.name, EditorStyles.boldLabel);

            CraftingManager cm = GetCraftingManager();
            if (cm != null && !cm.HasBlueprint(selectedRecipe) && GUILayout.Button("+ Blueprint", GUILayout.Width(82)))
            {
                cm.AddBlueprint(selectedRecipe);
                ShowNotification(new GUIContent("Blueprint débloqué !"), 1f);
            }
            if (GUILayout.Button("Dupliquer", GUILayout.Width(70)))
            {
                DuplicateAsset(selectedRecipe, SelectRecipe);
            }
            if (GUILayout.Button("Ping", GUILayout.Width(42)))
            {
                EditorGUIUtility.PingObject(selectedRecipe);
                Selection.activeObject = selectedRecipe;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos);
            if (cachedRecipeEditor == null || cachedRecipeEditor.target != selectedRecipe)
            {
                ClearEditor(ref cachedRecipeEditor);
                cachedRecipeEditor = Editor.CreateEditor(selectedRecipe);
            }
            cachedRecipeEditor?.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void SelectItem(Item item)
    {
        selectedItem = item;
        ClearEditor(ref cachedItemEditor);
        if (item != null) cachedItemEditor = Editor.CreateEditor(item);
    }

    private void SelectRecipe(CraftedObjectRecipe recipe)
    {
        selectedRecipe = recipe;
        ClearEditor(ref cachedRecipeEditor);
        if (recipe != null) cachedRecipeEditor = Editor.CreateEditor(recipe);
    }

    private void CreateNewItem()
    {
        EnsureFolderExists(itemFolder);
        EditorPrefs.SetString(ItemFolderPref, itemFolder);

        string safeName = string.IsNullOrWhiteSpace(newItemName) ? "NewItem" : newItemName.Trim();
        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(itemFolder, $"Item {safeName}.asset"));

        Item created = CreateInstance<Item>();
        created.ItemName = safeName;
        created.Type = newItemType;
        created.RarityInfos = newItemRarity;
        created.ItemSprite = newItemSprite;
        created.Tags = new List<TagValue>();

        AssetDatabase.CreateAsset(created, path);
        AssetDatabase.SaveAssets();
        RefreshDatabase();
        SelectItem(created);
        ShowNotification(new GUIContent($"Item '{safeName}' créé !"), 1.2f);
    }

    private void CreateNewRecipe()
    {
        EnsureFolderExists(recipeFolder);
        EditorPrefs.SetString(RecipeFolderPref, recipeFolder);

        string safeName = string.IsNullOrWhiteSpace(newRecipeName) ? "NewRecipe" : newRecipeName.Trim();
        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(recipeFolder, $"{safeName}.asset"));

        CraftedObjectRecipe created = CreateInstance<CraftedObjectRecipe>();
        AssetDatabase.CreateAsset(created, path);
        AssetDatabase.SaveAssets();
        RefreshDatabase();
        SelectRecipe(created);
        ShowNotification(new GUIContent($"Recipe '{safeName}' créée !"), 1.2f);
    }

    private void DuplicateAsset<T>(T source, Action<T> onDuplicated) where T : UnityEngine.Object
    {
        string srcPath = AssetDatabase.GetAssetPath(source);
        string dstPath = AssetDatabase.GenerateUniqueAssetPath(srcPath);
        if (AssetDatabase.CopyAsset(srcPath, dstPath))
        {
            AssetDatabase.SaveAssets();
            RefreshDatabase();
            onDuplicated?.Invoke(AssetDatabase.LoadAssetAtPath<T>(dstPath));
        }
    }

    private void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string[] parts = folderPath.Replace("\\", "/").Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }
}