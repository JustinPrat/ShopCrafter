using UnityEngine;

[CreateAssetMenu(fileName = "EmotionData", menuName = "ShopCrafter/Dialogue/EmotionData")]
public class EmotionData : ScriptableObject
{
    public EmotionType EmotionType;

    public GameObject EmotionPrefab;

    //public virtual EmotionBehaviour GetEmotionBehaviour()
    //{
    //    return new EmotionBehaviour(this);
    //}
}

public enum EmotionType
{
    Normal,
    Questionning
}

//public class EmotionBehaviour
//{
//    public EmotionBehaviour(EmotionData data)
//    {
//    }

//    public virtual void ApplyEmotion()
//    {

//    }

//    public virtual void RemoveEmotion()
//    {

//    }
//}
