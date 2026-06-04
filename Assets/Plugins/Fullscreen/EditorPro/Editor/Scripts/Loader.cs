using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fullscreen.EditorPro
{
    public class Loader
    {
        public Button changeviewButton;
        public Button createButton;
        public EditorPro editorPro;
        public VisualElement gridContainer;
        public bool gridView;
        public bool isLocked;
        public bool isRestoringSelection = false;
        public ScrollView itemDetailsScrollView;
        public Type itemType;
        public List<ScriptableObject> listScriptableObjectsItems = new();
        public List<ScriptableObject> listScriptableObjectsItemsFiltered = new();
        public VisualElement listViewContainer;
        public ListView listViewScriptableObjectsItems;
        public Button lockButton;
        public ModuleLoader moduleLoader;
        public Dictionary<string, ScriptableObject> openItems = new();
        public VisualElement overlay;
        public Button refreshButton;
        public VisualElement rootVisualElement;
        public ScrollView scrollView;
        public VisualElement searchContainer;
        public TextField searchField;
        public List<VisualElement> selectedGridItems = new();
        public List<ScriptableObject> selectedScriptableObjects = new();

        public Loader(
            VisualElement listViewContainer,
            VisualElement gridContainer,
            ScrollView itemDetailsScrollView,
            Button refreshButton,
            Button changeviewButton,
            Button createButton,
            Button lockButton,
            VisualElement rootVisualElement,
            VisualElement overlay,
            ScrollView scrollView)
        {
            this.gridContainer = gridContainer ?? throw new ArgumentNullException(nameof(gridContainer));
            this.rootVisualElement = rootVisualElement ?? throw new ArgumentNullException(nameof(rootVisualElement));
            this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
            moduleLoader = new ModuleLoader(this);
            this.listViewContainer = listViewContainer ?? throw new ArgumentNullException(nameof(listViewContainer));
            this.itemDetailsScrollView =
                itemDetailsScrollView ?? throw new ArgumentNullException(nameof(itemDetailsScrollView));
            this.refreshButton = refreshButton ?? throw new ArgumentNullException(nameof(refreshButton));
            this.changeviewButton = changeviewButton ?? throw new ArgumentNullException(nameof(changeviewButton));
            this.createButton = createButton ?? throw new ArgumentNullException(nameof(createButton));
            this.lockButton = lockButton ?? throw new ArgumentNullException(nameof(lockButton));
            gridView = EditorPrefs.GetBool("EditorPro_GridView", true);
            this.scrollView = scrollView ?? throw new ArgumentNullException(nameof(scrollView));
            SetupSearchField();
            SetupButtons();
        }

        public event Action OnItemsLoaded;

        private void SetupSearchField()
        {
            var isDark = EditorGUIUtility.isProSkin;
            var fieldBg = isDark ? new Color(0.129f, 0.129f, 0.129f) : new Color(0.9f, 0.9f, 0.9f);
            searchField = new TextField("")
            {
                style = { width = 250, height = 28, marginLeft = -1, backgroundColor = fieldBg }
            };
            var placeholder = "Search for assets...";
            searchField.value = placeholder;
            searchField.style.display = DisplayStyle.None;
            searchField.RegisterValueChangedCallback(evt => { SearchItems(evt.newValue); });
            searchField.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (string.IsNullOrEmpty(searchField.value)) searchField.value = placeholder;
            });
            searchContainer = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    top = 0,
                    left = 0
                }
            };
            searchContainer.Add(searchField);
        }

        private void SetupButtons()
        {
            SetupButtonWithIcon(refreshButton, "TreeEditor.Refresh");
            SetupButtonWithIcon(changeviewButton, gridView ? "d_ListView" : "d_GridView");
            SetupButtonWithIcon(lockButton, "Unlocked");
            SetupButtonWithIcon(createButton, "d_CreateAddNew");
            refreshButton.clicked += () => RefreshItems.Refresh(this);
            createButton.clicked += () => CreateNewItem.Create(this);
            lockButton.clicked += () =>
            {
                isLocked = !isLocked;
                UpdateLockButtonIcon();
            };
        }

        public void RebuildCurrentView()
        {
            listViewContainer.Clear();
            gridContainer.Clear();

            if (gridView)
                InitializeGridView();
            else
                InitializeListView();
        }

        public void ToggleView()
        {
            gridView = !gridView;
            EditorPrefs.SetBool("EditorPro_GridView", gridView);

            searchField.value = "Search for assets...";
            selectedScriptableObjects.Clear();
            selectedGridItems.Clear();
            openItems.Clear();
            itemDetailsScrollView.Clear();

            if (gridView)
            {
                listViewContainer.style.width = 0;
                gridContainer.style.width = Length.Percent(100);
                scrollView.style.width = 328;
                createButton.style.display = DisplayStyle.None;
            }
            else
            {
                scrollView.style.width = 0;
                gridContainer.style.width = 0;
                listViewContainer.style.width = 338;
                createButton.style.display = DisplayStyle.Flex;
            }

            RebuildCurrentView();
            UpdateButtonStates(null);
            UpdateChangeViewButtonIcon();
        }

        private void SetupButtonWithIcon(Button button, string iconName, string text = "")
        {
            var isDark = EditorGUIUtility.isProSkin;
            var baseColor = isDark ? new Color(0.149f, 0.149f, 0.149f) : new Color(0.85f, 0.85f, 0.85f);
            var hoverColor = isDark ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.9f, 0.9f, 0.9f);
            button.Clear();
            button.style.borderBottomWidth = 0;
            button.style.borderTopWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.paddingTop = 0;
            button.style.paddingBottom = 0;
            button.style.paddingLeft = 0;
            button.style.paddingRight = 0;
            button.style.marginTop = 0;
            button.style.marginBottom = 0;
            button.style.marginLeft = 0;
            button.style.marginRight = 0;
            button.style.backgroundColor = baseColor;
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = string.IsNullOrEmpty(text) ? Justify.Center : Justify.FlexStart,
                    alignItems = Align.Center,
                    height = 32,
                    width = 32,
                    backgroundColor = baseColor,
                    paddingTop = 0,
                    paddingBottom = 0,
                    paddingLeft = 0,
                    paddingRight = 0
                }
            };
            var icon = new Image
            {
                image = EditorGUIUtility.IconContent(iconName)?.image,
                scaleMode = ScaleMode.ScaleToFit
            };
            icon.style.width = 16;
            icon.style.height = 16;
            icon.style.marginRight = 5;
            container.Add(icon);
            if (!string.IsNullOrEmpty(text))
            {
                var label = new Label(text);
                label.style.fontSize = 12;
                label.style.alignSelf = Align.Center;
                container.Add(label);
            }

            button.Add(container);
            container.RegisterCallback<PointerEnterEvent>(evt => { container.style.backgroundColor = hoverColor; });
            container.RegisterCallback<PointerLeaveEvent>(evt => { container.style.backgroundColor = baseColor; });
            if (button == lockButton || button == changeviewButton) UpdateButtonIcon(button, iconName);
        }

        private void UpdateLockButtonIcon()
        {
            var iconName = isLocked ? "Locked" : "Unlocked";
            var icon = lockButton.Q<Image>();
            if (icon != null) icon.image = EditorGUIUtility.IconContent(iconName)?.image;
        }

        public void UpdateChangeViewButtonIcon()
        {
            var iconName = gridView ? "d_ListView" : "d_GridView";
            var icon = changeviewButton.Q<Image>();
            if (icon != null) icon.image = EditorGUIUtility.IconContent(iconName)?.image;
        }

        private void UpdateButtonIcon(Button button, string iconName)
        {
            var icon = button.Q<Image>();
            if (icon != null) icon.image = EditorGUIUtility.IconContent(iconName)?.image;
        }

        public void SetItemType(Type type)
        {
            itemType = type;
            if (itemType == null) return;
            if (!isLocked) itemDetailsScrollView.Clear();
            LoadItems();
        }

        public void ClearListView()
        {
            if (listViewScriptableObjectsItems != null)
            {
                listViewScriptableObjectsItems.selectionChanged -= items => ListItem.OnItemSelected(items, this);
                if (listViewContainer.Contains(listViewScriptableObjectsItems))
                    listViewContainer.Remove(listViewScriptableObjectsItems);
                listViewScriptableObjectsItems = null;
            }

            if (listViewContainer != null) listViewContainer.Clear();
        }

        public void LoadScriptableObjects(Type type, string searchTag)
        {
            var excludedFolders = EditorProRepository.Get.ExcludedFolders.Get;

            var guids = AssetDatabase.FindAssets(searchTag);
            var result = new List<ScriptableObject>();

            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);

                var isExcluded = false;
                foreach (var folder in excludedFolders)
                    if (path.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
                    {
                        isExcluded = true;
                        break;
                    }

                if (isExcluded) continue;

                var asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset is ScriptableObject so && type.IsAssignableFrom(so.GetType())) result.Add(so);
            }

            listScriptableObjectsItems = result;
        }

        public void LoadItems()
        {
            if (itemType == null) return;

            listScriptableObjectsItems.Clear();
            try
            {
                LoadScriptableObjects(itemType, "t:" + itemType.Name);
                moduleLoader.LoadAllModules();

                listScriptableObjectsItemsFiltered = new List<ScriptableObject>(listScriptableObjectsItems);

                OnItemsLoaded?.Invoke();
                EditorUtility.ClearProgressBar();
            }
            catch (Exception)
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public void InitializeGridView()
        {
            if (gridView)
            {
                gridContainer.Clear();
                searchField.style.display = DisplayStyle.Flex;
                if (!rootVisualElement.Contains(searchContainer)) rootVisualElement.Add(searchContainer);
                var addButton = GridItem.CreateAddButton(this);
                gridContainer.Add(addButton);
                var tooltipHandler = new GridItem.GridItemTooltip(overlay);
                foreach (var item in listScriptableObjectsItemsFiltered)
                {
                    var gridItem = GridItem.CreateGridItem(item, tooltipHandler, this);
                    gridContainer.Add(gridItem);
                }

                gridContainer.parent.RegisterCallback<GeometryChangedEvent>(evt =>
                {
                    GridItem.UpdateVisibleGridIcons(gridContainer, listScriptableObjectsItemsFiltered);
                });
            }
        }

        public void InitializeListView()
        {
            if (!gridView)
            {
                ClearListView();
                if (listViewContainer == null) return;
                searchField.style.display = DisplayStyle.Flex;
                if (!rootVisualElement.Contains(searchContainer)) rootVisualElement.Add(searchContainer);
                if (listScriptableObjectsItemsFiltered != null && listScriptableObjectsItemsFiltered.Any())
                {
                    listViewScriptableObjectsItems = new ListView(
                        listScriptableObjectsItemsFiltered,
                        32,
                        ListItem.MakeItem,
                        (element, index) => BindItem.Bind(this));
                    listViewScriptableObjectsItems.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        if (evt.clickCount == 2)
                        {
                            var selectedItems = listViewScriptableObjectsItems.selectedItems.OfType<ScriptableObject>();
                            foreach (var item in selectedItems) PingObjectInEditor(item);
                        }
                    });
                    listViewScriptableObjectsItems.selectionChanged += items => ListItem.OnItemSelected(items, this);
                    listViewScriptableObjectsItems.style.flexGrow = 1;
                    listViewScriptableObjectsItems.style.flexShrink = 0;
                    listViewScriptableObjectsItems.style.height = new StyleLength(Length.Percent(100));
                    listViewScriptableObjectsItems.selectionType = SelectionType.Multiple;
                    listViewScriptableObjectsItems.AddManipulator(new ContextualMenuManipulator(evt =>
                    {
                        evt.menu.AppendAction("Move", action => MoveItem.Move(this, selectedScriptableObjects));
                        evt.menu.AppendAction("Duplicate",
                            action => DuplicateItem.Duplicate(this, selectedScriptableObjects));
                        evt.menu.AppendAction("Rename", action => RenameItem.Rename(this, selectedScriptableObjects));
                        evt.menu.AppendAction("Delete", action => DeleteItem.Delete(this, selectedScriptableObjects));
                    }));
                    listViewContainer.Add(listViewScriptableObjectsItems);
                }
            }
        }

        private bool IsAlreadyOpen(ScriptableObject item)
        {
            var itemPath = AssetDatabase.GetAssetPath(item);
            return openItems.ContainsKey(itemPath);
        }

        public void ShowScriptableObjectItem(ScriptableObject item)
        {
            var isDark = EditorGUIUtility.isProSkin;
            var separatorColor = isDark ? new Color(0.129f, 0.129f, 0.129f) : new Color(0.8f, 0.8f, 0.8f);
            var itemPath = AssetDatabase.GetAssetPath(item);
            if (!IsAlreadyOpen(item))
            {
                var itemInspector = new InspectorElement();
                itemInspector.Bind(new SerializedObject(item));
                VisualElement container;
                if (itemDetailsScrollView.childCount > 0)
                {
                    container = new VisualElement();
                    container.style.borderTopWidth = 20;
                    container.style.borderTopColor = separatorColor;
                    container.Add(itemInspector);
                }
                else
                {
                    container = itemInspector;
                }

                itemDetailsScrollView.Add(container);
                openItems[itemPath] = item;
            }
        }

        public void PingObjectInEditor(ScriptableObject item)
        {
            EditorGUIUtility.PingObject(item);
        }

        public void UpdateButtonStates(List<ScriptableObject> selectedItems)
        {
            var itemsSelected = selectedItems != null && selectedItems.Any();
            lockButton.style.display = itemsSelected ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SearchItems(string searchTerm)
        {
            if (searchTerm == "Search for assets...") searchTerm = string.Empty;

            listScriptableObjectsItemsFiltered = listScriptableObjectsItems
                .Where(item => item.name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            RebuildCurrentView();
        }
    }
}