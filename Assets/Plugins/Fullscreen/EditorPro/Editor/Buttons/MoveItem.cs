using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Fullscreen.EditorPro
{
    public static class MoveItem
    {
        public static void Move(Loader loader, List<ScriptableObject> itemsToMove = null)
        {
            List<ScriptableObject> selectedItems;

            if (itemsToMove != null && itemsToMove.Any())
            {
                selectedItems = itemsToMove;
            }
            else if (loader.listViewScriptableObjectsItems.selectedItems.Any())
            {
                selectedItems = loader.listViewScriptableObjectsItems.selectedItems.OfType<ScriptableObject>().ToList();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "No items selected.", "OK");
                return;
            }

            string oldPath = AssetDatabase.GetAssetPath(selectedItems.First());
            string oldDirectory = System.IO.Path.GetDirectoryName(oldPath);

            string newDirectoryAbsolute = EditorUtility.OpenFolderPanel(
                "Select New Location",
                oldDirectory,
                ""
            );

            if (string.IsNullOrEmpty(newDirectoryAbsolute))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(newDirectoryAbsolute))
            {
                EditorUtility.DisplayDialog("Error", "No location selected.", "OK");
                return;
            }

            string newDirectoryRelative = "Assets" + newDirectoryAbsolute.Substring(Application.dataPath.Length);
            if (!AssetDatabase.IsValidFolder(newDirectoryRelative))
            {
                EditorUtility.DisplayDialog("Error", "Selected location is not a valid Unity folder.", "OK");
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar("Moving Items", "Moving assets...", 0f);

                foreach (var selectedItem in selectedItems)
                {
                    string oldPathItem = AssetDatabase.GetAssetPath(selectedItem);
                    string newPath = System.IO.Path.Combine(newDirectoryRelative, System.IO.Path.GetFileName(oldPathItem));

                    AssetDatabase.MoveAsset(oldPathItem, newPath);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                loader.LoadItems();
            }
            catch (Exception)
            {
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
