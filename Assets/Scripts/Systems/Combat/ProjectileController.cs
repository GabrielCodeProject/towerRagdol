using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using RagdollRealms.Systems.Ragdoll;

namespace RagdollRealms.Systems.Combat
{
    public class ProjectileController : MonoBehaviour, ISpawnable, IConfigurableProjectile
    {
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _lifetime = 5f;
        [SerializeField] private float _damage = 25f;
        [SerializeField] private float _knockbackForce = 15f;
        [SerializeField] private ForceType _forceType = ForceType.Impact;

        private Rigidbody _rigidbody;
        private float _spawnTime;
        private IEventBus _eventBus;
        private IPoolManager _poolManager;
        private bool _hasHit;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.TryGet<IEventBus>(out var bus);
            _eventBus = bus;
            ServiceLocator.Instance.TryGet<IPoolManager>(out var pool);
            _poolManager = pool;
        }

        public void Initialize()
        {
            _hasHit = false;
            _spawnTime = Time.time;
            _rigidbody.linearVelocity = transform.forward * _speed;
        }

        public void Reset()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _hasHit = false;
        }

        private void FixedUpdate()
        {
            if (_hasHit) return;

            if (Time.time - _spawnTime >= _lifetime)
            {
                _hasHit = true;
                _poolManager.Despawn(this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasHit)
                return;

            _hasHit = true;

            int targetId = collision.gameObject.GetInstanceID();
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 force = transform.forward * _knockbackForce;

            var receiver = collision.gameObject.GetComponentInParent<RagdollForceReceiver>();
            if (receiver != null)
                receiver.ApplyForceAtPosition(force, hitPoint, _forceType);

            _eventBus.Publish(new OnProjectileHit(
                gameObject.GetInstanceID(),
                targetId,
                _damage,
                hitPoint,
                force
            ));

            _poolManager.Despawn(this);
        }

        public void Configure(float damage, float speed, float knockback, ForceType type)
        {
            _damage = damage;
            _speed = speed;
            _knockbackForce = knockback;
            _forceType = type;
        }
    }
}
