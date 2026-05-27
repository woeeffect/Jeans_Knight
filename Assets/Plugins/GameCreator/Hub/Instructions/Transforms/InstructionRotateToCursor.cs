using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Rotate To Cursor")]
    [Description("Rotates the transform in place towards cursor over time.")]

    [Image(typeof(IconRotation), ColorTheme.Type.Yellow, typeof(OverlayHourglass))]
    [Category("Transforms/Rotate To Cursor")]
    [Version(1, 0, 1)]

    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the rotation over time")]
    [Parameter("Wait to Complete", "Whether to wait until the rotation is finished or not")]

    [Keywords("Rotate", "Rotation", "See", "Look", "Cursor", "Pointer")]
    [Serializable]
    public class InstructionRotateToCursor : TInstructionTransform
    {
        [Space]
        [SerializeField] private Transition m_Transition = new Transition();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"{this.m_Transform} rotate in place toward cursor over {this.m_Transition.Duration}s";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override async Task Run(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return;

            if (Camera.main == null)
            {
                Debug.LogError("Main Camera not found.");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // if (!Physics.Raycast(ray, out RaycastHit hit)) <--- it doesn't ignores trigger collieder
            // new check ignores trigger colliders
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            {
                Debug.LogWarning("Cursor ray did not hit any surface.");
                return;
            }

            Vector3 targetPosition = hit.point;

            Vector3 lookDirection = (targetPosition - gameObject.transform.position).normalized;
            lookDirection.y = 0; 

            Quaternion initialRotation = gameObject.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            ITweenInput tween = new TweenInput<Quaternion>(
                initialRotation,
                targetRotation,
                this.m_Transition.Duration,
                (a, b, t) =>
                {
                    gameObject.transform.rotation = Quaternion.LerpUnclamped(a, b, t);
                },
                Tween.GetHash(typeof(Transform), "rotation"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );

            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }
}
