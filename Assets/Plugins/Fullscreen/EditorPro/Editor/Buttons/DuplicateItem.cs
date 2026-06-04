using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Fullscreen.EditorPro
{
    public static class DuplicateItem
    {
        public static void Duplicate(Loader loader, List<ScriptableObject> itemsToDuplicate = null)
        {
            List<ScriptableObject> selectedItems;

            if (itemsToDuplicate != null && itemsToDuplicate.Any())
            {
                selectedItems = itemsToDuplicate;
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
                    $"Duplicate {loader.itemType.Name}",
                    selectedItem.name + " - Copy",
                    "asset",
                    "Enter a name for the duplicate item",
                    directory
                );

                if (string.IsNullOrEmpty(newName))
                {
                    return;
                }

                string newPath = AssetDatabase.GenerateUniqueAssetPath(newName);

                try
                {
                    EditorUtility.DisplayProgressBar("Duplicating Items", "Duplicating assets...", 0f);
                    bool success = AssetDatabase.CopyAsset(oldPath, newPath);
                    if (!success)
                    {
                        Debug.LogError($"Failed to duplicate asset: {oldPath}");
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
