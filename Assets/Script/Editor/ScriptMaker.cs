using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptMaker : EditorWindow
{
    private string scriptName = "";
    private string path = "Assets/Script/";

    private ScriptMakerTemplate template;

    [MenuItem("Window/ScriptMaker")]
    public static void ShowWindow()
    {
        GetWindow<ScriptMaker>("ScriptMaker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Générateur de script", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scriptName = EditorGUILayout.TextField("Nom du Script", scriptName);
        EditorGUILayout.Space();

        template = (ScriptMakerTemplate) EditorGUILayout.ObjectField("Template", template, typeof(ScriptMakerTemplate), false);

        if (path.StartsWith(Application.dataPath))
        {
            path = "Assets" + path.Substring(Application.dataPath.Length);
        }

        path = EditorGUILayout.TextField("Path", path);
        if (GUILayout.Button("ChangePath", GUILayout.Height(20)))
        {
            path = EditorUtility.OpenFolderPanel("Change Path", "Assets/Script/", "");
        }

        if (GUILayout.Button("MakeScript", GUILayout.Height(20)))
        {
            GenerateScript(scriptName, path, template.GetScriptTemplate(scriptName));
        }
    }

    private void GenerateScript(string name, string path, string template, bool refresh = true)
    {
        string className = name.Replace(" ", "");

        if (!Directory.Exists(path))
        {
            return;
        }

        string fullPath = Path.Combine(path, className + ".cs");

        if (File.Exists(fullPath))
        {
            return;
        }

        File.WriteAllText(fullPath, template);

        if (refresh)
            AssetDatabase.Refresh();
    }
}
