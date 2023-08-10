using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Collocator : MonoBehaviour
{
    [Tooltip("생성된 프리팹의 부모를 설정합니다. 없으면 씬에 직접 생성됩니다.")]
    public GameObject parent;
    [Tooltip("배치할 프리팹 객체입니다.\n게임 실행시 해당 객체가 자동으로 생성됩니다.")]
    public GameObject Prefab;
    [System.Serializable]
    public struct PlacingBox
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }
    [System.Serializable]
    public struct CellSize
    {
        public float x;
        public float y;
    }
    [System.Serializable]
    public struct Padding
    {
        public float up;
        public float down;
        public float left;
        public float right;
    }
    [System.Serializable]

    public struct Spacing
    {
        public float x;
        public float y;
    }
    public enum StartCorner
    {
        UpperLeft = 0,
        UpperRight,
        LowerLeft,
        LowerRight,
    }
    public enum StartAxis
    {
        Horizontal = 0,
        Vertical,
    }

    public PlacingBox placingBox;
    public CellSize cellSize;
    public Padding padding;
    public Spacing spacing;
    public StartCorner startCorner;
    public StartAxis startAxis;
    public int amountToMake = 1;

    private int columnCount;
    private int rowCount;
    private int axisXDirection;
    private int axisYDirection;

    private Vector3 upperLeft;
    private Vector3 upperRight;
    private Vector3 lowerLeft;
    private Vector3 lowerRight;

    private Vector3 startPosition;

    void Awake()
    {
        Init();
    }
    public void Init()
    {
        upperLeft = new Vector3(placingBox.UpperLeft.x + padding.left, placingBox.UpperLeft.y - padding.up, 0);
        upperRight = new Vector3(placingBox.UpperRight.x - padding.right, placingBox.UpperRight.y - padding.down, 0);
        lowerLeft = new Vector3(placingBox.LowerLeft.x + padding.left, placingBox.LowerLeft.y + padding.up, 0);
        lowerRight = new Vector3(placingBox.LowerRight.x - padding.right, placingBox.LowerRight.y + padding.down, 0);
        columnCount = (int)((upperRight.x - upperLeft.x) / (cellSize.x + spacing.x));
        if ((upperRight.x - upperLeft.x) % (cellSize.x + spacing.x) >= cellSize.x)
            columnCount++;
        rowCount = (int)((upperLeft.y - lowerLeft.y) / (cellSize.y + spacing.y));
        if (((upperLeft.y - lowerLeft.y) % (cellSize.y + spacing.y)) >= cellSize.y)
            rowCount++;

        if (columnCount <= 0 || rowCount <= 0)
        {
            Debug.Log("위치할 박스 영역을 올바르게 지정해주세요");
            return;
        }

        switch (startCorner)
        {
            case StartCorner.UpperLeft:
                {
                    startPosition = upperLeft;
                    axisXDirection = 1;
                    axisYDirection = -1;
                    break;
                }
            case StartCorner.UpperRight:
                {
                    startPosition = upperRight;
                    axisXDirection = -1;
                    axisYDirection = -1;
                    break;
                }
            case StartCorner.LowerLeft:
                {
                    startPosition = lowerLeft;
                    axisXDirection = 1;
                    axisYDirection = 1;
                    break;
                }
            case StartCorner.LowerRight:
                {
                    startPosition = lowerRight;
                    axisXDirection = -1;
                    axisYDirection = 1;
                    break;
                }
        }
    }
    public List<GameObject> InstantiatePrefab()
    {
        List<GameObject> prefabList = new List<GameObject>();
        for (int i = 0; i < amountToMake; ++i)
        {
            GameObject prefab = Instantiate(Prefab);

            int rowNum = i / columnCount;
            int columnNum = i % columnCount;
            if (rowNum >= rowCount || columnNum >= columnCount)
            {
                Debug.Log("생성할 수 있는 영역을 넘어섰습니다.");
                Debug.Log("행 번호 : " + rowNum);
                Debug.Log("열 번호 : " + columnNum);
                return null;
            }
            if (startAxis == StartAxis.Vertical)
            {
                int temp = rowNum;
                columnNum = rowNum;
                rowNum = temp;
            }

            float x = startPosition.x + (axisXDirection * (spacing.x + cellSize.x) * columnNum) + (axisXDirection * (cellSize.x / 2f));
            float y = startPosition.y + (axisYDirection * (spacing.y + cellSize.y) * rowNum) + (axisYDirection * (cellSize.y / 2f));
            prefab.transform.position = new Vector3(x, y, 0);

            if (parent != null)
            {
                prefab.transform.parent = parent.transform;
            }

            prefabList.Add(prefab);
        }
        return prefabList;
    }
}
