using Combat.Data;
using UnityEngine;

namespace Combat.Controllers
{
    public class ComboController : MonoBehaviour
    {
        private int _nextIndex;
        private float _lastAttackTime = -999f;

        public int CurrentIndex => Mathf.Max(0, _nextIndex - 1);
        public int NextIndex => _nextIndex;

        public AttackActionSO GetNextAction(WeaponDataSO weapon)
        {
            if (weapon == null || weapon.Combo == null || weapon.Combo.Count == 0)
                return null;

            if (Time.time - _lastAttackTime > weapon.ComboWindow)
                _nextIndex = 0;

            int index = Mathf.Clamp(_nextIndex, 0, weapon.Combo.Count - 1);
            AttackActionSO action = weapon.Combo[index];

            _nextIndex = index + 1;
            if (_nextIndex >= weapon.Combo.Count)
                _nextIndex = 0;

            _lastAttackTime = Time.time;
            return action;
        }

        public void ResetIfExpired(WeaponDataSO weapon)
        {
            if (weapon == null) return;
            if (Time.time - _lastAttackTime > weapon.ComboWindow)
                _nextIndex = 0;
        }

        public void Reset()
        {
            _nextIndex = 0;
            _lastAttackTime = -999f;
        }
    }
}
