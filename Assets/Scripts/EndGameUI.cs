using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

internal class EndGameUI : MonoBehaviour
{
    public static EndGameUI instance {  get; private set; }

    [Header("UI Components")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI messageText;
    public Image background;

    [Header("Messages by Difficulty")]
    public string[] easyMessages = { "Not Bad!", "Good Job!", "Well Played!" };
    public string[] mediumMessages = { "Great Job!", "Impressive!", "Excellent!" };
    public string[] hardMessages = { "You're Awesome!!!", "WTF?!", "NO WAY!!!" };

    private float fadeInDuration = 0.5f;
    private float displayDuration = 1.5f;
    private float fadeOutDuration = 0.1f;

    public Gradient backgroundGradient;
    private float gradientCycleSpeed = 1f;

    [Header("Particle Effects")]
    public ParticleSystem confettiParticles;

    private Coroutine showMessageCoroutine;

    private void Awake()
    {
        if (instance  != null) {
            Destroy(instance);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SetAsleep();
    }

    public void SetAsleep()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void Update()
    {
        // 更新背景渐变
        if (background != null && canvasGroup.alpha > 0) {
            float t = Mathf.PingPong(Time.time * gradientCycleSpeed, 1f);
            background.color = backgroundGradient.Evaluate(t);
        }
    }

    public void ShowEndGameMessage(GameManager.Difficulty difficulty)
    {
        if (showMessageCoroutine != null)
            StopCoroutine(showMessageCoroutine);

        showMessageCoroutine = StartCoroutine(ShowMessageCoroutine(difficulty));
    }

    private IEnumerator ShowMessageCoroutine(GameManager.Difficulty difficulty)
    {
        // 设置消息文本
        string message = GetRandomMessage(difficulty);
        if (messageText != null) {
            messageText.text = message;
            messageText.alpha = 1f;
        }

        // 启用UI
        if (canvasGroup != null) {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        // 淡入背景
        float timer = 0f;
        while (timer < fadeInDuration) {
            timer += Time.deltaTime;
            float progress = timer / fadeInDuration;

            if (canvasGroup != null)
                canvasGroup.alpha = progress;

            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (confettiParticles != null)
            confettiParticles.Play();

        yield return new WaitForSeconds(displayDuration);

        // 淡出
        timer = 0f;
        while (timer < fadeOutDuration) {
            timer += Time.deltaTime;
            float progress = timer / fadeOutDuration;

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - progress;

            yield return null;
        }

        if (canvasGroup != null) {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        showMessageCoroutine = null;
    }

    private string GetRandomMessage(GameManager.Difficulty difficulty)
    {
        string[] messages = difficulty switch
        {
            GameManager.Difficulty.EASY => easyMessages,
            GameManager.Difficulty.MEDIUM => mediumMessages,
            GameManager.Difficulty.HARD => hardMessages,
            _ => easyMessages
        };

        if (messages.Length == 0) return "Congratulations!";
        return messages[Random.Range(0, messages.Length)];
    }
}