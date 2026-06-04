using System;
using System.Collections.Generic;
using System.Linq;
using GameCreator.Runtime.Variables;
using GameCreator.Runtime.Characters;

namespace Fullscreen.EditorPro
{
    public class ModuleLoader
    {
        private Loader loader;

        public ModuleLoader(Loader loader)
        {
            this.loader = loader;
        }

        public void LoadAllModules()
        {
            LoadCore();
            LoadQuestModule();
            LoadStatsModule();
            LoadMeleeModule();
            LoadInventoryModule();
            LoadShooterModule();
            LoadDialogueModule();
            LoadLogicBlock();
            LoadInventoryExtended();
            LoadMailbox();
            LoadStateMachine2();
            LoadFactions();
            LoadBehaviorModule();
            LoadCozyWeatherModule();
            LoadAbilities();
            LoadTraversalModule();
        }
        
        public void LoadShooterModule()
        {
            Type weaponType = Type.GetType("GameCreator.Runtime.Shooter.ShooterWeapon, GameCreator.Runtime.Shooter");
            Type ammoType = Type.GetType("GameCreator.Runtime.Shooter.Ammo, GameCreator.Runtime.Shooter");
            Type reloadType = Type.GetType("GameCreator.Runtime.Shooter.Reload, GameCreator.Runtime.Shooter");
            Type sightType = Type.GetType("GameCreator.Runtime.Shooter.Sight, GameCreator.Runtime.Shooter");
            Type crosshairType = Type.GetType("GameCreator.Runtime.Shooter.Crosshair, GameCreator.Runtime.Shooter");

            if (loader.itemType == weaponType) loader.LoadScriptableObjects(weaponType, "t:ShooterWeapon");
            else if (loader.itemType == ammoType) loader.LoadScriptableObjects(ammoType, "t:Ammo");
            else if (loader.itemType == reloadType) loader.LoadScriptableObjects(reloadType, "t:Reload");
            else if (loader.itemType == sightType) loader.LoadScriptableObjects(sightType, "t:Sight");
            else if (loader.itemType == crosshairType) loader.LoadScriptableObjects(crosshairType, "t:Crosshair");
        }

        public void LoadBehaviorModule()
        {
            Type actionPlanType = Type.GetType("GameCreator.Runtime.Behavior.ActionPlan, GameCreator.Runtime.Behavior");
            Type behaviorTreeType = Type.GetType("GameCreator.Runtime.Behavior.BehaviorTree, GameCreator.Runtime.Behavior");
            Type stateMachineType = Type.GetType("GameCreator.Runtime.Behavior.StateMachine, GameCreator.Runtime.Behavior");
            Type utilityBoardType = Type.GetType("GameCreator.Runtime.Behavior.UtilityBoard, GameCreator.Runtime.Behavior");

            if (loader.itemType == actionPlanType) loader.LoadScriptableObjects(actionPlanType, "t:ActionPlan");
            else if (loader.itemType == behaviorTreeType) loader.LoadScriptableObjects(behaviorTreeType, "t:BehaviorTree");
            else if (loader.itemType == stateMachineType) loader.LoadScriptableObjects(stateMachineType, "t:StateMachine");
            else if (loader.itemType == utilityBoardType) loader.LoadScriptableObjects(utilityBoardType, "t:UtilityBoard");
        }

