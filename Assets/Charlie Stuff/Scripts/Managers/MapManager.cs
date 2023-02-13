using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Static")]
    public static MapManager main;

    [Header("Map")]
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    private int[,] _mapData;
    [SerializeField]
    private GridSquareBehaviour _gridSquareObj;

    public int Width => _width;
    public int Height => _height;

    public int[,] MapData => _mapData;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void CreateMap()
    {
        _mapData = new int[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                transform.position = new Vector3(x, 0, y);
                GridSquareBehaviour newGridSquare = Instantiate(_gridSquareObj, transform.position, Quaternion.identity);
                newGridSquare.gameObject.transform.name = x + ", " + y;
                newGridSquare.Init(this);

                _mapData[x, y] = 0;
            }
        }
    }

    public void EditGrid(int x, int y, int value)
    {
        _mapData[x, y] = value;
    }

    public int GetGridPos(int x, int y)
    {
        return _mapData[x, y];
    }
}
