using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace Fullscreen.EditorPro
{
    public static class ListItem
    {
        public static VisualElement MakeItem()
        {
            var itemContainer = new VisualElement();
            itemContainer.style.flexDirection = FlexDirection.Row;
            itemContainer.style.alignItems = Align.Center;
            var icon = new Image
            {
                name = "Icon",
                style =
                {
                    width = 32,
                    height = 32,
                    flexShrink = 0
                }
            };
            itemContainer.Add(icon);

            var label = new Label { name = "Title" };
            itemContainer.Add(label);

            return itemContainer;
        }

        public static void OnItemSelected(IEnumerable<object> incomingItems, Loader loader)
        {
            List<ScriptableObject> newSelections = incomingItems.OfType<ScriptableObject>().ToList();
            bool isCtrlHeld = Event.current != null && (Event.current.control || Event.current.command);

            if (newSelections.Any())
            {
                if (!loader.isLocked)
                {
                    loader.itemDetailsScrollView.Clear();
                    loader.openItems.Clear();
                    loader.selectedScriptableObjects.Clear();
                    foreach (var item in newSelections)
                    {
                        loader.selectedScriptableObjects.Add(item);
                        loader.ShowScriptableObjectItem(item);
                    }
                }
                else
                {
                    foreach (var item in newSelections)
                    {
                        if (loader.selectedScriptableObjects.Contains(item))
                        {
                            loader.selectedScriptableObjects.Remove(item);
                            loader.itemDetailsScrollView.Clear();
                            foreach (var remainingItem in loader.selectedScriptableObjects)
                            {
                                loader.ShowScriptableObjectItem(remainingItem);
                            }
                        }
                        else
                        {
                            loader.selectedScriptableObjects.Add(item);
                            loader.ShowScriptableObjectItem(item);
                        }
                    }
                }

                if (Event.current != null && Event.current.clickCount == 2)
                {
                    foreach (var item in newSelections)
                    {
                        loader.PingObjectInEditor(item);
                    }
                }

                loader.UpdateButtonStates(newSelections.Any() ? loader.selectedScriptableObjects : null);
            }
            else
            {
                if (!loader.isLocked)
                {
                    loader.itemDetailsScrollView.Clear();
                    loader.openItems.Clear();
                    loader.selectedScriptableObjects.Clear();
                }
                loader.UpdateButtonStates(null);
            }
        }
    }
}