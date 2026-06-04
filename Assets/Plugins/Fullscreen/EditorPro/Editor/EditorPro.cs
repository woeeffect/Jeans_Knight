using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fullscreen.EditorPro
{
    public class EditorPro : EditorWindow
    {
        private const string PREF_TREE_TYPE = "EditorPro_TreeType";
        private const string PREF_SEARCH_TEXT = "EditorPro_SearchText";
        private const string PREF_GRID_VIEW = "EditorPro_GridView";
        private const string PREF_SCROLL_GRID = "EditorPro_ScrollGrid";
        private const string PREF_SCROLL_DETAILS = "EditorPro_ScrollDetails";
        private const string PREF_SELECTED_GUIDS = "EditorPro_SelectedGuids";
        private const string PREF_EXPANDED_MENUS = "EditorPro_ExpandedMenus";
        private static Texture2D s_Icon;

        private static readonly HashSet<string> TopLevelMenus = new()
        {
            "Core", "Inventory", "Melee", "Shooter", "Stats", "Quests", "Dialogue",
            "Behavior", "Traversal", "LogicBlock", "Inventory Extended", "Mailbox",
            "Abilities", "State Machine 2", "Factions", "Perks", "COZY Weather"
        };

        private static bool s_PlayModeHandlerRegistered;
        private readonly Dictionary<string, VisualElement> containers = new();
        private readonly List<ScriptableObject> listScriptableObjectsItems = new();

        private Type currentType;
        private InitializeContainers initializeContainers;
        private ScrollView itemDetailsScrollView;
        private List<ScriptableObject> listScriptableObjectsItemsFiltered = new();
        private VisualElement listViewContainer;
        private ListView listViewScriptableObjectsItems;
        private Loader loader;
        private Button refreshButton, createButton, changeviewButton, lockButton;
        public ScrollView scrollView;
        private TextField searchField;
        private TreeMenu treeMenu;

        private static string ProjectSuffix => "_" + PlayerSettings.productGUID.ToString("N");

        private void OnEnable()
        {
            RegisterPlayModeStateHandlers();
            ApplyTitle();

            titleContent = new GUIContent("\u00A0\u00A0Editor Pro");

            var isDark = EditorGUIUtility.isProSkin;
            var sidebarColor = isDark ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.9f, 0.9f, 0.9f);
            var contentColor = isDark ? new Color(0.149f, 0.149f, 0.149f) : new Color(0.94f, 0.94f, 0.94f);
            var borderColor = isDark ? new Color(0.129f, 0.129f, 0.129f) : new Color(0.8f, 0.8f, 0.8f);

            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Row;

            var sidebar = new VisualElement
            {
                style =
                {
                    width = 180,
                    backgroundColor = sidebarColor,
                    flexDirection = FlexDirection.Column,
                    flexShrink = 0,
                    flexGrow = 0,
                    height = Length.Percent(100)
                }
            };

            treeMenu = new TreeMenu(SwitchType);
            initializeContainers = new InitializeContainers(treeMenu, containers);
            initializeContainers.Setup(sidebar, rootVisualElement);

            var mainContent = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    height = Length.Percent(100)
                }
            };

            root.Add(mainContent);

            var topBar = SetupTopBar(isDark);
            mainContent.Add(topBar);

            SetupContentContainer(mainContent, contentColor, borderColor);

            ModuleDetector.DetectModules(containers);

            RestoreState();
        }

        private void OnDisable()
        {
            SaveEditorState();
            UnregisterPlayModeStateHandlers();
        }

        private void OnFocus()
        {
            ApplyTitle();
        }

        private static Texture2D GetIcon()
        {
            if (s_Icon == null)
                s_Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Assets/Plugins/Fullscreen/EditorPro/Icons/Editor.png"
                );

            return s_Icon;
        }

        private void ApplyTitle()
        {
            titleContent = new GUIContent("\u00A0\u00A0Editor Pro", GetIcon());
        }

        private static string K(string baseKey)
        {
            return baseKey + ProjectSuffix;
        }

        [MenuItem("Game Creator/Editor Pro")]
        public static void ShowWindow()
        {
            var window = GetWindow<EditorPro>("Editor Pro");
            window.minSize = new Vector2(1000, 600);
        }

        private VisualElement SetupTopBar(bool isDark)
        {
            var topBarColor = isDark ? new Color(0.129f, 0.129f, 0.129f) : new Color(0.8f, 0.8f, 0.8f);

            var topBar = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    backgroundColor = topBarColor,
                    paddingTop = 5,
                    paddingBottom = 5,
                    flexShrink = 0,
                    height = 30,
                    alignItems = Align.Center
                }
            };

            Func<string, Action, string, Button> setupButton = (text, action, tooltip) =>
            {
                var btn = new Button(action)
                {
                    text = text,
                    style =
                    {
                        display = DisplayStyle.None,
                        width = 30,
                        height = 30,
                        marginLeft = -2
                    },
                    tooltip = tooltip
                };
                return btn;
            };

            refreshButton = setupButton("", null, "Refresh editor");
            changeviewButton = setupButton("", null, "Change view");
            createButton = setupButton("", null, "Create a new asset");
            lockButton = setupButton("", null, "Prevent inspector from clearing on selection");

            var spacer = new VisualElement { style = { width = 250 } };
            var flex = new VisualElement { style = { flexGrow = 1 } };

            topBar.Add(spacer);
            topBar.Add(changeviewButton);
            topBar.Add(refreshButton);
            topBar.Add(createButton);
            topBar.Add(flex);
            topBar.Add(lockButton);

            changeviewButton.clicked += () => { loader.ToggleView(); };

            return topBar;
        }

        public void SetupContentContainer(VisualElement mainContent, Color contentColor, Color borderColor)
        {
            var overlay = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    width = Length.Percent(100),
                    height = Length.Percent(100)
                },
                pickingMode = PickingMode.Ignore
            };

            rootVisualElement.Add(overlay);

            var contentContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                    height = Length.Percent(100)
                }
            };

            mainContent.Add(contentContainer);

            listViewContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    backgroundColor = contentColor,
                    width = 0,
                    flexShrink = 0,
                    flexGrow = 0,
                    height = Length.Percent(100),
                    borderRightWidth = 10,
                    borderRightColor = borderColor
                }
            };

            scrollView = new ScrollView
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap,
                    width = 330,
                    backgroundColor = contentColor,
                    height = Length.Percent(100)
                }
            };

            var gridContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap,
                    width = Length.Percent(100),
                    overflow = Overflow.Hidden
                }
            };

            scrollView.Add(gridContainer);
            contentContainer.Add(scrollView);
            contentContainer.Add(listViewContainer);

            itemDetailsScrollView = new ScrollView
            {
                style =
                {
                    flexGrow = 1,
                    backgroundColor = contentColor,
                    height = Length.Percent(100)
                }
            };

            contentContainer.Add(itemDetailsScrollView);

            loader = new Loader(
                listViewContainer,
                gridContainer,
                itemDetailsScrollView,
                refreshButton,
                changeviewButton,
                createButton,
                lockButton,
                rootVisualElement,
                overlay,
                scrollView
            );

            loader.editorPro = this;

            searchField = loader.searchField;
            if (searchField != null)
                searchField.RegisterValueChangedCallback(evt =>
                {
                    OnSearchChanged(evt);
                    EditorPrefs.SetString(K(PREF_SEARCH_TEXT), evt.newValue);
                });

            scrollView.RegisterCallback<GeometryChangedEvent>(_ => SaveScrollPositions());
            itemDetailsScrollView.RegisterCallback<GeometryChangedEvent>(_ => SaveScrollPositions());

            mainContent.Add(loader.searchContainer);
        }

        private void SwitchType(Type type)
        {
            currentType = type;
            loader.SetItemType(currentType);
            EditorPrefs.SetString(K(PREF_TREE_TYPE), type?.AssemblyQualifiedName ?? "");

            if (!loader.isLocked)
            {
                listViewContainer.Clear();
                loader.gridContainer.Clear();
                itemDetailsScrollView.Clear();

                Action rebuild = null;
                rebuild = () =>
                {
                    loader.OnItemsLoaded -= rebuild;
                    loader.RebuildCurrentView();
                };

                loader.OnItemsLoaded += rebuild;
                loader.LoadItems();

                refreshButton.style.display = DisplayStyle.Flex;
                changeviewButton.style.display = DisplayStyle.Flex;
                lockButton.style.display = DisplayStyle.None;
            }

            treeMenu.HighlightLastClickedTreeMenuItem(currentType);
            SaveScrollPositions();
        }

        private void OnSearchChanged(ChangeEvent<string> evt)
        {
            var txt = evt.newValue.ToLower();
            listScriptableObjectsItemsFiltered = listScriptableObjectsItems
                .FindAll(item => item.name.ToLower().Contains(txt));
            UpdateListView();
        }

        private void UpdateListView()
        {
            if (listViewScriptableObjectsItems != null)
            {
                listViewScriptableObjectsItems.itemsSource = listScriptableObjectsItemsFiltered;
                listViewScriptableObjectsItems.Rebuild();
            }
        }

        private void SaveEditorState()
        {
            EditorPrefs.SetBool(K(PREF_GRID_VIEW), loader.gridView);

            var guids = loader.selectedScriptableObjects
                .Select(so => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(so)))
                .Where(guid => !string.IsNullOrEmpty(guid))
                .ToArray();

            EditorPrefs.SetString(K(PREF_SELECTED_GUIDS), string.Join(",", guids));
            EditorPrefs.SetFloat(K(PREF_SCROLL_GRID), scrollView?.scrollOffset.y ?? 0f);
            EditorPrefs.SetFloat(K(PREF_SCROLL_DETAILS), itemDetailsScrollView?.scrollOffset.y ?? 0f);

            var expanded = TopLevelMenus
                .Where(menu =>
                    containers.TryGetValue(menu, out var c) && c.childCount > 1 &&
                    c[1].style.display == DisplayStyle.Flex)
                .ToArray();

            EditorPrefs.SetString(K(PREF_EXPANDED_MENUS), string.Join(",", expanded));
        }

        private void RestoreState()
        {
            Action restoreView = null;
            restoreView = () =>
            {
                loader.OnItemsLoaded -= restoreView;

                var savedGridView = EditorPrefs.GetBool(K(PREF_GRID_VIEW), true);
                if (loader.gridView != savedGridView) loader.gridView = savedGridView;

                if (loader.gridView)
                {
                    listViewContainer.style.width = 0;
                    scrollView.style.width = 328;
                    createButton.style.display = DisplayStyle.None;
                }
                else
                {
                    scrollView.style.width = 0;
                    listViewContainer.style.width = 338;
                    createButton.style.display = DisplayStyle.Flex;
                }

                loader.RebuildCurrentView();

                if (scrollView != null)
                    scrollView.scrollOffset = new Vector2(0, EditorPrefs.GetFloat(K(PREF_SCROLL_GRID), 0f));
                if (itemDetailsScrollView != null)
                    itemDetailsScrollView.scrollOffset = new Vector2(0, EditorPrefs.GetFloat(K(PREF_SCROLL_DETAILS), 0f));
            };

            loader.OnItemsLoaded += restoreView;

            var expandedStr = EditorPrefs.GetString(K(PREF_EXPANDED_MENUS), "");
            var expandedMenus = new HashSet<string>();

            if (!string.IsNullOrEmpty(expandedStr))
            {
                var parts = expandedStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts) expandedMenus.Add(p);
            }

            foreach (var menu in TopLevelMenus)
            {
                if (!containers.TryGetValue(menu, out var container)) continue;

                treeMenu.ShowTreeMenu(menu, containers);
                if (container.childCount <= 1) continue;

                var shouldExpand = expandedMenus.Contains(menu);
                for (var i = 1; i < container.childCount; i++)
                    container[i].style.display = shouldExpand ? DisplayStyle.Flex : DisplayStyle.None;
            }

            var typeName = EditorPrefs.GetString(K(PREF_TREE_TYPE), "");
            if (!string.IsNullOrEmpty(typeName))
            {
                var type = Type.GetType(typeName);
                if (type != null && typeof(ScriptableObject).IsAssignableFrom(type))
                {
                    currentType = type;
                    treeMenu.HighlightLastClickedTreeMenuItem(type);
                    loader.SetItemType(type);
                    loader.LoadItems();

                    refreshButton.style.display = DisplayStyle.Flex;
                    changeviewButton.style.display = DisplayStyle.Flex;
                    lockButton.style.display = DisplayStyle.None;
                }
            }

            Action restoreSelection = null;
            restoreSelection = () =>
            {
                loader.OnItemsLoaded -= restoreSelection;

                var guidStr = EditorPrefs.GetString(K(PREF_SELECTED_GUIDS), "");
                if (string.IsNullOrEmpty(guidStr)) return;

                var guids = guidStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var toSelect = new List<ScriptableObject>();

                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(path)) continue;

                    var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                    if (asset != null) toSelect.Add(asset);
                }

                loader.selectedScriptableObjects.Clear();
                loader.selectedGridItems.Clear();
                itemDetailsScrollView?.Clear();
                loader.listViewScriptableObjectsItems?.ClearSelection();

                if (loader.gridView)
                {
                    loader.isRestoringSelection = true;
                    foreach (var ve in loader.gridContainer.Children())
                        if (ve.userData is ScriptableObject so && toSelect.Contains(so))
                            GridItem.OnGridItemSelected(so, ve, loader);
                    loader.isRestoringSelection = false;

                    if (loader.gridView)
                        EditorApplication.delayCall += () => RestoreGridDetails(toSelect);
                }
                else
                {
                    var listView = loader.listViewScriptableObjectsItems;
                    if (listView?.itemsSource is IList itemsSource && itemsSource.Count > 0)
                    {
                        var indices = new List<int>();

                        for (var i = 0; i < itemsSource.Count; i++)
                            if (itemsSource[i] is ScriptableObject so && toSelect.Contains(so))
                            {
                                indices.Add(i);
                                loader.selectedScriptableObjects.Add(so);
                            }

                        itemDetailsScrollView?.Clear();
                        listView.ClearSelection();
                        var lv = loader.listViewScriptableObjectsItems;
                        if (lv == null) return;
                        if (indices.Count > 0)
                        {
                            lv.SetSelection(indices);

                            lv.schedule.Execute(() =>
                            {
                                if (loader == null || loader.gridView) return;
                                if (lv.itemsSource == null) return;

                                var i0 = indices[0];
                                if (i0 < 0 || i0 >= lv.itemsSource.Count) return;

                                lv.ScrollToItem(i0);
                                lv.RefreshItems();
                            }).ExecuteLater(0);
                        }
                    }
                }

                if (loader?.searchField != null)
                {
                    var savedSearch = EditorPrefs.GetString(K(PREF_SEARCH_TEXT), "");
                    var displayText = string.IsNullOrEmpty(savedSearch) ? "Search for assets..." : savedSearch;
                    loader.searchField.value = displayText;

                    if (!string.IsNullOrEmpty(savedSearch) && savedSearch != "Search for assets...")
                    {
                        Action applySearch = null;
                        applySearch = () =>
                        {
                            loader.OnItemsLoaded -= applySearch;
                            loader.SearchItems(savedSearch);
                        };
                        loader.OnItemsLoaded += applySearch;
                    }
                }

                loader.UpdateButtonStates(loader.selectedScriptableObjects);

                if (scrollView != null)
                    scrollView.scrollOffset = new Vector2(0, EditorPrefs.GetFloat(K(PREF_SCROLL_GRID), 0f));
                if (itemDetailsScrollView != null)
                    itemDetailsScrollView.scrollOffset = new Vector2(0, EditorPrefs.GetFloat(K(PREF_SCROLL_DETAILS), 0f));
            };

            loader.OnItemsLoaded -= restoreSelection;
            loader.OnItemsLoaded += restoreSelection;

            if (loader.listScriptableObjectsItemsFiltered.Count > 0)
                EditorApplication.delayCall += () => restoreSelection();
        }

        [InitializeOnLoadMethod]
        private static void SetupRecompileHandlers()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                var window = Resources.FindObjectsOfTypeAll<EditorPro>().FirstOrDefault();
                window?.SaveEditorState();
            };

            AssemblyReloadEvents.afterAssemblyReload += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    var window = Resources.FindObjectsOfTypeAll<EditorPro>().FirstOrDefault();
                    window?.RestoreState();
                };
            };
        }

        private void RegisterPlayModeStateHandlers()
        {
            if (s_PlayModeHandlerRegistered) return;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            s_PlayModeHandlerRegistered = true;
        }

        private void UnregisterPlayModeStateHandlers()
        {
            if (!s_PlayModeHandlerRegistered) return;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            s_PlayModeHandlerRegistered = false;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                SaveEditorState();
                return;
            }

            if (state == PlayModeStateChange.EnteredEditMode)
                EditorApplication.delayCall += () =>
                {
                    var window = Resources.FindObjectsOfTypeAll<EditorPro>().FirstOrDefault();
                    window?.RestoreState();
                };
        }


        private void RestoreGridDetails(List<ScriptableObject> toSelect)
        {
            if (loader == null) return;
            if (!loader.gridView) return;
            if (toSelect == null || toSelect.Count == 0) return;
            if (itemDetailsScrollView == null) return;

            if (itemDetailsScrollView.childCount > 0) return;

            var elements = new List<VisualElement>(toSelect.Count);

            foreach (var so in toSelect)
            {
                if (so == null) continue;

                VisualElement found = null;
                foreach (var ve in loader.gridContainer.Children())
                    if (ve.userData is ScriptableObject s && s == so)
                    {
                        found = ve;
                        break;
                    }

                if (found != null) elements.Add(found);
            }

            if (elements.Count == 0) return;

            var primarySo = (ScriptableObject)elements[0].userData;
            var primaryVe = elements[0];

            var prevRestoring = loader.isRestoringSelection;

            loader.isRestoringSelection = false;
            GridItem.OnGridItemSelected(primarySo, primaryVe, loader);

            if (elements.Count > 1)
            {
                loader.isRestoringSelection = true;
                for (var i = 1; i < elements.Count; i++)
                {
                    var ve = elements[i];
                    if (ve?.userData is ScriptableObject so)
                        GridItem.OnGridItemSelected(so, ve, loader);
                }
            }

            loader.isRestoringSelection = prevRestoring;
        }

        private void SaveScrollPositions()
        {
            if (scrollView != null)
                EditorPrefs.SetFloat(K(PREF_SCROLL_GRID), scrollView.scrollOffset.y);
            if (itemDetailsScrollView != null)
                EditorPrefs.SetFloat(K(PREF_SCROLL_DETAILS), itemDetailsScrollView.scrollOffset.y);
        }
    }
}