using GameCreator.Editor.Installs;
using UnityEditor;

namespace Fullscreen.EditorPro
{
    public static class UninstallEditorPro
    {
        private const string UNINSTALL_TITLE = "Are you sure you want to uninstall {0}";
        private const string UNINSTALL_MSG = "This operation cannot be undone";
        
        [MenuItem(
            itemName: "Game Creator/Uninstall/Editor Pro",
            isValidateFunction: false,
            priority: UninstallManager.PRIORITY
        )]
        
        private static void Uninstall()
        {
            UninstallManager.Uninstall("Editor Pro");

            var moduleFolder = "EditorPro";
            var path = "Assets/Plugins/Fullscreen/" + moduleFolder;
            if (!AssetDatabase.IsValidFolder(path)) return;

            var delete = EditorUtility.DisplayDialog(
                string.Format(UNINSTALL_TITLE, moduleFolder),
                UNINSTALL_MSG, 
                "Yes", "Cancel"
            );
            
            if (!delete) return;
            
            AssetDatabase.MoveAssetToTrash(path);
        }
    }
}