using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace Amorous2.UI.Settings
{
    [Version(0, 1, 0)]
    [Title("Set VSync")]
    [Description("Enables or disables Vertical Synchronization (VSync).")]
    [Category("Settings/Set VSync")]
    [Parameter("Enable", "Whether to turn VSync on (true) or off (false)")]
    [Keywords("VSync", "Performance", "Graphics", "Refresh", "Tear")]
    [Image(typeof(IconApplication), ColorTheme.Type.TextLight)]
    [Serializable]
    public class InstructionSettingsVSync : Instruction
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [SerializeField]
        private PropertyGetBool m_Enable = new PropertyGetBool(true);

        public override string Title => $"VSync {(m_Enable.ToString())}";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            bool enable = m_Enable.Get(args);
            QualitySettings.vSyncCount = enable ? 1 : 0;
            return DefaultResult;
        }
    }
}
