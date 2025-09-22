using UnityEngine;

[CreateAssetMenu(fileName = "ParticleEmotionData", menuName = "ShopCrafter/Dialogue/ParticleEmotionData")]
public class ParticleEmotionData : EmotionData
{
    public ParticleSystem EmotionParticlesPrefab;

    public override EmotionBehaviour GetEmotionBehaviour()
    {
        return new ParticleEmotionBehaviour(this);
    }
}

public class ParticleEmotionBehaviour : EmotionBehaviour
{
    protected ParticleEmotionData emotionData;
    protected ParticleSystem psInstance;

    public ParticleEmotionBehaviour(ParticleEmotionData emotionData) : base(emotionData)
    {
        this.emotionData = emotionData;
    }

    public override void ApplyEmotion()
    {
        base.ApplyEmotion();
    }

    public override void RemoveEmotion()
    {
        base.RemoveEmotion();
    }
}
