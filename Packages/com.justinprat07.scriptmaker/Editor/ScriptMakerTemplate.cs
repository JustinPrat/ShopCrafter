using UnityEngine;

namespace ScriptMaker.Editor
{
    [CreateAssetMenu(fileName = "ScriptMakerTemplate", menuName = "ScriptMaker/ScriptMakerTemplate")]
    public class ScriptMakerTemplate : ScriptableObject
    {
        [TextArea(10, 20), SerializeField, Tooltip("Utiliser {ScriptName}")]
        private string scriptBody;

        [SerializeField]
        private string scriptNameSuffix;

        public string ScriptNameSuffix => scriptNameSuffix;

        public string GetScriptTemplate(string scriptName)
        {
            return scriptBody.Replace("{ScriptName}", scriptName);
        }
    }
}