using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Fullscreen.EditorPro
{
    public class InitializeContainers
    {
        private readonly TreeMenu treeMenu;
        private readonly Dictionary<string, VisualElement> containers;

        public InitializeContainers(TreeMenu treeMenu, Dictionary<string, VisualElement> containers)
        {
            this.treeMenu = treeMenu;
            this.containers = containers;
        }

        public void Setup(VisualElement sidebar, VisualElement rootVisualElement)
        {
            bool isDark = EditorGUIUtility.isProSkin;
            Color scrollBg = isDark ? new Color(0.129f, 0.129f, 0.129f) : new Color(0.8f, 0.8f, 0.8f);
            Color itemBg = isDark ? new Color(0.129f, 0.129f, 0.129f) : new Color(0.8f, 0.8f, 0.8f);
            Color hoverBg = isDark ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.92f, 0.92f, 0.92f);
            Color borderColor = isDark ? new Color(0.129f, 0.129f, 0.129f) : new Color(0.8f, 0.8f, 0.8f);

            var scrollView = new ScrollView();
            scrollView.style.height = Length.Percent(100);
            scrollView.style.width = 170;
            scrollView.style.backgroundColor = scrollBg;
            scrollView.style.flexDirection = FlexDirection.Column;
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Auto;
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            rootVisualElement.Add(scrollView);
            scrollView.style.borderRightWidth = 5;
            scrollView.style.borderRightColor = borderColor;

            var moduleIcons = new Dictionary<string, string>
            {
                { "Core", "Assets/Plugins/GameCreator/Packages/Core/Editor/Gizmos/GizmoGlobalNameVariables.png" },
                { "Inventory", "Assets/Plugins/GameCreator/Packages/Inventory/Editor/Gizmos/GizmoBag.png" },
                { "Shooter", "Assets/Plugins/GameCreator/Packages/Shooter/Editor/Gizmos/GizmoShooterWeapon.png" },
                { "Stats", "Assets/Plugins/GameCreator/Packages/Stats/Editor/Gizmos/GizmoTable.png" },
                { "Quests", "Assets/Plugins/GameCreator/Packages/Quests/Editor/Gizmos/GizmoQuest.png" },
                { "Dialogue", "Assets/Plugins/GameCreator/Packages/Dialogue/Editor/Gizmos/GizmoDialogue.png" },
                { "Melee", "Assets/Plugins/GameCreator/Packages/Melee/Editor/Gizmos/GizmoMeleeWeapon.png" },
                { "Behavior", "Assets/Plugins/GameCreator/Packages/Behavior/Editor/Gizmos/GizmoBehaviorTree.png" },
                { "Traversal", "Assets/Plugins/GameCreator/Packages/Traversal/Editor/Gizmos/GizmoMotionInteractive.png" },
                { "LogicBlock", "Assets/Plugins/Fullscreen/LogicBlock/Runtime/Icons/Block.png" },
                { "Inventory Extended", "Assets/Plugins/Fullscreen/InventoryExtended/Runtime/Icons/Recipe.png" },
                { "Mailbox", "Assets/Plugins/Fullscreen/Mailbox/Runtime/Icons/Letter.png" },
                { "State Machine 2", "Assets/Plugins/NinjutsuGames/Packages/StateMachine/Editor/Resources/Icons/StateMachine.png" },
                { "Factions", "Assets/Plugins/NinjutsuGames/Packages/Factions/Editor/Gizmos/GizmoFaction.png" },
                { "Perks", "Assets/Plugins/McKinleyMassacre/Perks/Runtime/Icons/Perk.png" },
                { "Abilities", "Assets/Plugins/DaimahouGames/Packages/Abilities/Editor/Gizmos/GizmoAbility.png" }
            };

            #if COZY_WEATHER
            moduleIcons.Add("COZY Weather", "Packages/com.distantlands.cozy.core/Editor/Resources/Icons/Modules/Weather.png");
            #endif

            string[] modules =
            {
                "Core", "Inventory", "Dialogue", "Quests", "Shooter", "Melee", "Stats",
                "Behavior", "Traversal", "LogicBlock", "Inventory Extended", "Mailbox", "Abilities", "State Machine 2", "Factions", "Perks"
                #if COZY_WEATHER
                , "COZY Weather"
                #endif
            };

            foreach (var module in modules)
            {
                var container = new VisualElement { style = { flexDirection = FlexDirection.Column, flexGrow = 0 } };
                containers[module] = container;
                scrollView.Add(container);

                var itemContainer = new VisualElement();
                itemContainer.style.flexDirection = FlexDirection.Row;
                itemContainer.style.alignItems = Align.Center;
                itemContainer.style.height = 32;
                itemContainer.style.width = 150;

                var icon = new Image
                {
                    name = "Icon",
                    style =
                    {
                        width = 27,
                        height = 27,
                        flexShrink = 0,
                        paddingLeft = 5
                    }
                };

                if (moduleIcons.TryGetValue(module, out var iconPath))
                {
                    var loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
                    icon.image = loadedTexture ?? AssetDatabase.LoadAssetAtPath<Texture2D>(
                        "Assets/Plugins/GameCreator/Packages/Core/Editor/Gizmos/GizmoGameCreator.png");
                }

                itemContainer.Add(icon);

                var label = new Label { text = module, name = "Title" };
                label.style.fontSize = 14;
                label.style.marginLeft = 8;
                itemContainer.Add(label);
                var moduleVisualElement = new VisualElement();
                moduleVisualElement.Add(itemContainer);
                moduleVisualElement.style.width = 200;
                moduleVisualElement.style.height = 32;
                moduleVisualElement.style.backgroundColor = itemBg;
                moduleVisualElement.style.justifyContent = Justify.Center;
                moduleVisualElement.style.borderTopWidth = 0;
                moduleVisualElement.style.borderBottomWidth = 0;
                moduleVisualElement.style.borderLeftWidth = 0;
                moduleVisualElement.style.borderRightWidth = 0;
                moduleVisualElement.AddManipulator(new Clickable(() => treeMenu.ShowTreeMenu(module, containers)));

                moduleVisualElement.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    moduleVisualElement.style.backgroundColor = hoverBg;
                });

                moduleVisualElement.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    moduleVisualElement.style.backgroundColor = itemBg;
                });
                container.Add(moduleVisualElement);
            }
        }
    }
}
