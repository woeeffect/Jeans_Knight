using System;
using System.Collections.Generic;
using System.Linq;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fullscreen.EditorPro
{
    public class TreeMenu
    {
        private readonly Action<Type> onSwitchType;
        private readonly Dictionary<string, VisualElement> treeMenuItems = new();

        public TreeMenu(Action<Type> onSwitchType)
        {
            this.onSwitchType = onSwitchType;
        }

        public void ShowTreeMenu(string menuType, Dictionary<string, VisualElement> containers)
        {
            var container = containers[menuType];

            for (var i = 1; i < container.childCount; i++)
            {
                var currentDisplay = container[i].style.display;
                container[i].style.display =
                    currentDisplay == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (container.childCount == 1)
                switch (menuType)
                {
                    case "Core":
                        AddTreeMenuItem("Handles", typeof(Handle), container);
                        AddTreeMenuItem("Material Sound", typeof(MaterialSoundsAsset), container);
                        AddTreeMenuItem("Animation State", typeof(StateAnimation), container);
                        AddTreeMenuItem("Basic Locomotion", typeof(StateBasicLocomotion), container);
                        AddTreeMenuItem("Complete Locomotion", typeof(StateCompleteLocomotion), container);
                        AddTreeMenuItem("Global Name Variables", typeof(GlobalNameVariables), container);
                        AddTreeMenuItem("Global List Variables", typeof(GlobalListVariables), container);
                        break;
                    case "Inventory":
                        AddTreeMenuItem("Items",
                            Type.GetType("GameCreator.Runtime.Inventory.Item, GameCreator.Runtime.Inventory"),
                            container);
                        AddTreeMenuItem("Loot Tables",
                            Type.GetType("GameCreator.Runtime.Inventory.LootTable, GameCreator.Runtime.Inventory"),
                            container);
                        AddTreeMenuItem("Currencies",
                            Type.GetType("GameCreator.Runtime.Inventory.Currency, GameCreator.Runtime.Inventory"),
                            container);
                        AddTreeMenuItem("Equipment",
                            Type.GetType("GameCreator.Runtime.Inventory.Equipment, GameCreator.Runtime.Inventory"),
                            container);
                        AddTreeMenuItem("Bag Skins",
                            Type.GetType("GameCreator.Runtime.Inventory.BagSkin, GameCreator.Runtime.Inventory"),
                            container);
                        AddTreeMenuItem("Merchant Skins",
                            Type.GetType("GameCreator.Runtime.Inventory.MerchantSkin, GameCreator.Runtime.Inventory"),
                            container);
                        AddTreeMenuItem("Tinker Skins",
                            Type.GetType("GameCreator.Runtime.Inventory.TinkerSkin, GameCreator.Runtime.Inventory"),
                            container);
                        break;
                    case "Melee":
                        AddTreeMenuItem("MeleeWeapon",
                            Type.GetType("GameCreator.Runtime.Melee.MeleeWeapon, GameCreator.Runtime.Melee"),
                            container);
                        AddTreeMenuItem("Shield",
                            Type.GetType("GameCreator.Runtime.Melee.Shield, GameCreator.Runtime.Melee"), container);
                        AddTreeMenuItem("Skill",
                            Type.GetType("GameCreator.Runtime.Melee.Skill, GameCreator.Runtime.Melee"), container);
                        AddTreeMenuItem("Combos",
                            Type.GetType("GameCreator.Runtime.Melee.Combos, GameCreator.Runtime.Melee"), container);
                        AddTreeMenuItem("MeleeReaction",
                            Type.GetType("GameCreator.Runtime.Melee.MeleeReaction, GameCreator.Runtime.Melee"),
                            container);
                        break;
                    case "Shooter":
                        AddTreeMenuItem("Weapons",
                            Type.GetType("GameCreator.Runtime.Shooter.ShooterWeapon, GameCreator.Runtime.Shooter"),
                            container);
                        AddTreeMenuItem("Ammo",
                            Type.GetType("GameCreator.Runtime.Shooter.Ammo, GameCreator.Runtime.Shooter"), container);
                        AddTreeMenuItem("Reloads",
                            Type.GetType("GameCreator.Runtime.Shooter.Reload, GameCreator.Runtime.Shooter"), container);
                        AddTreeMenuItem("Sights",
                            Type.GetType("GameCreator.Runtime.Shooter.Sight, GameCreator.Runtime.Shooter"), container);
                        AddTreeMenuItem("Crosshairs",
                            Type.GetType("GameCreator.Runtime.Shooter.Crosshair, GameCreator.Runtime.Shooter"),
                            container);
                        break;
                    case "Stats":
                        AddTreeMenuItem("Attributes",
                            Type.GetType("GameCreator.Runtime.Stats.Attribute, GameCreator.Runtime.Stats"), container);
                        AddTreeMenuItem("Classes",
                            Type.GetType("GameCreator.Runtime.Stats.Class, GameCreator.Runtime.Stats"), container);
                        AddTreeMenuItem("Formulas",
                            Type.GetType("GameCreator.Runtime.Stats.Formula, GameCreator.Runtime.Stats"), container);
                        AddTreeMenuItem("Stats",
                            Type.GetType("GameCreator.Runtime.Stats.Stat, GameCreator.Runtime.Stats"), container);
                        AddTreeMenuItem("Status Effects",
                            Type.GetType("GameCreator.Runtime.Stats.StatusEffect, GameCreator.Runtime.Stats"),
                            container);
                        AddTreeMenuItem("Tables",
                            Type.GetType("GameCreator.Runtime.Stats.Table, GameCreator.Runtime.Stats"), container);
                        break;
                    case "Quests":
                        AddTreeMenuItem("Quests",
                            Type.GetType("GameCreator.Runtime.Quests.Quest, GameCreator.Runtime.Quests"), container);
                        break;
                    case "Dialogue":
                        AddTreeMenuItem("Actor",
                            Type.GetType("GameCreator.Runtime.Dialogue.Actor, GameCreator.Runtime.Dialogue"),
                            container);
                        AddTreeMenuItem("DialogueSkin",
                            Type.GetType("GameCreator.Runtime.Dialogue.DialogueSkin, GameCreator.Runtime.Dialogue"),
                            container);
                        AddTreeMenuItem("SpeechSkin",
                            Type.GetType("GameCreator.Runtime.Dialogue.SpeechSkin, GameCreator.Runtime.Dialogue"),
                            container);
                        break;
                    case "Behavior":
                        AddTreeMenuItem("Action Plan",
                            Type.GetType("GameCreator.Runtime.Behavior.ActionPlan, GameCreator.Runtime.Behavior"),
                            container);
                        AddTreeMenuItem("Behavior Tree",
                            Type.GetType("GameCreator.Runtime.Behavior.BehaviorTree, GameCreator.Runtime.Behavior"),
                            container);
                        AddTreeMenuItem("State Machine",
                            Type.GetType("GameCreator.Runtime.Behavior.StateMachine, GameCreator.Runtime.Behavior"),
                            container);
                        AddTreeMenuItem("Utility Board",
                            Type.GetType("GameCreator.Runtime.Behavior.UtilityBoard, GameCreator.Runtime.Behavior"),
                            container);
                        break;
                    case "Traversal":
                        AddTreeMenuItem("Motion Actions",
                            Type.GetType("GameCreator.Runtime.Traversal.MotionActions, GameCreator.Runtime.Traversal"),
                            container);
                        AddTreeMenuItem("Motion Interactive",
                            Type.GetType(
                                "GameCreator.Runtime.Traversal.MotionInteractive, GameCreator.Runtime.Traversal"),
                            container);
                        AddTreeMenuItem("Motion Link",
                            Type.GetType("GameCreator.Runtime.Traversal.MotionLink, GameCreator.Runtime.Traversal"),
                            container);
                        AddTreeMenuItem("StateTraverseBase",
                            Type.GetType(
                                "GameCreator.Runtime.Traversal.StateTraverseBase, GameCreator.Runtime.Traversal"),
                            container);
                        AddTreeMenuItem("StateTraverseHorizontal",
                            Type.GetType(
                                "GameCreator.Runtime.Traversal.StateTraverseHorizontal, GameCreator.Runtime.Traversal"),
                            container);
                        AddTreeMenuItem("StateTraverseVertical",
                            Type.GetType(
                                "GameCreator.Runtime.Traversal.StateTraverseVertical, GameCreator.Runtime.Traversal"),
                            container);
                        break;
                    case "LogicBlock":
                        AddTreeMenuItem("Block",
                            Type.GetType("Fullscreen.LogicBlock.Runtime.Block, Fullscreen.LogicBlock.Runtime"),
                            container);
                        break;
                    case "Inventory Extended":
                        AddTreeMenuItem(
                            "Dismantle Item",
                            Type.GetType("Fullscreen.InventoryExtended.Runtime.DismantleItem, Fullscreen.InventoryExtended.Runtime"),
                            container
                        );
                    
                        AddTreeMenuItem(
                            "Grid Recipe",
                            Type.GetType("Fullscreen.InventoryExtended.Runtime.GridRecipe, Fullscreen.InventoryExtended.Runtime"),
                            container
                        );
                    
                        AddTreeMenuItem(
                            "Item State",
                            Type.GetType("Fullscreen.InventoryExtended.Runtime.ItemState, Fullscreen.InventoryExtended.Runtime"),
                            container
                        );
                    
                        AddTreeMenuItem(
                            "List Recipe",
                            Type.GetType("Fullscreen.InventoryExtended.Runtime.ListRecipe, Fullscreen.InventoryExtended.Runtime"),
                            container
                        );
                    
                        AddTreeMenuItem(
                            "Smelt Item",
                            Type.GetType("Fullscreen.InventoryExtended.Runtime.SmeltItem, Fullscreen.InventoryExtended.Runtime"),
                            container
                        );
                    
                        break;
                    case "Mailbox":
                        AddTreeMenuItem("Letters",
                            Type.GetType("Fullscreen.Mailbox.Runtime.Letter, Fullscreen.Mailbox.Runtime"), container);
                        AddTreeMenuItem("Letter Theme",
                            Type.GetType("Fullscreen.Mailbox.Runtime.LetterTheme, Fullscreen.Mailbox.Runtime"),
                            container);
                        break;
                    case "Abilities":
                        AddTreeMenuItem("Ability",
                            Type.GetType("DaimahouGames.Runtime.Abilities.Ability, DaimahouGames.Runtime.Abilities"),
                            container);
                        AddTreeMenuItem("Impact",
                            Type.GetType("DaimahouGames.Runtime.Abilities.Impact, DaimahouGames.Runtime.Abilities"),
                            container);
                        AddTreeMenuItem("Indicator",
                            Type.GetType("DaimahouGames.Runtime.Abilities.Indicator, DaimahouGames.Runtime.Abilities"),
                            container);
                        AddTreeMenuItem("Projectile",
                            Type.GetType("DaimahouGames.Runtime.Abilities.Projectile, DaimahouGames.Runtime.Abilities"),
                            container);
                        break;
                    case "State Machine 2":
                        AddTreeMenuItem("State Machine",
                            Type.GetType(
                                "NinjutsuGames.StateMachine.Runtime.StateMachineAsset, NinjutsuGames.StateMachine.Runtime"),
                            container);
                        break;
                    case "Factions":
                        AddTreeMenuItem("Faction",
                            Type.GetType("NinjutsuGames.Runtime.Factions.Faction, NinjutsuGames.Runtime.Factions"),
                            container);
                        break;
                    case "Perks":
                        AddTreeMenuItem("Perk",
                            Type.GetType("McKinleyMassacre.Perks.Runtime.Perk, McKinleyMassacre.Perks.Runtime"),
                            container);

                        AddTreeMenuItem("PerkTree",
                            Type.GetType("McKinleyMassacre.Perks.Runtime.PerkTree, McKinleyMassacre.Perks.Runtime"),
                            container);
                        break;
                    case "COZY Weather":
                    {
                        var assembly = AppDomain.CurrentDomain.GetAssemblies()
                            .FirstOrDefault(a => a.GetName().Name == "DistantLands.Cozy.Runtime");
                        if (assembly != null)
                        {
                            AddTreeMenuItem("AmbienceProfile",
                                assembly.GetType("DistantLands.Cozy.Data.AmbienceProfile"), container);
                            AddTreeMenuItem("AtmosphereProfile",
                                assembly.GetType("DistantLands.Cozy.Data.AtmosphereProfile"), container);
                            AddTreeMenuItem("ClimateProfile", assembly.GetType("DistantLands.Cozy.Data.ClimateProfile"),
                                container);
                            AddTreeMenuItem("ForecastProfile",
                                assembly.GetType("DistantLands.Cozy.Data.ForecastProfile"), container);
                            AddTreeMenuItem("MaterialManagerProfile",
                                assembly.GetType("DistantLands.Cozy.Data.MaterialManagerProfile"), container);
                            AddTreeMenuItem("PerennialProfile",
                                assembly.GetType("DistantLands.Cozy.Data.PerennialProfile"), container);
                            AddTreeMenuItem("SatelliteProfile",
                                assembly.GetType("DistantLands.Cozy.Data.SatelliteProfile"), container);
                            AddTreeMenuItem("WeatherProfile", assembly.GetType("DistantLands.Cozy.Data.WeatherProfile"),
                                container);
                        }
                    }
                        break;
                }
        }

        public void AddTreeMenuItem(string text, Type type, VisualElement parentContainer)
        {
            if (type == null) return;

            var itemContainer = new VisualElement
                { style = { flexDirection = FlexDirection.Row, justifyContent = Justify.Center } };

            var visualElement = new VisualElement();
            visualElement.style.width = 170;
            visualElement.style.height = 45;
            var isDark = EditorGUIUtility.isProSkin;
            visualElement.style.backgroundColor = isDark
                ? new Color(0.149f, 0.149f, 0.149f)
                : new Color(0.85f, 0.85f, 0.85f);

            visualElement.style.justifyContent = Justify.Center;
            visualElement.style.alignItems = Align.Center;
            var icon = new Image
            {
                name = "Icon",
                style =
                {
                    width = 24,
                    height = 24,
                    flexShrink = 0
                }
            };

            AssignIcon(type, icon);
            visualElement.Add(icon);

            var label = new Label { text = type.Name };
            label.style.fontSize = 12;
            visualElement.Add(label);
            visualElement.AddManipulator(new Clickable(() => SwitchType(type)));
            visualElement.userData = false;
            var baseColor = isDark ? new Color(0.149f, 0.149f, 0.149f) : new Color(0.85f, 0.85f, 0.85f);
            var hoverColor = isDark ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.9f, 0.9f, 0.9f);
            visualElement.RegisterCallback<PointerEnterEvent>(evt =>
            {
                if (!(bool)visualElement.userData) visualElement.style.backgroundColor = hoverColor;
            });

            visualElement.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                if (!(bool)visualElement.userData) visualElement.style.backgroundColor = baseColor;
            });
            treeMenuItems[type.Name] = visualElement;
            itemContainer.Add(visualElement);
            parentContainer.Add(itemContainer);
        }

        private void AssignIcon(Type type, Image icon)
        {
            var assetPath = AssetDatabase.FindAssets($"t:{type.Name}").FirstOrDefault();

            if (!string.IsNullOrEmpty(assetPath))
            {
                var texture = AssetDatabase.GetCachedIcon(AssetDatabase.GUIDToAssetPath(assetPath)) as Texture2D;
                if (texture != null)
                {
                    icon.image = texture;
                    return;
                }
            }

            var defaultIconPath = "Assets/Plugins/GameCreator/Packages/Core/Editor/Gizmos/GizmoGameCreator.png";
            var defaultIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(defaultIconPath);
            if (defaultIcon != null)
                icon.image = defaultIcon;
            else
                icon.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
        }

        private void SwitchType(Type type)
        {
            onSwitchType?.Invoke(type);
        }

        public void HighlightLastClickedTreeMenuItem(Type currentType)
        {
            var isDark = EditorGUIUtility.isProSkin;
            var normalColor = isDark ? new Color(0.149f, 0.149f, 0.149f) : new Color(0.85f, 0.85f, 0.85f);
            var highlightColor = isDark ? new Color(44f / 255f, 93f / 255f, 135f / 255f) : new Color(0.5f, 0.7f, 1f);

            foreach (var item in treeMenuItems.Values)
            {
                item.userData = false;
                item.style.backgroundColor = normalColor;
            }

            if (treeMenuItems.TryGetValue(currentType?.Name, out var matchedItem))
            {
                matchedItem.userData = true;
                matchedItem.style.backgroundColor = highlightColor;
            }
        }
    }
}