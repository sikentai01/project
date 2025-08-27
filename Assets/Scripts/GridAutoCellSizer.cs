using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridAutoCellSizer : MonoBehaviour
{
    [Header("���̗񐔁i�Œ肷��񐔁j")]
    public int columns = 2;           // ���̗񐔁i�Œ�j

    [Header("�Z���̏c���� (1=�����`, 0.5=����, 2=�c��)")]
    public float aspectRatio = 1.0f;  // �Z���̏c����i���� = �� * aspectRatio�j

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
    /// Viewport�̕�����ɃZ���T�C�Y���v�Z���čX�V
    /// </summary>
    void UpdateCellSize()
    {
        //  ScrollRect ���� Viewport ���擾
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect == null || scrollRect.viewport == null) return;

        float parentWidth = scrollRect.viewport.rect.width;
        if (parentWidth <= 0) return;

        //  �X�N���[���o�[�̕����l��
        float scrollbarWidth = 0f;
        if (scrollRect.verticalScrollbar != null)
        {
            RectTransform sbRect = scrollRect.verticalScrollbar.GetComponent<RectTransform>();
            scrollbarWidth = sbRect.rect.width;
        }

        //  Padding & Spacing ���l��
        float totalSpacing = grid.spacing.x * (columns - 1);
        float totalPadding = grid.padding.left + grid.padding.right;
        float availableWidth = parentWidth - totalSpacing - totalPadding - scrollbarWidth;

        //  �Z���T�C�Y������
        float cellWidth = availableWidth / columns;
        float cellHeight = cellWidth * aspectRatio;

        //  GridLayoutGroup �ɔ��f
        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
