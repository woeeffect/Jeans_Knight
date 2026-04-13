using EventBusSystem;
using UnityEngine;

namespace Input
{
    public interface IInputMoveHandler : IGlobalSubscriber
    {
        void OnMove(Vector2 direction);
    }
}
