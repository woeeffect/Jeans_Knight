using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fullscreen.EditorPro
{
    [Serializable]
    public class ExcludedFolders
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private List<string> m_Folders;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ExcludedFolders()
        {
            m_Folders = new List<string>();
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public List<string> Get => m_Folders;
    }
}