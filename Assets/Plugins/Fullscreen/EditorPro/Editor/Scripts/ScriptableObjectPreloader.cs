using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine;

namespace Fullscreen.EditorPro
{
    [InitializeOnLoad]
    public static class ScriptableObjectPreloader
    {
        private const float MaxMillisecondsPerFrame = 5f;
        private const int InitialDelayFrames = 60;

        public static Dictionary<Type, List<ScriptableObject>> PreloadedObjects = new();
        private static IEnumerator<ScriptableObject> preloadCoroutine;
        private static int totalLoaded;
        private static int frameCounter;
        private static Stopwatch frameTimer = new();

        static ScriptableObjectPreloader()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            EditorApplication.update += OnEditorUpdate;
        }

        private static bool preloadedThisSession
        {
            get => SessionState.GetBool("Fullscreen_EditorPro_Preloaded", false);
            set => SessionState.SetBool("Fullscreen_EditorPro_Preloaded", value);
        }

        private static void OnBeforeAssemblyReload()
        {
            preloadedThisSession = true;
        }

        private static void OnEditorUpdate()
        {
            if (preloadedThisSession) return;
            
            if (EditorApplication.isCompiling) return;
            if (EditorApplication.isUpdating) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        
            if (PreloadedObjects.Count > 0)
            {
                preloadedThisSession = true;
                return;
            }
        
            if (frameCounter < InitialDelayFrames)
            {
                frameCounter++;
                return;
            }
        
            EditorApplication.update -= OnEditorUpdate;
            preloadCoroutine = PreloadAllModuleLoaderScriptableObjectsAsync().GetEnumerator();
            EditorApplication.update += PreloadCoroutineStep;
        }

        private static void PreloadCoroutineStep()
        {
            frameTimer.Restart();
            var hasMore = true;

            while (hasMore && frameTimer.Elapsed.TotalMilliseconds < MaxMillisecondsPerFrame)
            {
                hasMore = preloadCoroutine.MoveNext();
                if (hasMore) totalLoaded++;
            }

            frameTimer.Stop();

            if (!hasMore)
            {
                EditorApplication.update -= PreloadCoroutineStep;
                preloadedThisSession = true;
            }
        }

        private static IEnumerable<ScriptableObject> PreloadAllModuleLoaderScriptableObjectsAsync()
        {
            var typesToPreload = GetTypesToPreload();

            foreach (var (type, searchTag) in typesToPreload.Where(x => x.type != null))
            {
                var list = new List<ScriptableObject>();

                var guids = AssetDatabase.FindAssets(searchTag);

                if (guids.Length > 0) list.Capacity = guids.Length;

                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);

                    var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                    if (assetType != null && (assetType == type || assetType.IsSubclassOf(type)))
                    {
                        var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                        if (so != null)
                        {
                            list.Add(so);
                            yield return so;
                        }
                    }
                }

