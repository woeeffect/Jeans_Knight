using UnityEngine;

namespace Character
{
    public static class AnimatorParams
    {
        public static readonly int IsMoving = Animator.StringToHash("IsMoving");
        public static readonly int IsRunning = Animator.StringToHash("IsRunning");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Speed = Animator.StringToHash("Speed");

        public static readonly int Block = Animator.StringToHash("Block");
        public static readonly int Dodge = Animator.StringToHash("Dodge");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int ComboIndex = Animator.StringToHash("ComboIndex");
        public static readonly int WeaponCategory = Animator.StringToHash("WeaponCategory");
    }
}
