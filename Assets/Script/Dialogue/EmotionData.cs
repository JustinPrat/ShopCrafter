using UnityEngine;

public abstract class EmotionData : ScriptableObject
{
    [SerializeField]
    protected EmotionType emotionType;

    public virtual EmotionBehaviour GetEmotionBehaviour()
    {
        return new EmotionBehaviour(this);
    }
}

public enum EmotionType
{
    Normal,
    Questionning
}

public class EmotionBehaviour
{
    public EmotionBehaviour(EmotionData data)
    {
    }

    public virtual void ApplyEmotion()
    {

    }

    public virtual void RemoveEmotion()
    {

    }
}
