using UnityEngine;

[CreateAssetMenu(fileName = "ScriptMakerTemplate", menuName = "ScriptMaker/ScriptMakerTemplate")]
public class ScriptMakerTemplate : ScriptableObject
{
    [TextArea, SerializeField, Tooltip("Utiliser {ScriptName}")]
    private string scriptBody;

    public string GetScriptTemplate(string scriptName)
    {
        return scriptBody.Replace("{ScriptName}", scriptName);
    }
}
