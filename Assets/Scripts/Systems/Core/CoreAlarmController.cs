using System;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Systems.CoreBase
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class CoreAlarmController : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private float _priorityRadius = 5f;
        [SerializeField] private float _alarmCooldown = 1f;

        private SphereCollider _triggerCollider;
        private float _effectiveRange;
        private float _lastAlarmTime;
        private IEventBus _eventBus;
        private ICoreManager _coreManager;
        private Action<OnCoreUpgraded> _onCoreUpgraded;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
            _coreManager = ServiceLocator.Instance.Get<ICoreManager>();

            _triggerCollider = GetComponent<SphereCollider>();
            _triggerCollider.isTrigger = true;

            _effectiveRange = _coreManager.AlarmRange;
            _triggerCollider.radius = _effectiveRange;

            _onCoreUpgraded = HandleCoreUpgraded;
            _eventBus.Subscribe(_onCoreUpgraded);
        }

        private void OnTriggerStay(Collider other)
        {
            if ((_enemyLayerMask & (1 << other.gameObject.layer)) == 0) return;

            if (Time.time - _lastAlarmTime < _alarmCooldown) return;
            _lastAlarmTime = Time.time;

            Vector3 direction = (other.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(other.transform.position, transform.position);
            bool isPriority = distance <= _priorityRadius;

            _eventBus.Publish(new OnCoreAlarmTriggered(direction, distance, isPriority));
        }

        private void HandleCoreUpgraded(OnCoreUpgraded evt)
        {
            if (_coreManager == null) return;

            // SSOT: Read calculated range from CoreManager (single source)
            _effectiveRange = _coreManager.AlarmRange;
            _triggerCollider.radius = _effectiveRange;
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe(_onCoreUpgraded);
        }
    }
}
