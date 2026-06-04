using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace Fullscreen.EditorPro
{
    [Serializable]
    public class EditorProRepository : TRepository<EditorProRepository>
    {
        internal const string REPOSITORY_ID = "editorpro.settings";

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private ExcludedFolders m_ExcludedFolders = new();

        // REPOSITORY PROPERTIES: -----------------------------------------------------------------

        public override string RepositoryID => REPOSITORY_ID;

        // PROPERTIES: ----------------------------------------------------------------------------

        public ExcludedFolders ExcludedFolders => m_ExcludedFolders;

        // ENTER PLAYMODE: -----------------------------------------------------------------
        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode()
        {
            Instance = null;
        }
    }
}