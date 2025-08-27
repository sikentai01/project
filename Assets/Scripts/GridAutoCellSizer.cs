using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridAutoCellSizer : MonoBehaviour
{
    [Header("横の列数（固定する列数）")]
    public int columns = 2;           // 横の列数（固定）

    [Header("セルの縦横比 (1=正方形, 0.5=横長, 2=縦長)")]
    public float aspectRatio = 1.0f;  // セルの縦横比（高さ = 幅 * aspectRatio）

    private GridLayoutGroup grid;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
    }

    void OnEnable()
    {
        UpdateCellSize();
    }

    void OnRectTransformDimensionsChange()
    {
        UpdateCellSize();
    }

    /// <summary>
    /// Viewportの幅を基準にセルサイズを計算して更新
    /// </summary>
    void UpdateCellSize()
    {
        //  ScrollRect から Viewport を取得
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect == null || scrollRect.viewport == null) return;

        float parentWidth = scrollRect.viewport.rect.width;
        if (parentWidth <= 0) return;

        //  スクロールバーの幅を考慮
        float scrollbarWidth = 0f;
        if (scrollRect.verticalScrollbar != null)
        {
            RectTransform sbRect = scrollRect.verticalScrollbar.GetComponent<RectTransform>();
            scrollbarWidth = sbRect.rect.width;
        }

        //  Padding & Spacing を考慮
        float totalSpacing = grid.spacing.x * (columns - 1);
        float totalPadding = grid.padding.left + grid.padding.right;
        float availableWidth = parentWidth - totalSpacing - totalPadding - scrollbarWidth;

        //  セルサイズを決定
        float cellWidth = availableWidth / columns;
        float cellHeight = cellWidth * aspectRatio;

        //  GridLayoutGroup に反映
        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
