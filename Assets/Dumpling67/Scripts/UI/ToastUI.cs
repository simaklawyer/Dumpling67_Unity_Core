using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dumpling67.Core;

namespace Dumpling67.UI
{
    /// <summary>
    /// Простой toast. Повесь на Canvas и назначь Text + Background.
    /// </summary>
    public class ToastUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float displayTime = 2.2f;

        private float _timer;
        private bool _active;

        private void OnEnable()
        {
            GameEvents.OnToastTriggered += ShowToast;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        private void OnDisable()
        {
            GameEvents.OnToastTriggered -= ShowToast;
        }

        private void Update()
        {
            if (!_active) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _active = false;
                if (canvasGroup != null) canvasGroup.alpha = 0f;
            }
        }

        private void ShowToast(string message, string type)
        {
            if (messageText != null) messageText.text = message;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            _timer = displayTime;
            _active = true;
        }
    }
}
