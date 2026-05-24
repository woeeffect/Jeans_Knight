using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 0)]

    [Title("Look At Mouse")]
    [Description("Enables or disables character rotation towards the mouse pointer")]

    [Category("Characters/Rotation/Look At Mouse")]

    [Parameter("Character", "The character GameObject that rotates towards the mouse")]
    [Parameter("Enable", "Whether this behavior is enabled or disabled")]

    [Keywords("Character", "Rotate", "Mouse", "Aim", "Look")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Green)]

    [Serializable]
    public class InstructionCharacterLookAtMouse : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetBool m_Enable = new PropertyGetBool(true);

        public override string Title => $"{(this.m_Enable.ToString())} Look At Mouse on {this.m_Character}";

        protected override Task Run(Args args)
        {
            GameObject character = this.m_Character.Get(args);
            if (character == null) return DefaultResult;

            bool enable = this.m_Enable.Get(args);

            if (enable)
            {
                if (character.TryGetComponent(out TopDownLookAtMouse lookAtMouse))
                    return DefaultResult;

                TopDownLookAtMouse temp = character.AddComponent<TopDownLookAtMouse>();
                temp.enabled = true;
            }
            else
            {
                if (character.TryGetComponent(out TopDownLookAtMouse lookAtMouse))
                {
                    UnityEngine.Object.Destroy(lookAtMouse);
                }
            }

            return DefaultResult;
        }
    }
}
