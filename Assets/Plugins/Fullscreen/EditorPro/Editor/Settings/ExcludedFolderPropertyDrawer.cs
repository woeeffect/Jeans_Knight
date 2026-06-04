using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fullscreen.EditorPro
{
    [CustomPropertyDrawer(typeof(ExcludedFolders))]
    public class ExcludedFoldersDrawer : PropertyDrawer
    {
        private const string NAME_BUTTON_ADD = "GC-EditorPro-Folders-Foot-Add";
        private const string CLASS_FOLDERS_HEAD = "GC-EditorPro-Folders-Head";
        private const string CLASS_FOLDERS_BODY = "GC-EditorPro-Folders-Body";
        private const string CLASS_FOLDERS_FOOT = "GC-EditorPro-Folders-Foot";
        private const string CLASS_FOLDER_ITEM = "GC-EditorPro-Folder-Item";

        private static readonly IIcon ICON_ADD = new IconFolderSolid(ColorTheme.Type.TextLight);

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var foldersProperty = property.FindPropertyRelative("m_Folders");

            var header = new Label(property.displayName);
            header.AddToClassList(CLASS_FOLDERS_HEAD);
            header.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);
            header.style.marginBottom = 4;
            header.style.paddingLeft = 2;
            header.style.paddingRight = 2;
            header.style.paddingTop = 2;
            header.style.paddingBottom = 2;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;

            var body = new VisualElement();
            body.AddToClassList(CLASS_FOLDERS_BODY);
            body.style.paddingLeft = 10;
            body.style.marginBottom = 2;

            var foot = new VisualElement();
            foot.AddToClassList(CLASS_FOLDERS_FOOT);
            foot.style.paddingTop = 5;
            foot.style.paddingLeft = 10;

            var buttonAdd = new Button { name = NAME_BUTTON_ADD };
            buttonAdd.Add(new Image { image = ICON_ADD.Texture, style = { width = 16, height = 16, marginRight = 4 } });
            buttonAdd.Add(new Label { text = "Add Folder..." });
            buttonAdd.style.flexDirection = FlexDirection.Row;
            buttonAdd.style.alignItems = Align.Center;
            buttonAdd.style.borderLeftWidth = 1;
            buttonAdd.style.borderRightWidth = 1;
            buttonAdd.style.borderTopWidth = 1;
            buttonAdd.style.borderBottomWidth = 1;
            buttonAdd.style.borderLeftColor = new Color(0.3f, 0.3f, 0.3f);
            buttonAdd.style.borderRightColor = new Color(0.3f, 0.3f, 0.3f);
            buttonAdd.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f);
            buttonAdd.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);
            buttonAdd.style.width = 200;
            buttonAdd.style.height = 24;

            foot.Add(buttonAdd);
            root.Add(header);
            root.Add(body);
            root.Add(new SpaceSmaller());
            root.Add(foot);

            void RefreshBody()
            {
                body.Clear();
                for (var i = 0; i < foldersProperty.arraySize; i++)
                {
                    var index = i;
                    var folderPathProperty = foldersProperty.GetArrayElementAtIndex(i);

                    var item = new VisualElement();
                    item.AddToClassList(CLASS_FOLDER_ITEM);
                    item.style.flexDirection = FlexDirection.Row;
                    item.style.alignItems = Align.Center;
                    item.style.marginBottom = 2;

                    var pathField = new PropertyField(folderPathProperty) { label = $"Folder {i + 1}" };
                    pathField.BindProperty(folderPathProperty);
                    pathField.style.flexGrow = 1;

                    var removeButton = new Button { text = "-" };
                    removeButton.style.width = 30;
                    removeButton.style.marginLeft = 5;
                    removeButton.style.height = 18;

                    item.Add(pathField);
                    item.Add(removeButton);
                    body.Add(item);

                    removeButton.clicked += () =>
                    {
                        foldersProperty.DeleteArrayElementAtIndex(index);
                        foldersProperty.serializedObject.ApplyModifiedProperties();
                        RefreshBody();
                        AssetDatabase.Refresh();
                        EditorUtility.RequestScriptReload();
                    };
                }
            }

            buttonAdd.clicked += () =>
            {
                var defaultPath = "Assets";
                var selectedPath = EditorUtility.OpenFolderPanel("Select Folder to Exclude", defaultPath, "");

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
                    if (selectedPath.StartsWith(projectPath))
                    {
                        var relativePath = selectedPath.Substring(projectPath.Length);
                        if (!relativePath.EndsWith("/")) relativePath += "/";

                        foldersProperty.arraySize++;
                        var newElement = foldersProperty.GetArrayElementAtIndex(foldersProperty.arraySize - 1);
                        newElement.stringValue = relativePath;
                        foldersProperty.serializedObject.ApplyModifiedProperties();

                        RefreshBody();
                        AssetDatabase.Refresh();
                        EditorUtility.RequestScriptReload();
                    }
                    else
                    {
                        Debug.LogWarning("Selected folder must be within the Unity project directory.");
                    }
                }
            };

            body.style.display = DisplayStyle.Flex;
            foot.style.display = DisplayStyle.Flex;
            RefreshBody();

            return root;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -1;
        }
    }
}