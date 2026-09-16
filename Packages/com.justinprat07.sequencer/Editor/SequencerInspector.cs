using Sequencer.Actions;
using System;
using System.IO;
using System.Linq;
using TriInspector.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sequencer.Editor
{
    [CustomEditor(typeof(Sequencer), true)]
    public class SequencerInspector : TriEditor
    {
        private static string _saveFolder = "Assets/Resources/Data/Sequencer";
        private static string _customFileName = "";
        private static int _selectedTypeIndex;

        private Type[] _availableTypes;
        private SequenceActionData _tempActionInstance;
        private VisualElement _actionPropertiesContainer;

        protected override void OnEnable()
        {
            base.OnEnable();

            _availableTypes = TypeCache.GetTypesDerivedFrom<SequenceActionData>()
                .Where(t => !t.IsAbstract)
                .ToArray();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_tempActionInstance != null)
                DestroyImmediate(_tempActionInstance);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // 1. Inspecteur standard TriInspector
            var triInspectorElement = base.CreateInspectorGUI();
            if (triInspectorElement != null)
                root.Add(triInspectorElement);

            if (_availableTypes == null || _availableTypes.Length == 0)
                return root;

            // 2. Boîte Action Factory (UI Toolkit pure)
            var factoryBox = new VisualElement();
            factoryBox.style.marginTop = 15;
            factoryBox.style.paddingTop = 8;
            factoryBox.style.paddingBottom = 8;
            factoryBox.style.paddingLeft = 8;
            factoryBox.style.paddingRight = 8;
            factoryBox.style.backgroundColor = new Color(0, 0, 0, 0.15f);
            factoryBox.style.borderTopLeftRadius = 4;
            factoryBox.style.borderTopRightRadius = 4;
            factoryBox.style.borderBottomLeftRadius = 4;
            factoryBox.style.borderBottomRightRadius = 4;

            var title = new Label("Action Factory");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 6;
            factoryBox.Add(title);

            // Sélecteur de dossier
            var folderRow = new VisualElement();
            folderRow.style.flexDirection = FlexDirection.Row;

            var folderField = new TextField("Save Folder") { value = _saveFolder };
            folderField.style.flexGrow = 1;
            folderField.RegisterValueChangedCallback(evt => _saveFolder = evt.newValue);
            folderRow.Add(folderField);

            var browseBtn = new Button(() =>
            {
                string absPath = Path.Combine(Application.dataPath, _saveFolder.Replace("Assets/", ""));
                string chosen = EditorUtility.OpenFolderPanel("Select Save Folder", absPath, "");
                if (!string.IsNullOrEmpty(chosen))
                {
                    if (chosen.StartsWith(Application.dataPath))
                    {
                        _saveFolder = "Assets" + chosen.Substring(Application.dataPath.Length);
                        folderField.value = _saveFolder;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Erreur", "Le dossier doit être dans Assets.", "OK");
                    }
                }
            })
            { text = "Browse..." };
            browseBtn.style.width = 75;
            folderRow.Add(browseBtn);
            factoryBox.Add(folderRow);

            // Dropdown de type d'action
            var typeDropdown = new PopupField<Type>(
                "Action Type",
                _availableTypes.ToList(),
                _selectedTypeIndex,
                t => t.Name,
                t => t.Name
            );

            // Nom de l'asset
            var nameField = new TextField("Asset Name") { value = _customFileName };
            nameField.RegisterValueChangedCallback(evt => _customFileName = evt.newValue);
            factoryBox.Add(typeDropdown);
            factoryBox.Add(nameField);

            // Conteneur dynamique pour les propriétés de l'action
            _actionPropertiesContainer = new VisualElement();
            _actionPropertiesContainer.style.marginTop = 6;
            factoryBox.Add(_actionPropertiesContainer);

            // Callback lors du changement de type
            typeDropdown.RegisterValueChangedCallback(evt =>
            {
                _selectedTypeIndex = _availableTypes.ToList().IndexOf(evt.newValue);
                UpdateActionPreview(evt.newValue, nameField);
            });

            // Initialisation initiale
            if (_tempActionInstance == null && _availableTypes.Length > 0)
            {
                UpdateActionPreview(_availableTypes[_selectedTypeIndex], nameField);
            }

            // Bouton de sauvegarde
            var saveBtn = new Button(SaveAndAddAction) { text = "Save Asset & Add To Sequence" };
            saveBtn.style.height = 28;
            saveBtn.style.marginTop = 8;
            factoryBox.Add(saveBtn);

            root.Add(factoryBox);
            return root;
        }

        private void UpdateActionPreview(Type targetType, TextField nameField)
        {
            if (_tempActionInstance != null)
                DestroyImmediate(_tempActionInstance);

            _tempActionInstance = (SequenceActionData)ScriptableObject.CreateInstance(targetType);
            _customFileName = $"New_{targetType.Name}";
            nameField.value = _customFileName;

            _actionPropertiesContainer.Clear();

            // Génère l'inspecteur natif UI Toolkit sans aucun bug de coordonnées de souris
            var serializedObj = new SerializedObject(_tempActionInstance);
            var inspectorElement = new InspectorElement(serializedObj);
            _actionPropertiesContainer.Add(inspectorElement);
        }

        private void SaveAndAddAction()
        {
            if (_tempActionInstance == null) return;

            if (!Directory.Exists(_saveFolder))
            {
                Directory.CreateDirectory(_saveFolder);
                AssetDatabase.Refresh();
            }

            string fileName = string.IsNullOrWhiteSpace(_customFileName) ? _tempActionInstance.GetType().Name : _customFileName;
            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{_saveFolder}/{fileName}.asset");

            AssetDatabase.CreateAsset(_tempActionInstance, assetPath);
            AssetDatabase.SaveAssets();

            var sequencer = (Sequencer)target;
            Undo.RecordObject(sequencer, "Add Action To Sequencer");

            sequencer.Actions.Add(new Sequencer.ActionScriptable { ActionData = _tempActionInstance });
            EditorUtility.SetDirty(sequencer);

            EditorGUIUtility.PingObject(_tempActionInstance);

            _tempActionInstance = null;
            if (_availableTypes.Length > 0)
                UpdateActionPreview(_availableTypes[_selectedTypeIndex], new TextField());
        }
    }
}