        public void LoadTraversalModule()
        {
            Type motionLinkType = Type.GetType("GameCreator.Runtime.Traversal.MotionLink, GameCreator.Runtime.Traversal");
            Type motionInteractiveType = Type.GetType("GameCreator.Runtime.Traversal.MotionInteractive, GameCreator.Runtime.Traversal");
            Type motionActionsType = Type.GetType("GameCreator.Runtime.Traversal.MotionActions, GameCreator.Runtime.Traversal");
            Type stateTraverseBaseType = Type.GetType("GameCreator.Runtime.Traversal.StateTraverseBase, GameCreator.Runtime.Traversal");
            Type stateTraverseHorizontalType = Type.GetType("GameCreator.Runtime.Traversal.StateTraverseHorizontal, GameCreator.Runtime.Traversal");
            Type stateTraverseVerticalType = Type.GetType("GameCreator.Runtime.Traversal.StateTraverseVertical, GameCreator.Runtime.Traversal");

            if (loader.itemType == motionLinkType) loader.LoadScriptableObjects(motionLinkType, "t:MotionLink");
            else if (loader.itemType == motionInteractiveType) loader.LoadScriptableObjects(motionInteractiveType, "t:MotionInteractive");
            else if (loader.itemType == motionActionsType) loader.LoadScriptableObjects(motionActionsType, "t:MotionActions");
            else if (loader.itemType == stateTraverseBaseType) loader.LoadScriptableObjects(stateTraverseBaseType, "t:StateTraverseBase");
            else if (loader.itemType == stateTraverseHorizontalType) loader.LoadScriptableObjects(stateTraverseHorizontalType, "t:StateTraverseHorizontal");
            else if (loader.itemType == stateTraverseVerticalType) loader.LoadScriptableObjects(stateTraverseVerticalType, "t:StateTraverseVertical");
        }
        
        public void LoadDialogueModule()
        {
            Type actorType = Type.GetType("GameCreator.Runtime.Dialogue.Actor, GameCreator.Runtime.Dialogue");
            Type dialogueSkinType = Type.GetType("GameCreator.Runtime.Dialogue.DialogueSkin, GameCreator.Runtime.Dialogue");
            Type speechSkinType = Type.GetType("GameCreator.Runtime.Dialogue.SpeechSkin, GameCreator.Runtime.Dialogue");

            if (loader.itemType == actorType) loader.LoadScriptableObjects(actorType, "t:Actor");
            else if (loader.itemType == dialogueSkinType) loader.LoadScriptableObjects(dialogueSkinType, "t:DialogueSkin");
            else if (loader.itemType == speechSkinType) loader.LoadScriptableObjects(speechSkinType, "t:SpeechSkin");
        }

        public void LoadInventoryModule()
        {
            Type inventoryType = Type.GetType("GameCreator.Runtime.Inventory.Item, GameCreator.Runtime.Inventory");
            Type lootType = Type.GetType("GameCreator.Runtime.Inventory.LootTable, GameCreator.Runtime.Inventory");
            Type currencyType = Type.GetType("GameCreator.Runtime.Inventory.Currency, GameCreator.Runtime.Inventory");
            Type equipmentType = Type.GetType("GameCreator.Runtime.Inventory.Equipment, GameCreator.Runtime.Inventory");
            Type bagskinType = Type.GetType("GameCreator.Runtime.Inventory.BagSkin, GameCreator.Runtime.Inventory");
            Type merchantskinType = Type.GetType("GameCreator.Runtime.Inventory.MerchantSkin, GameCreator.Runtime.Inventory");
            Type tinkerskinType = Type.GetType("GameCreator.Runtime.Inventory.TinkerSkin, GameCreator.Runtime.Inventory");

            if (loader.itemType == inventoryType) loader.LoadScriptableObjects(inventoryType, "t:Item");
            else if (loader.itemType == lootType) loader.LoadScriptableObjects(lootType, "t:LootTable");
            else if (loader.itemType == currencyType) loader.LoadScriptableObjects(currencyType, "t:Currency");
            else if (loader.itemType == equipmentType) loader.LoadScriptableObjects(equipmentType, "t:Equipment");
            else if (loader.itemType == bagskinType) loader.LoadScriptableObjects(bagskinType, "t:BagSkin");
            else if (loader.itemType == merchantskinType) loader.LoadScriptableObjects(merchantskinType, "t:MerchantSkin");
            else if (loader.itemType == tinkerskinType) loader.LoadScriptableObjects(tinkerskinType, "t:TinkerSkin");
        }

        public void LoadQuestModule()
        {
            Type questType = Type.GetType("GameCreator.Runtime.Quests.Quest, GameCreator.Runtime.Quests");

            if (loader.itemType == questType) loader.LoadScriptableObjects(questType, "t:Quest");
        }

