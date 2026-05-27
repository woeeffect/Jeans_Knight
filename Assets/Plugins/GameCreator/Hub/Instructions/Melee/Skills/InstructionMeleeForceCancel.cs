using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Melee
{
    [Version(0, 0, 1)]
    
    [Title("Force Cancel Skill")]
    [Description("Forces the cancellation of an ongoing Charge, Skill or Reaction being executed by a character, regardless of current state")]

    [Category("Melee/Skills/Force Cancel Skill")]
    
    [Parameter("Character", "The Character reference using a Charge, Skill or Reaction")]

    [Keywords("Melee", "Combat", "Skill", "Force", "Stop", "Reaction", "Charge", "Cancel")]
    [Image(typeof(IconMeleeSkill), ColorTheme.Type.Red, typeof(OverlayCross))]
    
    [Example(
        "Use this instruction when you need to guarantee that a skill is cancelled"
    )]
    
    [Serializable]
    public class InstructionMeleeForceCancel : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Force Cancel Skill on {this.m_Character}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Combat
                .RequestStance<MeleeStance>()
                .ForceCancel();
            
            return DefaultResult;
        }
    }
}
