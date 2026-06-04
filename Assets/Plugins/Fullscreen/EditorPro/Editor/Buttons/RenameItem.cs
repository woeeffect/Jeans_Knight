using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Fullscreen.EditorPro
{
    public static class RenameItem
    {
        public static void Rename(Loader loader, List<ScriptableObject> selectedItems = null)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                if (loader.listViewScriptableObjectsItems.selectedItem != null)
                {
                    selectedItems = new List<ScriptableObject>
                    {
                        (ScriptableObject)loader.listViewScriptableObjectsItems.selectedItem
                    };
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "No item selected.", "OK");
                    return;
                }
            }

            foreach (var selectedItem in selectedItems)
            {
                string oldPath = AssetDatabase.GetAssetPath(selectedItem);
                if (string.IsNullOrEmpty(oldPath))
                {
                    Debug.LogWarning($"Could not find path for {selectedItem.name}.");
                    continue;
                }

                string directory = System.IO.Path.GetDirectoryName(oldPath);

                string newName = EditorUtility.SaveFilePanelInProject(
                    $"Rename {selectedItem.name}",
                    $"{selectedItem.name}",
                    "asset",
                    $"Enter new name for {selectedItem.name}:",
                    directory
                );

                if (string.IsNullOrEmpty(newName))
                {
                    continue;
                }

                string newFileName = System.IO.Path.GetFileNameWithoutExtension(newName);

                try
                {
                    EditorUtility.DisplayProgressBar("Renaming Item", $"Renaming {selectedItem.name}...", 0f);
                    string result = AssetDatabase.RenameAsset(oldPath, newFileName);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Debug.LogError($"Failed to rename asset: {result}");
                        continue;
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
}