        public void LoadLogicBlock()
        {
            Type blockType = Type.GetType("Fullscreen.LogicBlock.Runtime.Block, Fullscreen.LogicBlock.Runtime");

            if (loader.itemType == blockType) loader.LoadScriptableObjects(blockType, "t:Block");
        }
        public void LoadInventoryExtended()
        {
            Type blockType = Type.GetType("Fullscreen.InventoryExtended.Runtime.Recipe, Fullscreen.InventoryExtended.Runtime");

            if (loader.itemType == blockType) loader.LoadScriptableObjects(blockType, "t:Recipe");
        }

        public void LoadMailbox()
        {
            Type letterType = Type.GetType("Fullscreen.Mailbox.Runtime.Letter, Fullscreen.Mailbox.Runtime");
            Type letterThemeType = Type.GetType("Fullscreen.Mailbox.Runtime.LetterTheme, Fullscreen.Mailbox.Runtime");

            if (loader.itemType == letterType) loader.LoadScriptableObjects(letterType, "t:Letter");
            else if (loader.itemType == letterThemeType) loader.LoadScriptableObjects(letterThemeType, "t:LetterTheme");
        }

        public void LoadAbilities()
        {
            Type abilityType = Type.GetType("DaimahouGames.Runtime.Abilities.Ability, DaimahouGames.Runtime.Abilities");
            Type impactType = Type.GetType("DaimahouGames.Runtime.Abilities.Impact, DaimahouGames.Runtime.Abilities");
            Type indicatorType = Type.GetType("DaimahouGames.Runtime.Abilities.Indicator, DaimahouGames.Runtime.Abilities");
            Type projectileType = Type.GetType("DaimahouGames.Runtime.Abilities.Projectile, DaimahouGames.Runtime.Abilities");

            if (loader.itemType == abilityType) loader.LoadScriptableObjects(abilityType, "t:Ability");
            else if (loader.itemType == impactType) loader.LoadScriptableObjects(impactType, "t:Impact");
            else if (loader.itemType == indicatorType) loader.LoadScriptableObjects(indicatorType, "t:Indicator");
            else if (loader.itemType == projectileType) loader.LoadScriptableObjects(projectileType, "t:Projectile");
        }

        public void LoadStateMachine2()
        {
            Type statemachineType = Type.GetType("NinjutsuGames.StateMachine.Runtime.StateMachineAsset, NinjutsuGames.StateMachine.Runtime");

            if (loader.itemType == statemachineType) loader.LoadScriptableObjects(statemachineType, "t:StateMachineAsset");
        }

        public void LoadFactions()
        {
            Type factionsType = Type.GetType("NinjutsuGames.Runtime.Factions.Faction, NinjutsuGames.Runtime.Factions");

            if (loader.itemType == factionsType) loader.LoadScriptableObjects(factionsType, "t:Faction");
        }

        public void LoadStatsModule()
        {
            Type attributeType = Type.GetType("GameCreator.Runtime.Stats.Attribute, GameCreator.Runtime.Stats");
            Type classType = Type.GetType("GameCreator.Runtime.Stats.Class, GameCreator.Runtime.Stats");
            Type formulaType = Type.GetType("GameCreator.Runtime.Stats.Formula, GameCreator.Runtime.Stats");
            Type statType = Type.GetType("GameCreator.Runtime.Stats.Stat, GameCreator.Runtime.Stats");
            Type statusEffectType = Type.GetType("GameCreator.Runtime.Stats.StatusEffect, GameCreator.Runtime.Stats");
            Type tableType = Type.GetType("GameCreator.Runtime.Stats.Table, GameCreator.Runtime.Stats");

            if (loader.itemType == attributeType) loader.LoadScriptableObjects(attributeType, "t:Attribute");
            else if (loader.itemType == classType) loader.LoadScriptableObjects(classType, "t:Class");
            else if (loader.itemType == formulaType) loader.LoadScriptableObjects(formulaType, "t:Formula");
            else if (loader.itemType == statType) loader.LoadScriptableObjects(statType, "t:Stat");
            else if (loader.itemType == statusEffectType) loader.LoadScriptableObjects(statusEffectType, "t:StatusEffect");
            else if (loader.itemType == tableType) loader.LoadScriptableObjects(tableType, "t:Table");
        }

