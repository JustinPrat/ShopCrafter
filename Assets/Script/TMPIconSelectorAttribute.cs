using System;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TMPIconSelectorAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TMPIconSelectorAttribute))]
public class TMPIconSelectorDrawer : PropertyDrawer
{
    private bool showPicker = false;
    private string searchFilter = "";
    private Vector2 scrollPos;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = EditorGUIUtility.singleLineHeight + 4f;
        return showPicker ? baseHeight + 245f : baseHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect fieldRect = EditorGUI.PrefixLabel(headerRect, label);

        string buttonLabel = string.IsNullOrEmpty(property.stringValue)
            ? "Choisir une icône ▼"
            : $"{property.stringValue}  {(showPicker ? "▲" : "▼")}";

        if (GUI.Button(fieldRect, buttonLabel, EditorStyles.popup))
        {
            showPicker = !showPicker;
        }

        if (!showPicker) return;

        TMP_SpriteAsset spriteAsset = Resources.Load<TMP_SpriteAsset>("Data/TMPIconData/TextIcons");
        if (spriteAsset == null)
        {
            Rect warnRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 4f, position.width, 36f);
            EditorGUI.HelpBox(warnRect, "TextIcons introuvable dans Resources/Data/TMPIconData/", MessageType.Warning);
            return;
        }

        float yOffset = position.y + EditorGUIUtility.singleLineHeight + 6f;

        // Barre de recherche
        Rect searchRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
        searchFilter = EditorGUI.TextField(searchRect, "Rechercher", searchFilter);
        yOffset += EditorGUIUtility.singleLineHeight + 4f;

        // Zone scrollable
        Rect scrollOuter = new Rect(position.x, yOffset, position.width, 215f);
        GUI.Box(scrollOuter, GUIContent.none, EditorStyles.helpBox);

        // Filtrage des icônes
        var filtered = new System.Collections.Generic.List<TMP_SpriteCharacter>();
        foreach (TMP_SpriteCharacter c in spriteAsset.spriteCharacterTable)
        {
            if (string.IsNullOrEmpty(searchFilter) ||
                c.name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered.Add(c);
            }
        }

        float iconSize = 46f;
        float spacing = 4f;
        int columns = Mathf.Max(1, Mathf.FloorToInt((position.width - 20f) / (iconSize + spacing)));
        int rows = Mathf.CeilToInt((float)filtered.Count / columns);
        float contentHeight = rows * (iconSize + spacing) + spacing;

        Rect scrollInner = new Rect(0, 0, position.width - 20f, contentHeight);
        scrollPos = GUI.BeginScrollView(scrollOuter, scrollPos, scrollInner);

        for (int i = 0; i < filtered.Count; i++)
        {
            TMP_SpriteCharacter character = filtered[i];
            int col = i % columns;
            int row = i / columns;

            Rect btnRect = new Rect(
                spacing + col * (iconSize + spacing),
                spacing + row * (iconSize + spacing),
                iconSize,
                iconSize
            );

            Sprite sp = spriteAsset.spriteGlyphTable[(int)character.glyphIndex].sprite;
            Texture2D tex = AssetPreview.GetAssetPreview(sp) ?? (sp != null ? sp.texture : null);

            GUI.backgroundColor = (character.name == property.stringValue) ? Color.cyan : Color.white;
            if (GUI.Button(btnRect, new GUIContent(tex, character.name)))
            {
                property.stringValue = character.name;
                property.serializedObject.ApplyModifiedProperties();

                // Met aussi à jour le champ "preview" si présent sur l'objet
                SerializedProperty previewProp = property.serializedObject.FindProperty("preview");
                if (previewProp != null)
                {
                    previewProp.objectReferenceValue = sp;
                    property.serializedObject.ApplyModifiedProperties();
                }

                showPicker = false;
            }
        }

        GUI.backgroundColor = Color.white;
        GUI.EndScrollView();
    }
}
#endif