using TMPro;
using UnityEditor;
using UnityEngine;

public static class TMPSpriteAssetTools
{
    // Ajoute une option lors du clic droit sur un TMP_SpriteAsset
    [MenuItem("CONTEXT/TMP_SpriteAsset/Copier les métriques du 1er Sprite sur TOUS")]
    public static void CopyFirstGlyphMetricsToAll(MenuCommand command)
    {
        // On récupère le SpriteAsset sur lequel on a cliqué
        TMP_SpriteAsset spriteAsset = (TMP_SpriteAsset)command.context;

        if (spriteAsset == null || spriteAsset.spriteGlyphTable == null || spriteAsset.spriteGlyphTable.Count <= 1)
        {
            Debug.LogWarning("Le Sprite Asset est vide ou ne contient qu'un seul sprite.");
            return;
        }

        // Permet de faire un "Ctrl+Z" si on se trompe
        Undo.RecordObject(spriteAsset, "Copie métriques TMP Sprites");

        // On récupère les réglages du TOUT PREMIER sprite (Index 0)
        var referenceMetrics = spriteAsset.spriteGlyphTable[0].metrics;
        var referenceScale = spriteAsset.spriteCharacterTable[0].scale;

        // On applique ces réglages à tous les autres glyphs
        for (int i = 1; i < spriteAsset.spriteGlyphTable.Count; i++)
        {
            var glyph = spriteAsset.spriteGlyphTable[i];

            // On écrase les métriques (BX, BY, Width, Height, Advance)
            glyph.metrics = new UnityEngine.TextCore.GlyphMetrics(
                referenceMetrics.width,
                referenceMetrics.height,
                referenceMetrics.horizontalBearingX,
                referenceMetrics.horizontalBearingY,
                referenceMetrics.horizontalAdvance
            );
        }

        // On applique aussi le Scale à tous les caractères
        for (int i = 1; i < spriteAsset.spriteCharacterTable.Count; i++)
        {
            spriteAsset.spriteCharacterTable[i].scale = referenceScale;
        }

        // On dit à Unity que le fichier a été modifié pour qu'il le sauvegarde
        EditorUtility.SetDirty(spriteAsset);
        AssetDatabase.SaveAssets();

        Debug.Log($"<color=green>Succès !</color> Les métriques ont été copiées sur {spriteAsset.spriteGlyphTable.Count - 1} sprites.");
    }
}
