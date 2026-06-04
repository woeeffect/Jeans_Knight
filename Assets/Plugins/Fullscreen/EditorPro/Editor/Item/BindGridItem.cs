using System;
using System.Reflection;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Fullscreen.EditorPro
{
    public static class BindGridItem
    {
        public static void Bind(VisualElement gridItem, ScriptableObject item)
        {
            var icon = gridItem.Q<Image>("Icon");
            if (icon == null) return;
            gridItem.userData = item;

            void AssignSpriteOrIcon(Object obj, Sprite sprite)
            {
                Texture2D texture;
                if (sprite != null)
                {
                    texture = sprite.texture;
                    icon.image = texture;
                }
                else
                {
                    var assetPath = AssetDatabase.GetAssetPath(obj);
                    texture = AssetDatabase.GetCachedIcon(assetPath) as Texture2D;
                    if (texture != null) icon.image = texture;
                }
            }

            var abilityType = Type.GetType("DaimahouGames.Runtime.Abilities.Ability, DaimahouGames.Runtime.Abilities");
            if (abilityType != null && abilityType.IsInstanceOfType(item))
            {
                var iconField = abilityType.GetField("m_Icon", BindingFlags.NonPublic | BindingFlags.Instance);
                if (iconField != null)
                {
                    var sprite = iconField.GetValue(item) as Sprite;
                    AssignSpriteOrIcon(item, sprite);
                    return;
                }
            }

            var typeToSpriteMethod = new (Type type, string methodName)[]
            {
                (Type.GetType("GameCreator.Runtime.Shooter.ShooterWeapon, GameCreator.Runtime.Shooter"), "GetSprite"),
                (Type.GetType("GameCreator.Runtime.Shooter.Reload, GameCreator.Runtime.Shooter"), "GetSprite"),
                (Type.GetType("GameCreator.Runtime.Shooter.Ammo, GameCreator.Runtime.Shooter"), "GetSprite"),
                (Type.GetType("GameCreator.Runtime.Stats.Stat, GameCreator.Runtime.Stats"), "GetIcon"),
                (Type.GetType("GameCreator.Runtime.Stats.StatusEffect, GameCreator.Runtime.Stats"), "GetIcon"),
                (Type.GetType("GameCreator.Runtime.Quests.Quest, GameCreator.Runtime.Quests"), "GetSprite"),
                (Type.GetType("GameCreator.Runtime.Stats.Class, GameCreator.Runtime.Stats"), "GetSprite"),
                (Type.GetType("GameCreator.Runtime.Stats.Attribute, GameCreator.Runtime.Stats"), "GetIcon"),
                (Type.GetType("GameCreator.Runtime.Melee.MeleeWeapon, GameCreator.Runtime.Melee"), "GetSprite"),
                (Type.GetType("GameCreator.Runtime.Melee.Skill, GameCreator.Runtime.Melee"), "GetSprite"),
                (Type.GetType("NinjutsuGames.Runtime.Factions.Faction, NinjutsuGames.Runtime.Factions"), "GetSprite"),
                (Type.GetType("Fullscreen.Mailbox.Runtime.Letter, Fullscreen.Mailbox.Runtime"), "GetSenderSprite")
            };

            var itemType = Type.GetType("GameCreator.Runtime.Inventory.Item, GameCreator.Runtime.Inventory");
            if (itemType != null && itemType.IsInstanceOfType(item))
            {
                var infoProperty = itemType.GetProperty("Info");
                if (infoProperty != null)
                {
                    var itemInfo = infoProperty.GetValue(item);
                    var spriteMethod = itemInfo.GetType().GetMethod("Sprite");
                    if (spriteMethod != null)
                    {
                        var sprite = spriteMethod.Invoke(itemInfo, new object[] { Args.EMPTY }) as Sprite;
                        AssignSpriteOrIcon(item, sprite);
                        return;
                    }
                }
            }

            foreach (var (type, methodName) in typeToSpriteMethod)
                if (type != null && type.IsInstanceOfType(item))
                {
                    var method = type.GetMethod(methodName);
                    if (method != null)
                    {
                        var sprite = method.Invoke(item, new object[] { Args.EMPTY }) as Sprite;
                        AssignSpriteOrIcon(item, sprite);
                        return;
                    }
                }

            AssignSpriteOrIcon(item, null);
        }
    }
}