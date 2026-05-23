using OpalStudio.CustomToolbar.Editor.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace OpalStudio.CustomToolbar.Editor.ToolbarElements
{
      sealed internal class ToolbarSaveProject : BaseToolbarElement
      {
            private GUIContent _buttonContent;

            protected override string Name => "Save Project";
            protected override string Tooltip => "Saves the current scene(s) and all modified assets in the project.";

            public override void OnInit()
            {
                  Texture icon = EditorGUIUtility.IconContent("d_SaveAs").image;
                  _buttonContent = new GUIContent(icon, this.Tooltip);
            }

            public override void OnDrawInToolbar()
            {
                  if (GUILayout.Button(_buttonContent, ToolbarStyles.CommandButtonStyle, GUILayout.Width(this.Width)))
                  {
                        EditorSceneManager.SaveOpenScenes();

                        AssetDatabase.SaveAssets();
                  }
            }
      }
}
