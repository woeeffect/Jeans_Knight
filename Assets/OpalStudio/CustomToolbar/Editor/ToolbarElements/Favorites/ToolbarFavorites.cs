using OpalStudio.CustomToolbar.Editor.Core;
using OpalStudio.CustomToolbar.Editor.ToolbarElements.Favorites.Window;
using UnityEditor;
using UnityEngine;

namespace OpalStudio.CustomToolbar.Editor.ToolbarElements.Favorites
{
      sealed internal class ToolbarFavorites : BaseToolbarElement
      {
            private GUIContent _buttonContent;

            protected override string Name => "Favorites";
            protected override string Tooltip => "Quick access to favorite assets and folders.";

            public override void OnInit()
            {
                  Texture icon = EditorGUIUtility.IconContent("d_Favorite Icon").image;

                  _buttonContent = new GUIContent(icon, this.Tooltip);
            }

            public override void OnDrawInToolbar()
            {
                  using (new EditorGUI.DisabledScope(!this.Enabled))
                  {
                        if (GUILayout.Button(_buttonContent, ToolbarStyles.CommandButtonStyle, GUILayout.Width(this.Width)))
                        {
                              FavoritesWindow.ShowWindow();
                        }
                  }
            }
      }
}
