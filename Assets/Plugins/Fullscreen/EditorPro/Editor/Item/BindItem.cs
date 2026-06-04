using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Reflection;
using GameCreator.Runtime.Common;

namespace Fullscreen.EditorPro
{
    public static class BindItem
    {
        public static void Bind(Loader loader)
        {
            void BindItemItem(VisualElement element, int index)
            {
                var obj = loader.listScriptableObjectsItemsFiltered[index];
                var icon = element.Q<Image>("Icon");
                var titleLabel = element.Q<Label>("Title");

                void AssignSpriteOrIcon(UnityEngine.Object obj, Sprite sprite)
                {
                    Texture2D texture;
                    if (sprite != null)
                    {
                        texture = sprite.texture;
                        icon.image = texture;
                    }
                    else
                    {
                        string assetPath = AssetDatabase.GetAssetPath(obj);
                        texture = AssetDatabase.GetCachedIcon(assetPath) as Texture2D;
                        if (texture != null)
                        {
                            icon.image = texture;
                        }
                    }
                }

                if (obj == null)
                {
                    element.style.backgroundColor = new StyleColor(Color.red);
                    element.style.height = new StyleLength(new Length(1, LengthUnit.Pixel));
                    titleLabel.text = string.Empty;
                    return;
                }

                var abilityType = Type.GetType("DaimahouGames.Runtime.Abilities.Ability, DaimahouGames.Runtime.Abilities");
                if (abilityType != null && abilityType.IsInstanceOfType(obj))
                {
                    var iconField = abilityType.GetField("m_Icon", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (iconField != null)
                    {
                        var sprite = iconField.GetValue(obj) as Sprite;
                        AssignSpriteOrIcon(obj, sprite);
                        titleLabel.text = obj.name;
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
                    (Type.GetType("NinjutsuGames.Runtime.Factions.Faction, NinjutsuGames.Runtime.Factions"), "GetSprite")
                };

                var itemType = Type.GetType("GameCreator.Runtime.Inventory.Item, GameCreator.Runtime.Inventory");
                if (itemType != null && itemType.IsInstanceOfType(obj))
                {
                    var infoProperty = itemType.GetProperty("Info");
                    if (infoProperty != null)
                    {
                        var itemInfo = infoProperty.GetValue(obj);
                        var spriteMethod = itemInfo.GetType().GetMethod("Sprite");
                        if (spriteMethod != null)
                        {
                            var sprite = spriteMethod.Invoke(itemInfo, new object[] { Args.EMPTY }) as Sprite;
                            AssignSpriteOrIcon(obj, sprite);
                            titleLabel.text = obj.name;
                            return;
                        }
                    }
                }

                foreach (var (type, methodName) in typeToSpriteMethod)
                {
                    if (type != null && type.IsInstanceOfType(obj))
                    {
                        var method = type.GetMethod(methodName);
                        if (method != null)
                        {
                            var sprite = method.Invoke(obj, new object[] { Args.EMPTY }) as Sprite;
                            AssignSpriteOrIcon(obj, sprite);
                            titleLabel.text = obj.name;
                            return;
                        }
                    }
                }

                AssignSpriteOrIcon(obj, null);
                titleLabel.text = obj.name;
            }

            loader.listViewScriptableObjectsItems.bindItem = BindItemItem;
        }
    }
}