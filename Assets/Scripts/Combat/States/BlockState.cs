using Character;
using Combat.Controllers;
using StateMachine;

namespace Combat.States
{
    public class BlockState : IState
    {
        private readonly HeroController _hero;
        private readonly BlockController _blockController;

        public BlockState(HeroController hero)
        {
            _hero = hero;
            _blockController = hero.GetComponent<BlockController>();
        }

        public void Enter()
        {
            _blockController?.BeginBlock();
            _hero.Animator.SetBool(AnimatorParams.Block, true);
        }

        public void Exit()
        {
            _blockController?.EndBlock();
            _hero.Animator.SetBool(AnimatorParams.Block, false);
        }

        public void Update() { }
    }
}
