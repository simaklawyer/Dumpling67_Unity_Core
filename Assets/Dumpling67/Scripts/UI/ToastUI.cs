using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToastUI : MonoBehaviour
{
    public static ToastUI Instance { get; private set; }

    [SerializeField] private Text messageText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displayTime = 2f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public void Show(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        if (messageText != null) messageText.text = message;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            yield return new WaitForSeconds(displayTime);
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = 1f - t / 0.5f;
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
    }
}
