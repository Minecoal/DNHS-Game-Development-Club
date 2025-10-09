using System;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Grid<T>
{
    private int gridx;
    private int gridy;
    private float cellSize;

    private Vector3 origin;
    private T[,] gridArray;
    private TextMeshPro[,] debugTextArray;
    private Transform debugTextParent;
    private static Transform globalDebugTextParent;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged; // Updates the textArray only when there is a change, reduce computational resources.
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public bool showDebug = true;

    public Grid(
        int gridx, int gridy, float cellSize, // takes in a width, height, and size
        Func<Grid<T>, int, int, T> createGridObject, // passes each individual gridObject's constructor call to the Grid; more customizable than assigning all a default gridObject.
        Vector3 origin) // botton left
    {
        this.gridx = gridx;
        this.gridy = gridy;
        this.cellSize = cellSize;
        this.origin = origin;

        gridArray = new T[this.gridx, this.gridy];

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = createGridObject(this, i, j);
            }
        }

        if (showDebug)
        {
            debugTextArray = new TextMeshPro[this.gridx, this.gridy];
            debugTextParent = new GameObject("Debug Text Parent").transform;
            if (globalDebugTextParent == null)
            {
                globalDebugTextParent = new GameObject("Global Debug Text Parent").transform;
            }
            debugTextParent.SetParent(globalDebugTextParent);

            Vector3 cellOffset = new Vector3(0.5f * cellSize, 0.5f * cellSize);
            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    TextDisplayer debugText = new TextDisplayer(gridArray[i, j]?.ToString(), GetGlobalPosition(i, j) + cellOffset, 0.1f);
                    debugText.textObject.transform.SetParent(debugTextParent);
                    Debug.DrawLine(GetGlobalPosition(i, j), GetGlobalPosition(i + 1, j), Color.white, 100f);
                    Debug.DrawLine(GetGlobalPosition(i, j), GetGlobalPosition(i, j + 1), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetGlobalPosition(0, gridy), GetGlobalPosition(gridx, gridy), Color.white, 100f);
            Debug.DrawLine(GetGlobalPosition(gridx, 0), GetGlobalPosition(gridx, gridy), Color.white, 100f);
        }

        OnGridValueChanged += UpdateGrid;
    }


    public void UpdateGrid(object sender, OnGridValueChangedEventArgs e)
    {
        debugTextArray[e.x, e.y].text = gridArray[e.x, e.y]?.ToString();
    }

    private Vector3 GetGlobalPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + origin;
    }

    public void GetGridPosition(Vector3 globalPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((globalPosition - origin).x / cellSize);
        y = Mathf.FloorToInt((globalPosition - origin).y / cellSize);
    }

    /// <summary>
    /// Triggers Update for the debug textArray
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(int x, int y, T value)
    {
        if (x < gridx && y < gridy && x >= 0 && y >= 0)
        {
            gridArray[x, y] = value;
            TriggerGridObjectChanged(x, y);
        }
    }

    public void SetGridObject(Vector3 globalPosition, T value)
    {
        GetGridPosition(globalPosition, out int x, out int y);
        SetGridObject(x, y, value);
    }

    public T GetGridObject(int x, int y)
    {
        if (x < gridx && y < gridy && x >= 0 && y >= 0)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T);
        }
    }

    public T GetGridObject(Vector3 globalPosition)
    {
        GetGridPosition(globalPosition, out int x, out int y);
        return GetGridObject(x, y);
    }

    // public void SetCenter(Vector3 globalPosition)
    // {
        
    // }
}