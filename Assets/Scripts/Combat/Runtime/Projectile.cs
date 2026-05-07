using Combat.Stats;
using UnityEngine;

namespace Combat.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private bool _useTrigger = true;

        private Rigidbody _rigidbody;
        private DamageInfo _damage;
        private GameObject _owner;
        private float _expireAt;
        private bool _consumed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        public void Launch(DamageInfo damage, GameObject owner, Vector3 direction, float speed, float lifetime)
        {
            _damage = damage;
            _owner = owner;
            _rigidbody.linearVelocity = direction.normalized * speed;
            _expireAt = Time.time + lifetime;
        }

        private void Update()
        {
            if (Time.time >= _expireAt)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_useTrigger || _consumed) return;
            _consumed = true;
            DamageApplicationHelper.TryApply(other, _damage, _owner);
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_useTrigger || _consumed) return;
            _consumed = true;
            DamageApplicationHelper.TryApply(collision.collider, _damage, _owner);
            Destroy(gameObject);
        }
    }
}
