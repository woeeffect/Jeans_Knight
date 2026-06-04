using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using Object = UnityEngine.Object;

namespace Fullscreen.EditorPro
{
    public static class CreateNewItem
    {
        public static void Create(Loader loader)
        {
            if (loader.itemType == null)
            {
                EditorUtility.DisplayDialog("Error", "Item type is not set.", "OK");
                return;
            }

            var newName = EditorUtility.SaveFilePanelInProject(
                $"Create New {loader.itemType.Name}",
                $"New{loader.itemType.Name}",
                "asset",
                $"Enter name for new {loader.itemType.Name}:");

            if (string.IsNullOrEmpty(newName)) return;

            var directory = Path.GetDirectoryName(newName);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            try
            {
                EditorUtility.DisplayProgressBar("Creating Item", "Creating new asset...", 0f);

                ScriptableObject newItem = ScriptableObject.CreateInstance(loader.itemType);
                AssetDatabase.CreateAsset(newItem, newName);

                Type shooterWeaponType = Type.GetType(
                    "GameCreator.Runtime.Shooter.ShooterWeapon, GameCreator.Runtime.Shooter"
                );

                if (loader.itemType == shooterWeaponType)
                {
                    const string weaponPath =
                        "Assets/Plugins/GameCreator/Packages/Shooter/Runtime/Animators/Weapon.overrideController";

                    AnimatorOverrideController controller = Object.Instantiate(
                        AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(weaponPath)
                    );

                    controller.name = Path.GetFileNameWithoutExtension(weaponPath);
                    controller.hideFlags = HideFlags.HideInHierarchy;

                    AssetDatabase.AddObjectToAsset(controller, newItem);

                    shooterWeaponType
                        .GetField("m_Controller", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.SetValue(newItem, controller);
                }

                TryAddStateController(newItem, loader.itemType);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                loader.LoadItems();

                EditorApplication.delayCall += () =>
                {
                    loader.RebuildCurrentView();

                    loader.selectedScriptableObjects.Clear();
                    loader.selectedGridItems.Clear();
                    loader.selectedScriptableObjects.Add(newItem);

                    if (!loader.gridView && loader.listViewScriptableObjectsItems != null)
                    {
                        int index = loader.listScriptableObjectsItemsFiltered.IndexOf(newItem);
                        if (index >= 0)
                        {
                            loader.listViewScriptableObjectsItems.SetSelection(new[] { index });
                        }
                    }
                };
            }
            catch
            {
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void TryAddStateController(
            ScriptableObject newItem,
            Type itemType
        )
        {
            string path = null;

            if (itemType == typeof(StateBasicLocomotion))
            {
                path = RuntimePaths.CHARACTERS +
                       "Assets/Overrides/BasicLocomotion.overrideController";
            }
            else if (itemType == typeof(StateCompleteLocomotion))
            {
                path = RuntimePaths.CHARACTERS +
                       "Assets/Overrides/CompleteLocomotion.overrideController";
            }
            else if (itemType == typeof(StateAnimation))
            {
                path = RuntimePaths.CHARACTERS +
                       "Assets/Overrides/Animation.overrideController";
            }

            if (string.IsNullOrEmpty(path)) return;

            AnimatorOverrideController source =
                AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);

            if (source == null) return;

            AnimatorOverrideController controller = Object.Instantiate(source);
            controller.name = Path.GetFileNameWithoutExtension(path);
            controller.hideFlags = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(controller, newItem);

            itemType
                .GetField("m_Controller", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(newItem, controller);
        }
    }
}
