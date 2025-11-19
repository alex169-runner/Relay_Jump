using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

internal class DifficultySelector : MonoBehaviour
{
    public static DifficultySelector Instance { get; private set; }

    [Header("Difficulty Selecting Buttons")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    [Header("Highlight Effect")]
    public Color highlightColor = Color.yellow;
    public float highlightThickness = 5f;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        easyButton.onClick.AddListener(() => OnDifficultySelected(GameManager.Difficulty.EASY));
        mediumButton.onClick.AddListener(() => OnDifficultySelected(GameManager.Difficulty.MEDIUM));
        hardButton.onClick.AddListener(() => OnDifficultySelected(GameManager.Difficulty.HARD));

        AddHoverEffect(easyButton);
        AddHoverEffect(mediumButton);
        AddHoverEffect(hardButton);
    }

    private void AddHoverEffect(Button button)
    {
        Outline outline = button.GetComponent<Outline>();
        if (outline == null)
            outline = button.gameObject.AddComponent<Outline>();

        outline.effectColor = highlightColor;
        outline.effectDistance = new Vector2(highlightThickness, highlightThickness);
        outline.enabled = false;

        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnButtonHover(button, true); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnButtonHover(button, false); });
        trigger.triggers.Add(entryExit);
    }

    private void OnButtonHover(Button button, bool isHovering)
    {
        Outline outline = button.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = isHovering;
    }

    private void OnDifficultySelected(GameManager.Difficulty difficulty)
    {
        SetAsleep();
        GameManager.Instance.SetDifficulty(difficulty);
    }

    public void SetAsleep()
    {
        easyButton.interactable = false;
        mediumButton.interactable = false;
        hardButton.interactable = false;
        gameObject.SetActive(false);

    }

    public void SetAwake()
    {
        easyButton.interactable = true;
        mediumButton.interactable = true;
        hardButton.interactable = true;
        gameObject.SetActive(true);
    }
}