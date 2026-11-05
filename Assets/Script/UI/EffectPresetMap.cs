using Coffee.UIEffects;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectPresetMap", menuName = "ShopCrafter/EffectPresetMap")]
public class EffectPresetMap : ScriptableObject
{
    [SerializeField]
    private Dictionary<PresetType, PresetVariants> effectPresets;

    public Dictionary<PresetType, PresetVariants> EffectPresets => effectPresets;
}

[Serializable]
public class PresetVariants
{
    public UIEffectPreset Normal;
    public UIEffectPreset Small;
}

public enum PresetType
{
    RewardElementBase,
    RewardElementBlueprint,
    Unknown
}
