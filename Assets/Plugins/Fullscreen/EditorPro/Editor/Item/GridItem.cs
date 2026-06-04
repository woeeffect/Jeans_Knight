using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fullscreen.EditorPro
{
    public class GridItem
    {
        private static Color GetBackgroundColor(bool hovered = false, bool transparent = true)
        {
            var dark = EditorGUIUtility.isProSkin;
            if (hovered) return dark ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.92f, 0.92f, 0.92f);
            if (transparent) return dark ? new Color(0.2f, 0.2f, 0.2f, 0.3f) : new Color(0.87f, 0.87f, 0.87f);
            return dark ? new Color(0.2f, 0.2f, 0.2f) : new Color(.9f, 0.9f, 0.9f);
        }

        public static Color GetSelectedColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.173f, 0.365f, 0.529f) : new Color(0.45f, 0.65f, 0.9f);
        }

        private static Color GetLabelColor()
        {
            return EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        private static Color GetTooltipBackground()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.1f, 0.1f, 0.1f, 0.9f) : new Color(1f, 1f, 1f, 0.95f);
        }

        private static Color GetTooltipTextColor()
        {
            return EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        private static Color GetTooltipPathColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.35f, 0.35f, 0.35f);
        }

        public static void UpdateGridItemsVisibility(VisualElement gridContainer,
            List<ScriptableObject> listScriptableObjectsItemsFiltered, VisualElement overlay, Loader loader)
        {
            var children = gridContainer.Children().ToList();
            var addButton = children.FirstOrDefault(child => child.Q<Label>()?.text == "+");
            if (addButton != null)
                addButton.style.display = DisplayStyle.Flex;

            var filteredNames = listScriptableObjectsItemsFiltered.Select(i => i.name).ToHashSet();

            foreach (var gridItem in children)
            {
                var label = gridItem.Q<Label>();
                if (label == null || label.text == "+") continue;

                if (filteredNames.Contains(label.text))
                {
                    gridItem.style.display = DisplayStyle.Flex;
                    var item = listScriptableObjectsItemsFiltered.FirstOrDefault(i => i.name == label.text);
                    if (item != null) BindGridItem.Bind(gridItem, item);
                    gridItem.userData = item;
                }
                else
                {
                    gridItem.style.display = DisplayStyle.None;
                }
            }

            var existingNames = children
                .Select(child => child.Q<Label>()?.text)
                .Where(text => !string.IsNullOrEmpty(text))
                .ToHashSet();

            foreach (var item in listScriptableObjectsItemsFiltered)
                if (!existingNames.Contains(item.name))
                {
                    var tooltipHandler = new GridItemTooltip(overlay);
                    var newGridItem = CreateGridItem(item, tooltipHandler, loader);
                    gridContainer.Add(newGridItem);
                    BindGridItem.Bind(newGridItem, item);
                }
        }

        public static void UpdateVisibleGridIcons(VisualElement gridContainer,
            List<ScriptableObject> listScriptableObjectsItemsFiltered)
        {
            if (gridContainer == null || !(gridContainer.parent is ScrollView scrollView)) return;

            var scrollViewHeight = scrollView.resolvedStyle.height;
            var scrollOffset = scrollView.verticalScroller.value;

            foreach (var gridItem in gridContainer.Children())
            {
                var label = gridItem.Q<Label>();
                if (label == null) continue;

                var item = listScriptableObjectsItemsFiltered.FirstOrDefault(i => i.name == label.text);
                if (item == null) continue;

                var itemTop = gridItem.layout.y;
                var itemBottom = itemTop + gridItem.layout.height;

                if (itemBottom >= scrollOffset && itemTop <= scrollOffset + scrollViewHeight)
                    if (gridItem.userData as ScriptableObject != item)
                    {
                        gridItem.userData = item;
                        BindGridItem.Bind(gridItem, item);
                    }
            }
        }

        public static VisualElement CreateAddButton(Loader loader)
        {
            var addButton = new VisualElement
            {
                style =
                {
                    width = 100,
                    height = 85,
                    marginRight = 5,
                    marginBottom = 5,
                    backgroundColor = GetBackgroundColor(),
                    borderTopLeftRadius = 5,
                    borderTopRightRadius = 5,
                    borderBottomLeftRadius = 5,
                    borderBottomRightRadius = 5,
                    justifyContent = Justify.Center,
                    alignItems = Align.Center,
                    flexDirection = FlexDirection.Column,
                    display = DisplayStyle.Flex
                }
            };

            var labelContainer = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    justifyContent = Justify.Center,
                    alignItems = Align.Center,
                    display = DisplayStyle.Flex
                }
            };

            var label = new Label("+")
            {
                style =
                {
                    fontSize = 30,
                    color = GetLabelColor(),
                    unityTextAlign = TextAnchor.MiddleCenter,
                    width = 100,
                    height = 85,
                    flexGrow = 0,
                    alignSelf = Align.Center
                }
            };

            labelContainer.Add(label);
            addButton.Add(labelContainer);

            addButton.RegisterCallback<ClickEvent>(evt => CreateNewItem.Create(loader));
            addButton.RegisterCallback<PointerEnterEvent>(evt =>
                addButton.style.backgroundColor = GetBackgroundColor(true, false));
            addButton.RegisterCallback<PointerLeaveEvent>(evt =>
                addButton.style.backgroundColor = GetBackgroundColor());
            return addButton;
        }

        public static VisualElement CreateGridItem(ScriptableObject item, GridItemTooltip tooltipHandler, Loader loader)
        {
            var gridItem = new VisualElement
            {
                style =
                {
                    width = 100,
                    height = 85,
                    marginRight = 5,
                    marginBottom = 5,
                    backgroundColor = GetBackgroundColor(),
                    borderTopLeftRadius = 5,
                    borderTopRightRadius = 5,
                    borderBottomLeftRadius = 5,
                    borderBottomRightRadius = 5,
                    position = Position.Relative
                }
            };

            var icon = new Image
            {
                name = "Icon",
                style =
                {
                    width = 60,
                    height = 60,
                    position = Position.Absolute,
                    top = 3,
                    left = 20,
                    backgroundImage = null
                }
            };

            var assetPath = AssetDatabase.GetAssetPath(item);
            var assetIcon = AssetDatabase.GetCachedIcon(assetPath) as Texture2D;
            if (assetIcon != null) icon.image = assetIcon;
            gridItem.Add(icon);

            var label = new Label { text = item.name };
            label.style.position = Position.Absolute;
            label.style.bottom = 0;
            label.style.left = 0;
            label.style.right = 0;
            label.style.height = 20;
            label.style.fontSize = 12;
            label.style.marginBottom = 3;
            label.style.overflow = Overflow.Hidden;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.color = GetLabelColor();

            var truncatedText = item.name.Length > 12 ? item.name.Substring(0, 12) + "..." : item.name;
            label.text = truncatedText;

            gridItem.Add(label);
            gridItem.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == (int)MouseButton.LeftMouse) OnGridItemSelected(item, gridItem, loader);

                if (evt.button == (int)MouseButton.RightMouse)
                {
                    if ((evt.ctrlKey || loader.isLocked) && !loader.selectedScriptableObjects.Contains(item))
                    {
                        loader.selectedScriptableObjects.Add(item);
                        loader.selectedGridItems.Add(gridItem);
                        gridItem.style.backgroundColor = GetSelectedColor();
                    }
                    else if (!loader.selectedScriptableObjects.Contains(item))
                    {
                        foreach (var previousItem in loader.selectedGridItems)
                            previousItem.style.backgroundColor = GetBackgroundColor();
                        loader.selectedScriptableObjects.Clear();
                        loader.selectedGridItems.Clear();
                        loader.selectedScriptableObjects.Add(item);
                        loader.selectedGridItems.Add(gridItem);
                        gridItem.style.backgroundColor = GetSelectedColor();
                    }
                }
            });

            gridItem.RegisterCallback<PointerMoveEvent>(evt => tooltipHandler.UpdateTooltipPosition(evt.position));
            gridItem.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                if (!loader.selectedGridItems.Contains(gridItem)) gridItem.style.backgroundColor = GetBackgroundColor();
                tooltipHandler.HideTooltip();
            });

            gridItem.RegisterCallback<PointerEnterEvent>(evt =>
            {
                try
                {
                    if (!loader.selectedGridItems.Contains(gridItem))
                        gridItem.style.backgroundColor = GetBackgroundColor(true, false);

                    tooltipHandler.ShowTooltip(item.name, assetPath, evt.position);
                }
                catch
                {
                }
            });

            gridItem.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Move", action => MoveItem.Move(loader, loader.selectedScriptableObjects));
                evt.menu.AppendAction("Duplicate",
                    action => DuplicateItem.Duplicate(loader, loader.selectedScriptableObjects));
                evt.menu.AppendAction("Rename", action => RenameItem.Rename(loader, loader.selectedScriptableObjects));
                evt.menu.AppendAction("Delete", action => DeleteItem.Delete(loader, loader.selectedScriptableObjects));
            }));

            gridItem.userData = item;
            BindGridItem.Bind(gridItem, item);
            return gridItem;
        }

        public static void OnGridItemSelected(ScriptableObject item, VisualElement gridItem, Loader loader)
        {
            var isCtrlHeld = Event.current != null && (Event.current.control || Event.current.command);
            var itemAdded = false;
            var itemRemoved = false;

            if (isCtrlHeld)
            {
                if (loader.selectedScriptableObjects.Contains(item))
                {
                    loader.selectedScriptableObjects.Remove(item);
                    loader.selectedGridItems.Remove(gridItem);
                    gridItem.style.backgroundColor = GetBackgroundColor();
                    itemRemoved = true;
                }
                else
                {
                    loader.selectedScriptableObjects.Add(item);
                    loader.selectedGridItems.Add(gridItem);
                    gridItem.style.backgroundColor = GetSelectedColor();
                    itemAdded = true;
                }
            }
            else
            {
                if (!loader.isRestoringSelection && !loader.isLocked)
                {
                    foreach (var previousItem in loader.selectedGridItems)
                        previousItem.style.backgroundColor = GetBackgroundColor();
                    loader.selectedScriptableObjects.Clear();
                    loader.selectedGridItems.Clear();
                }


                if (loader.isRestoringSelection || loader.isLocked || isCtrlHeld)
                {
                    if (!loader.selectedScriptableObjects.Contains(item))
                    {
                        loader.selectedScriptableObjects.Add(item);
                        loader.selectedGridItems.Add(gridItem);
                        gridItem.style.backgroundColor = GetSelectedColor();
                        itemAdded = true;
                    }
                    else
                    {
                        loader.selectedScriptableObjects.Remove(item);
                        loader.selectedGridItems.Remove(gridItem);
                        gridItem.style.backgroundColor = GetBackgroundColor();
                        itemRemoved = true;
                    }
                }
                else
                {
                    loader.selectedScriptableObjects.Clear();
                    loader.selectedGridItems.Clear();
                    loader.selectedScriptableObjects.Add(item);
                    loader.selectedGridItems.Add(gridItem);
                    gridItem.style.backgroundColor = GetSelectedColor();
                    itemAdded = true;
                }
            }

            var shouldClear = !loader.isRestoringSelection && (
                itemRemoved ? isCtrlHeld || loader.isLocked :
                itemAdded ? !(isCtrlHeld || loader.isLocked) :
                true
            );

            if (shouldClear)
            {
                loader.itemDetailsScrollView.Clear();
                loader.openItems.Clear();
            }

            if (loader.selectedScriptableObjects.Any())
            {
                foreach (var selectedItem in loader.selectedScriptableObjects)
                    loader.ShowScriptableObjectItem(selectedItem);
                if (Event.current != null && Event.current.clickCount == 2)
                    foreach (var selectedItem in loader.selectedScriptableObjects)
                        loader.PingObjectInEditor(selectedItem);

                loader.UpdateButtonStates(loader.selectedScriptableObjects);
            }
            else
            {
                loader.UpdateButtonStates(null);
            }
        }

        public class GridItemTooltip
        {
            private readonly VisualElement overlay;
            private VisualElement tooltip;

            public GridItemTooltip(VisualElement overlay)
            {
                this.overlay = overlay;
                CreateTooltip();
            }

            private void CreateTooltip()
            {
                tooltip = new VisualElement
                {
                    style =
                    {
                        position = Position.Absolute,
                        backgroundColor = GetTooltipBackground(),
                        borderTopLeftRadius = 5,
                        borderTopRightRadius = 5,
                        borderBottomLeftRadius = 5,
                        borderBottomRightRadius = 5,
                        paddingTop = 5,
                        paddingBottom = 5,
                        paddingLeft = 5,
                        paddingRight = 5,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        visibility = Visibility.Hidden,
                        flexDirection = FlexDirection.Column,
                        flexGrow = 1,
                        maxWidth = 250
                    }
                };
                overlay.Add(tooltip);
            }

            public void ShowTooltip(string itemName, string assetPath, Vector2 position)
            {
                tooltip.Clear();

                var tooltipName = new Label(itemName)
                {
                    style =
                    {
                        color = GetTooltipTextColor(),
                        unityFontStyleAndWeight = FontStyle.Bold,
                        flexGrow = 1,
                        whiteSpace = WhiteSpace.Normal
                    }
                };

                var tooltipPath = new Label(assetPath)
                {
                    style =
                    {
                        color = GetTooltipPathColor(),
                        fontSize = 10,
                        flexGrow = 1,
                        whiteSpace = WhiteSpace.Normal
                    }
                };

                tooltip.Add(tooltipName);
                tooltip.Add(tooltipPath);
                tooltip.style.visibility = Visibility.Visible;

                UpdateTooltipPosition(position);
                tooltip.BringToFront();
            }

            public void HideTooltip()
            {
                tooltip.style.visibility = Visibility.Hidden;
            }

            public void UpdateTooltipPosition(Vector2 mousePosition)
            {
                tooltip.MarkDirtyRepaint();
                var tooltipWidth = tooltip.resolvedStyle.width;
                var localMousePosition = overlay.WorldToLocal(mousePosition);
                tooltip.style.left = localMousePosition.x - tooltipWidth / 2;
                tooltip.style.top = localMousePosition.y + 20;
            }
        }
    }
}