using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.UI.HUD
{
    public sealed class CoreAlarmView : MonoBehaviour
    {
        [SerializeField] private RectTransform _directionIndicator;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private Color _normalColor = Color.yellow;
        [SerializeField] private Color _priorityColor = Color.red;
        [SerializeField] private float _displayDuration = 3f;

        private IEventBus _eventBus;
        private Action<OnCoreAlarmTriggered> _onAlarmTriggered;
        private Coroutine _hideCoroutine;

        private void Start()
        {
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();

            _onAlarmTriggered = HandleAlarmTriggered;
            _eventBus.Subscribe(_onAlarmTriggered);

            if (_directionIndicator != null)
                _directionIndicator.gameObject.SetActive(false);
        }

        private void HandleAlarmTriggered(OnCoreAlarmTriggered evt)
        {
            if (_directionIndicator == null) return;

            _directionIndicator.gameObject.SetActive(true);

            float angle = Mathf.Atan2(evt.EnemyDirection.z, evt.EnemyDirection.x) * Mathf.Rad2Deg;
            _directionIndicator.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            if (_indicatorImage != null)
                _indicatorImage.color = evt.IsPriority ? _priorityColor : _normalColor;

            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(_displayDuration);
            if (_directionIndicator != null)
                _directionIndicator.gameObject.SetActive(false);
            _hideCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
                _eventBus.Unsubscribe(_onAlarmTriggered);
        }
    }
}
