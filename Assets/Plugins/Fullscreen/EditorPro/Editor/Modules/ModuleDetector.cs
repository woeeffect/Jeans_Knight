using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Fullscreen.EditorPro
{
    public static class ModuleDetector
    {
        public static void DetectModules(Dictionary<string, VisualElement> containers)
        {
            var typesToCheck = new[]
            {
#if COZY_WEATHER
                ("COZY Weather", "DistantLands.Cozy.Data.WeatherProfile, DistantLands.Cozy.Runtime"),
#endif
                ("Inventory", "GameCreator.Runtime.Inventory.Item, GameCreator.Runtime.Inventory"),
                ("Stats", "GameCreator.Runtime.Stats.Attribute, GameCreator.Runtime.Stats"),
                ("Shooter", "GameCreator.Runtime.Shooter.ShooterWeapon, GameCreator.Runtime.Shooter"),
                ("Quests", "GameCreator.Runtime.Quests.Quest, GameCreator.Runtime.Quests"),
                ("Melee", "GameCreator.Runtime.Melee.MeleeWeapon, GameCreator.Runtime.Melee"),
                ("Dialogue", "GameCreator.Runtime.Dialogue.Actor, GameCreator.Runtime.Dialogue"),
                ("Behavior", "GameCreator.Runtime.Behavior.ActionPlan, GameCreator.Runtime.Behavior"),
                ("Traversal", "GameCreator.Runtime.Traversal.MotionLink, GameCreator.Runtime.Traversal"),
                ("LogicBlock", "Fullscreen.LogicBlock.Runtime.Block, Fullscreen.LogicBlock.Runtime"),
                ("Inventory Extended", "Fullscreen.InventoryExtended.Runtime.ItemState, Fullscreen.InventoryExtended.Runtime"),
                ("Mailbox", "Fullscreen.Mailbox.Runtime.Letter, Fullscreen.Mailbox.Runtime"),
                ("Abilities", "DaimahouGames.Runtime.Abilities.Ability, DaimahouGames.Runtime.Abilities"),
                ("State Machine 2", "NinjutsuGames.StateMachine.Runtime.StateMachineAsset, NinjutsuGames.StateMachine.Runtime"),
                ("Factions", "NinjutsuGames.Runtime.Factions.Faction, NinjutsuGames.Runtime.Factions"),
                ("Perks", "McKinleyMassacre.Perks.Runtime.Perk, McKinleyMassacre.Perks.Runtime")
            };

            foreach (var (module, typeName) in typesToCheck)
            {
                Type type = null;

                if (module == "COZY Weather")
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "DistantLands.Cozy.Runtime");
                    if (assembly != null) type = assembly.GetType(typeName.Split(',')[0].Trim());
                }
                else
                {
                    type = Type.GetType(typeName);
                }

                if (type != null)
                    containers[module].style.display = DisplayStyle.Flex;
                else
                    containers[module].style.display = DisplayStyle.None;
            }
        }
    }
}