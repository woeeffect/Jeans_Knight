using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fullscreen.EditorPro
{
    public static class DeleteItem
    {
        public static void Delete(Loader loader, List<ScriptableObject> itemsToDelete = null)
        {
            List<ScriptableObject> toDelete;

            if (itemsToDelete != null && itemsToDelete.Count > 0)
            {
                toDelete = itemsToDelete;
            }
            else if (loader.listViewScriptableObjectsItems?.selectedItems != null &&
                     loader.listViewScriptableObjectsItems.selectedItems.Any())
            {
                toDelete = loader.listViewScriptableObjectsItems.selectedItems
                    .OfType<ScriptableObject>()
                    .ToList();
            }
            else if (loader.selectedScriptableObjects.Count > 0)
            {
                toDelete = new List<ScriptableObject>(loader.selectedScriptableObjects);
            }
            else
            {
                EditorUtility.DisplayDialog("Nothing Selected", "Please select one or more items to delete.", "OK");
                return;
            }

            if (toDelete.Count == 0)
            {
                EditorUtility.DisplayDialog("Nothing to Delete", "No valid assets selected.", "OK");
                return;
            }

            var names = string.Join("\n• ", toDelete.Select(so => so.name));
            var message = toDelete.Count == 1
                ? $"Are you sure you want to delete:\n\n• {names}"
                : $"Are you sure you want to delete {toDelete.Count} items?\n\n• {names}";

            if (!EditorUtility.DisplayDialog("Confirm Delete", message, "Delete", "Cancel"))
                return;


            try
            {
                EditorUtility.DisplayProgressBar("Deleting Assets", "Removing files from project...", 0.5f);

                foreach (var item in toDelete)
                {
                    var path = AssetDatabase.GetAssetPath(item);
                    if (!string.IsNullOrEmpty(path)) AssetDatabase.DeleteAsset(path);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                loader.selectedScriptableObjects.Clear();
                loader.selectedGridItems.Clear();
                loader.openItems.Clear();
                loader.itemDetailsScrollView.Clear();

                loader.LoadItems();

                void ForceRebuild()
                {
                    loader.OnItemsLoaded -= ForceRebuild;
                    loader.RebuildCurrentView();
                    loader.UpdateButtonStates(null);
                }

                loader.OnItemsLoaded += ForceRebuild;

                EditorApplication.delayCall += () =>
                {
                    if (loader != null)
                    {
                        loader.RebuildCurrentView();
                        loader.UpdateButtonStates(null);
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete assets: {ex}");
                EditorUtility.DisplayDialog("Error", $"Failed to delete some assets.\n\n{ex.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}