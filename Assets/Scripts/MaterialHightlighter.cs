using System.Collections.Generic;
using UnityEngine;

internal class MaterialHighlighter : MonoBehaviour
{
    [Header("材质属性")]
    [SerializeField] private Color _highlightColor = new(0.2f, 0.2f, 1, 1);
    public float highlightRadius = 0.6f;

    // 使用属性来保护颜色值
    public Color highlightColor
    {
        get => _highlightColor;
        set
        {
            _highlightColor = value;
        }
    }

    private MaterialPropertyBlock propertyBlock;
    private SpriteRenderer spriteRenderer;
    private List<Vector4> highlightedPositions = new List<Vector4>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            propertyBlock = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetFloat("_HighlightRadius", highlightRadius);
            propertyBlock.SetColor("_HighlightColor", _highlightColor);

            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
        else {
            Debug.LogError("MaterialHighlighter: 没有找到SpriteRenderer组件");
        }
    }

    public void HighlightPositions(List<Vector2Int> positions)
    {
        highlightedPositions.Clear();

        foreach (Vector2Int pos in positions) {
            Vector3 realPos = Map.GetRealPosition(pos);
            Vector4 worldPosition = new Vector4(realPos.x, realPos.y, 0, 0);
            highlightedPositions.Add(worldPosition);
        }

        UpdateShaderProperties();
    }

    public void ClearHighlights()
    {
        highlightedPositions.Clear();
        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {
        if (spriteRenderer != null && propertyBlock != null) {
            spriteRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor("_HighlightColor", _highlightColor);
            propertyBlock.SetFloat("_HighlightRadius", highlightRadius);

            if (highlightedPositions.Count > 0) {
                propertyBlock.SetInt("_HighlightCount", highlightedPositions.Count);
                propertyBlock.SetVectorArray("_HighlightPositions", highlightedPositions.ToArray());
            }
            else {
                propertyBlock.SetInt("_HighlightCount", 0);
            }

            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}