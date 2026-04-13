using UnityEngine;

namespace Character
{
    public static class AnimatorParams
    {
        public static readonly int IsMoving = Animator.StringToHash("IsMoving");
        public static readonly int IsRunning = Animator.StringToHash("IsRunning");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Speed = Animator.StringToHash("Speed");
    }
}
