using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Fullscreen.EditorPro
{
    public static class RefreshItems
    {
        public static void Refresh(Loader loader)
        {
            List<ScriptableObject> previouslySelectedItems = new List<ScriptableObject>();
            if (loader.listViewScriptableObjectsItems != null)
            {
                previouslySelectedItems = loader.listViewScriptableObjectsItems.selectedItems
                    .OfType<ScriptableObject>()
                    .ToList();
            }

            loader.LoadItems();

            if (loader.itemDetailsScrollView.childCount == 0)
            {
                loader.lockButton.style.display = DisplayStyle.None;
            }
            else
            {
                loader.UpdateButtonStates(null);
                loader.selectedScriptableObjects.Clear();
                loader.selectedGridItems.Clear();
                loader.openItems.Clear();
                loader.itemDetailsScrollView.Clear();
            }
        }
    }
}