        public void LoadMeleeModule()
        {
            Type meleeWeaponType = Type.GetType("GameCreator.Runtime.Melee.MeleeWeapon, GameCreator.Runtime.Melee");
            Type shieldType = Type.GetType("GameCreator.Runtime.Melee.Shield, GameCreator.Runtime.Melee");
            Type skillType = Type.GetType("GameCreator.Runtime.Melee.Skill, GameCreator.Runtime.Melee");
            Type combosType = Type.GetType("GameCreator.Runtime.Melee.Combos, GameCreator.Runtime.Melee");
            Type meleeReactionType = Type.GetType("GameCreator.Runtime.Melee.MeleeReaction, GameCreator.Runtime.Melee");

            if (loader.itemType == meleeWeaponType) loader.LoadScriptableObjects(meleeWeaponType, "t:MeleeWeapon");
            else if (loader.itemType == shieldType) loader.LoadScriptableObjects(shieldType, "t:Shield");
            else if (loader.itemType == skillType) loader.LoadScriptableObjects(skillType, "t:Skill");
            else if (loader.itemType == combosType) loader.LoadScriptableObjects(combosType, "t:Combos");
            else if (loader.itemType == meleeReactionType) loader.LoadScriptableObjects(meleeReactionType, "t:MeleeReaction");
        }

        public void LoadCore()
        {
            if (loader.itemType == typeof(GlobalNameVariables)) loader.LoadScriptableObjects(typeof(GlobalNameVariables), "t:GlobalNameVariables");
            else if (loader.itemType == typeof(GlobalListVariables)) loader.LoadScriptableObjects(typeof(GlobalListVariables), "t:GlobalListVariables");
            else if (loader.itemType == typeof(StateBasicLocomotion)) loader.LoadScriptableObjects(typeof(StateBasicLocomotion), "t:StateBasicLocomotion");
            else if (loader.itemType == typeof(StateAnimation)) loader.LoadScriptableObjects(typeof(StateAnimation), "t:StateAnimation");
            else if (loader.itemType == typeof(StateCompleteLocomotion)) loader.LoadScriptableObjects(typeof(StateCompleteLocomotion), "t:StateCompleteLocomotion");
            else if (loader.itemType == typeof(Handle)) loader.LoadScriptableObjects(typeof(Handle), "t:Handle");
        }

        public void LoadCozyWeatherModule()
        {
            Dictionary<string, Type> profileTypes = new Dictionary<string, Type>();

            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "DistantLands.Cozy.Runtime");
            if (assembly != null)
            {
                profileTypes["AmbienceProfile"] = assembly.GetType("DistantLands.Cozy.Data.AmbienceProfile");
                profileTypes["AtmosphereProfile"] = assembly.GetType("DistantLands.Cozy.Data.AtmosphereProfile");
                profileTypes["ClimateProfile"] = assembly.GetType("DistantLands.Cozy.Data.ClimateProfile");
                profileTypes["ForecastProfile"] = assembly.GetType("DistantLands.Cozy.Data.ForecastProfile");
                profileTypes["MaterialManagerProfile"] = assembly.GetType("DistantLands.Cozy.Data.MaterialManagerProfile");
                profileTypes["PerennialProfile"] = assembly.GetType("DistantLands.Cozy.Data.PerennialProfile");
                profileTypes["SatelliteProfile"] = assembly.GetType("DistantLands.Cozy.Data.SatelliteProfile");
                profileTypes["WeatherProfile"] = assembly.GetType("DistantLands.Cozy.Data.WeatherProfile");
            }

            foreach (var profile in profileTypes)
            {
                if (loader.itemType == profile.Value)
                {
                    loader.LoadScriptableObjects(profile.Value, $"t:{profile.Key}");
                    return;
                }
            }
        }
    }
}