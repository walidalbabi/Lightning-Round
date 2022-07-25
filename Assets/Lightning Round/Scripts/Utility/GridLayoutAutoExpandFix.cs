using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutAutoExpandFix : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public CanvasScaler canvasScaler;
    public int amountPerRow = 3;

    void Start()
    {
        RecalculateGridLayout();
    }

    void RecalculateGridLayout()
    {
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;

            int count = gridLayout.transform.childCount;

            Vector2 scale = canvasScaler.referenceResolution;

            Vector3 cellSize = gridLayout.cellSize;
            Vector3 spacing = gridLayout.spacing;

            int amountPerColumn = count / amountPerRow;

            float childWidth = (scale.x - spacing.x * (amountPerRow - 1)) / amountPerRow;
            float childHeight = (scale.y - spacing.y * (amountPerColumn - 1)) / amountPerColumn;

            cellSize.x = childWidth;
            cellSize.y = childHeight;

            gridLayout.cellSize = cellSize;
        }
    }
}