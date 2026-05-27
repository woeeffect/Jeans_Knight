using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Animator Speed")]
    [Description("Changes the playback speed of all Character animations over time")]

    [Category("Characters/Animation/Change Animator Speed")]

    [Parameter("Character", "The target Character")]
    [Parameter("Speed", "The target playback speed value")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the parameter over time")]
    [Parameter("Wait to Complete", "Whether to wait until the transition is finished")]

    [Keywords("Characters", "Animation", "Animator", "Speed", "Playback", "Time")]
    [Image(typeof(IconAnimator), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterAnimatorSpeed : TInstructionCharacterProperty
    {
        [SerializeField] private ChangeDecimal m_Speed = new ChangeDecimal(1f);
        [SerializeField] private Transition m_Transition = new Transition();

        public override string Title => $"Animator Speed {this.m_Character} {this.m_Speed}";

        protected override async Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return;

            Animator animator = character.Animim?.Animator;
            if (animator == null) return;

            float valueSource = animator.speed;
            float valueTarget = (float) this.m_Speed.Get(valueSource, args);
            
            animator.speed = Mathf.Max(0f, valueTarget);

            /*ITweenInput tween = new TweenInput<float>(
                valueSource,
                Mathf.Max(0f, valueTarget),
                this.m_Transition.Duration,
                (a, b, t) => animator.speed = Mathf.Lerp(a, b, t),
                Tween.GetHash(typeof(Character), "property:animator-speed"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );

            Tween.To(character.gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);*/
        }
    }
}
