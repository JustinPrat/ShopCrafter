using Coffee.UIEffects;
using UnityEngine;

public class EffectPresetMap : ScriptableObject
{

}

public class PresetVariants
{
    public UIEffectPreset Normal;
    public UIEffectPreset Small;
}

public enum PresetType
{
    RewardElementBase,
    RewardElementBlueprint
}
