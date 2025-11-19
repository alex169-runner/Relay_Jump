using UnityEngine;
using UnityEngine.EventSystems;

internal class Frog : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public bool isStimulated;
    public bool isHome;

    public Vector2Int pos;
    public SpriteRenderer spriteRenderer;

    public static event System.Action<Frog> OnFrogClicked;

    private void Awake()
    {
        pos = new();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.color = isHome ? (isStimulated ? Color.yellow : Color.blue) : (isStimulated ? Color.green : Color.red);
        transform.position = Map.GetRealPosition(pos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            OnFrogClicked?.Invoke(this);
        }
    }
}