                PreloadedObjects[type] = list;
            }
        }

        private static List<(Type type, string searchTag)> GetTypesToPreload()
        {
            return new List<(Type, string)>
            {
                (typeof(GlobalNameVariables), "t:GlobalNameVariables"),
                (typeof(GlobalListVariables), "t:GlobalListVariables"),
                (typeof(StateBasicLocomotion), "t:StateBasicLocomotion"),
                (typeof(StateAnimation), "t:StateAnimation"),
                (typeof(StateCompleteLocomotion), "t:StateCompleteLocomotion"),
                (typeof(Handle), "t:Handle"),
                (Type.GetType("GameCreator.Runtime.Shooter.ShooterWeapon, GameCreator.Runtime.Shooter"),
                    "t:ShooterWeapon"),
                (Type.GetType("GameCreator.Runtime.Shooter.Ammo, GameCreator.Runtime.Shooter"), "t:Ammo"),
                (Type.GetType("GameCreator.Runtime.Shooter.Reload, GameCreator.Runtime.Shooter"), "t:Reload"),
                (Type.GetType("GameCreator.Runtime.Shooter.Sight, GameCreator.Runtime.Shooter"), "t:Sight"),
                (Type.GetType("GameCreator.Runtime.Shooter.Crosshair, GameCreator.Runtime.Shooter"), "t:Crosshair"),
                (Type.GetType("GameCreator.Runtime.Behavior.ActionPlan, GameCreator.Runtime.Behavior"), "t:ActionPlan"),
                (Type.GetType("GameCreator.Runtime.Behavior.BehaviorTree, GameCreator.Runtime.Behavior"),
                    "t:BehaviorTree"),
                (Type.GetType("GameCreator.Runtime.Behavior.StateMachine, GameCreator.Runtime.Behavior"),
                    "t:StateMachine"),
                (Type.GetType("GameCreator.Runtime.Behavior.UtilityBoard, GameCreator.Runtime.Behavior"),
                    "t:UtilityBoard"),
                (Type.GetType("GameCreator.Runtime.Traversal.MotionLink, GameCreator.Runtime.Traversal"),
                    "t:MotionLink"),
                (Type.GetType("GameCreator.Runtime.Traversal.MotionInteractive, GameCreator.Runtime.Traversal"),
                    "t:MotionInteractive"),
                (Type.GetType("GameCreator.Runtime.Traversal.MotionActions, GameCreator.Runtime.Traversal"),
                    "t:MotionActions"),
                (Type.GetType("GameCreator.Runtime.Traversal.StateTraverseBase, GameCreator.Runtime.Traversal"),
                    "t:StateTraverseBase"),
                (Type.GetType("GameCreator.Runtime.Traversal.StateTraverseHorizontal, GameCreator.Runtime.Traversal"),
                    "t:StateTraverseHorizontal"),
                (Type.GetType("GameCreator.Runtime.Traversal.StateTraverseVertical, GameCreator.Runtime.Traversal"),
                    "t:StateTraverseVertical"),
                (Type.GetType("GameCreator.Runtime.Dialogue.Actor, GameCreator.Runtime.Dialogue"), "t:Actor"),
                (Type.GetType("GameCreator.Runtime.Dialogue.DialogueSkin, GameCreator.Runtime.Dialogue"),
                    "t:DialogueSkin"),
                (Type.GetType("GameCreator.Runtime.Dialogue.SpeechSkin, GameCreator.Runtime.Dialogue"), "t:SpeechSkin"),
                (Type.GetType("GameCreator.Runtime.Inventory.Item, GameCreator.Runtime.Inventory"), "t:Item"),
                (Type.GetType("GameCreator.Runtime.Inventory.LootTable, GameCreator.Runtime.Inventory"), "t:LootTable"),
                (Type.GetType("GameCreator.Runtime.Inventory.Currency, GameCreator.Runtime.Inventory"), "t:Currency"),
                (Type.GetType("GameCreator.Runtime.Inventory.Equipment, GameCreator.Runtime.Inventory"), "t:Equipment"),
                (Type.GetType("GameCreator.Runtime.Inventory.BagSkin, GameCreator.Runtime.Inventory"), "t:BagSkin"),
                (Type.GetType("GameCreator.Runtime.Inventory.MerchantSkin, GameCreator.Runtime.Inventory"),
                    "t:MerchantSkin"),
                (Type.GetType("GameCreator.Runtime.Inventory.TinkerSkin, GameCreator.Runtime.Inventory"),
                    "t:TinkerSkin"),
                (Type.GetType("GameCreator.Runtime.Quests.Quest, GameCreator.Runtime.Quests"), "t:Quest"),
                (Type.GetType("Fullscreen.LogicBlock.Runtime.Block, Fullscreen.LogicBlock.Runtime"), "t:Block"),
                (Type.GetType("Fullscreen.InventoryExtended.Runtime.Recipe, Fullscreen.InventoryExtended.Runtime"),
                    "t:Recipe"),
                (Type.GetType("Fullscreen.Mailbox.Runtime.Letter, Fullscreen.Mailbox.Runtime"), "t:Letter"),
                (Type.GetType("Fullscreen.Mailbox.Runtime.LetterTheme, Fullscreen.Mailbox.Runtime"), "t:LetterTheme"),
                (Type.GetType("DaimahouGames.Runtime.Abilities.Ability, DaimahouGames.Runtime.Abilities"), "t:Ability"),
                (Type.GetType("DaimahouGames.Runtime.Abilities.Impact, DaimahouGames.Runtime.Abilities"), "t:Impact"),
                (Type.GetType("DaimahouGames.Runtime.Abilities.Indicator, DaimahouGames.Runtime.Abilities"),
                    "t:Indicator"),
                (Type.GetType("DaimahouGames.Runtime.Abilities.Projectile, DaimahouGames.Runtime.Abilities"),
                    "t:Projectile"),
                (Type.GetType("NinjutsuGames.StateMachine.Runtime.StateMachineAsset, NinjutsuGames.StateMachine.Runtime"),
                    "t:StateMachineAsset"),
                (Type.GetType("NinjutsuGames.Runtime.Factions.Faction, NinjutsuGames.Runtime.Factions"), "t:Faction"),
                (Type.GetType("McKinleyMassacre.Perks.Runtime.Perk, McKinleyMassacre.Perks.Runtime"), "t:Perk"),
                (Type.GetType("GameCreator.Runtime.Stats.Attribute, GameCreator.Runtime.Stats"), "t:Attribute"),
                (Type.GetType("GameCreator.Runtime.Stats.Class, GameCreator.Runtime.Stats"), "t:Class"),
                (Type.GetType("GameCreator.Runtime.Stats.Formula, GameCreator.Runtime.Stats"), "t:Formula"),
                (Type.GetType("GameCreator.Runtime.Stats.Stat, GameCreator.Runtime.Stats"), "t:Stat"),
                (Type.GetType("GameCreator.Runtime.Stats.StatusEffect, GameCreator.Runtime.Stats"), "t:StatusEffect"),
                (Type.GetType("GameCreator.Runtime.Stats.Table, GameCreator.Runtime.Stats"), "t:Table"),
                (Type.GetType("GameCreator.Runtime.Melee.MeleeWeapon, GameCreator.Runtime.Melee"), "t:MeleeWeapon"),
                (Type.GetType("GameCreator.Runtime.Melee.Shield, GameCreator.Runtime.Melee"), "t:Shield"),
                (Type.GetType("GameCreator.Runtime.Melee.Skill, GameCreator.Runtime.Melee"), "t:Skill"),
                (Type.GetType("GameCreator.Runtime.Melee.Combos, GameCreator.Runtime.Melee"), "t:Combos"),
                (Type.GetType("GameCreator.Runtime.Melee.MeleeReaction, GameCreator.Runtime.Melee"), "t:MeleeReaction")
            };
        }

        public static List<ScriptableObject> GetObjectsOfType(Type type)
        {
            if (PreloadedObjects.TryGetValue(type, out var list))
                return list;
            return new List<ScriptableObject>();
        }
    }
}